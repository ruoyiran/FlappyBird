"""
@version: 1.0
@author: Roy
@contact: iranpeng@gmail.com
@file: dueling_dqn_training.py.py
@time: 2018/1/7
"""

import numpy as np
import random
import tensorflow as tf
import image_utils
from unity_environment import UnityEnvironment
from dueling_dqn_network import DeepQNetwork
from data_preprocessing import DataPrerocessing
from tensorflow.python.tools import freeze_graph
from game_config import GameConfig

class ExperienceBuffer(object):
    def __init__(self, buffer_size=50000):
        self.buffer = []
        self.buffer_size = buffer_size

    def add(self, experience):
        if len(self.buffer) + len(experience) >= self.buffer_size:
            clear_old_buffer_count = len(self.buffer) + len(experience) - self.buffer_size
            self.buffer[0:clear_old_buffer_count] = []
        self.buffer.extend(experience)

    def sample(self, size):
        return np.reshape(np.asarray(random.sample(self.buffer, size)), [size, 5])

    def size(self):
        return len(self.buffer)

def update_target_graph(tfVars, tau):
    total_vars = len(tfVars)
    op_hoder = []
    for idx, var in enumerate(tfVars[0:total_vars//2]):
        update_value = tau * var.value() + (1-tau) * tfVars[total_vars//2+idx].value()
        op_hoder.append(tfVars[total_vars//2+idx].assign(update_value))
    return op_hoder

def update_target(op_hoder, sess):
    for op in op_hoder:
        sess.run(op)

def export_graph(sess, model_dir, target_nodes):
    print("Exporting graph...")
    tf.train.write_graph(sess.graph_def, model_dir, 'raw_graph_def.pb', as_text=False)
    ckpt = tf.train.get_checkpoint_state(model_dir)
    output_graph = model_dir + "/graph_def.bytes"
    freeze_graph.freeze_graph(input_graph=model_dir + '/raw_graph_def.pb',
                              input_binary=True,
                              input_checkpoint=ckpt.model_checkpoint_path,
                              output_node_names=target_nodes,
                              output_graph=output_graph,
                              clear_devices=True, initializer_nodes="", input_saver="",
                              restore_op_name="save/restore_all", filename_tensor_name="save/Const:0")

if __name__ == "__main__":
    GameConfig.target_size = 84
    GameConfig.n_channels = 4
    tf.reset_default_graph()
    h_size = 512
    learning_rate = 1e-4
    decay_steps = 300000
    final_learning_rate = 1e-6
    n_actions = 2
    with tf.variable_scope("MainQNetwork"):
        mainQN = DeepQNetwork(h_size, n_actions, learning_rate)
    with tf.variable_scope("TargetQNetwork"):
        targetQN = DeepQNetwork(h_size, n_actions, learning_rate)
    pre_train_steps = 0# 50000
    total_steps = 2000000
    anneling_steps = 50000.
    input_size = GameConfig.target_size * GameConfig.target_size * GameConfig.n_channels
    start_e = 1
    end_e = 0.00001
    e = end_e
    update_freq = 4
    batch_size = 32
    tau = 0.001
    step_drop_e = (start_e - end_e) / anneling_steps
    experience_buffer = ExperienceBuffer()
    trainables = tf.trainable_variables()
    targetOps = update_target_graph(trainables, tau)
    y = 0.99
    saver = tf.train.Saver()
    model_dir = "../models"  # The model_dir to save our model to.
    dp = DataPrerocessing()
    env = UnityEnvironment()
    with tf.Session() as sess:
        if False:
            writer = tf.summary.FileWriter(logdir="../log", graph=sess.graph)
            writer.close()

        checkpoint = tf.train.get_checkpoint_state(model_dir)
        if checkpoint and checkpoint.model_checkpoint_path:
            saver.restore(sess, checkpoint.model_checkpoint_path)
            print("Successfully loaded:", checkpoint.model_checkpoint_path)
        else:
            print("Could not find old network weights")
            sess.run(tf.global_variables_initializer())
        update_target(targetOps, sess)
        s = env.reset()
        s_t = dp.get_state_stacked_image(sess, s)

        rList = []
        total_score = 0
        max_score = 0
        prev_reward = 0
        is_saved = False
        for step in range(total_steps + 1):
            if np.random.rand(1) < e:
                print("================= Random Action ====================")
                a = np.random.randint(0, n_actions)
            else:
                a = sess.run(mainQN.predict, feed_dict={mainQN.input_x: [s_t]})[0]
                qvalues = sess.run(mainQN.Qout, feed_dict={mainQN.input_x: [s_t]})[0]

            s1, r, d = env.step(a)
            if r == 10 and prev_reward != 10:
                prev_reward = r
                total_score += 1
            if d:
                if total_score > max_score:
                    max_score = total_score
                print("Step: %d ====> Bird dead, got score: %d, max score: %d" % (step, total_score, max_score))
                total_score = 0
                s1 = env.reset()
            prev_reward = r
            s_t1 = dp.get_state_stacked_image(sess, s1, s_t)
            # image_utils.show_image(s_t1[:,:,0], "gray")
            # image_utils.show_image(s_t1[:,:,1], "gray")
            # image_utils.show_image(s_t1[:,:,2], "gray")
            # image_utils.show_image(s_t1[:,:,3], "gray")

            rList.append(r)
            # if len(rList) > 25 and np.mean(rList[-25:]) > 20:
            #     print("Action:", a)
            #     cv.imshow("Image", s1)
            #     cv.waitKey()
            experience_buffer.add(
                np.reshape(np.array([s_t.reshape([input_size]), a, r, s_t1.reshape([input_size]), d]), [1, 5]))

            if step > pre_train_steps:
                if e > end_e:
                    e -= step_drop_e
                if step % decay_steps == 0:
                    curr_lr = sess.run(mainQN.lr)
                    if curr_lr > final_learning_rate:
                        sess.run(tf.assign(mainQN.lr, curr_lr/10.0))
                if step % update_freq == 0 and experience_buffer.size() >= batch_size:
                    is_saved = False
                    train_batch = experience_buffer.sample(batch_size)
                    inputs = np.reshape(np.vstack(train_batch[:, 3]),
                                        [-1, GameConfig.target_size, GameConfig.target_size, GameConfig.n_channels])
                    A = sess.run(mainQN.predict, feed_dict={
                        mainQN.input_x: inputs
                    })
                    Q = sess.run(targetQN.Qout, feed_dict={
                        targetQN.input_x: inputs
                    })
                    doubleQ = Q[range(batch_size), A]
                    targetQ = train_batch[:, 2] + y * doubleQ

                    inputs = np.reshape(np.vstack(train_batch[:, 0]),
                                        [-1, GameConfig.target_size, GameConfig.target_size, GameConfig.n_channels])
                    _, loss_val = sess.run([mainQN.update_model, mainQN.loss], feed_dict={
                        mainQN.input_x: inputs,
                        mainQN.actions: train_batch[:, 1],
                        mainQN.targetQ: targetQ})
                    update_target(targetOps, sess)
            s_t = s_t1
            if step > 0 and step % 25 == 0:
                print("step: {}, average reward of last 25 episodes {}, e: {}, learning_rate: {}".format(step, np.mean(rList), e, sess.run(mainQN.lr)))
                rList = []
            global_step = sess.run(mainQN.global_step)
            if global_step > 0 and global_step % 10000 == 0 and not is_saved:
                model_path = "{}/model-{}.ckpt".format(model_dir, global_step)
                print("===> Global Step: {}, Saving mode to {}".format(global_step, model_path))
                saver.save(sess=sess, save_path=model_path)
                is_saved = True
            # if step > 0 and step % 10000 == 0:
            #     export_graph(sess, model_dir, target_nodes="MainQNetwork/Qout/QValue")
        export_graph(sess, model_dir, target_nodes="MainQNetwork/Qout/QValue")
        print("Training finished.")

    env.close()
