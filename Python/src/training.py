"""
@version: 1.0
@author: Roy
@contact: iranpeng@gmail.com
@file: grid_world_training.py
@time: 2018/1/7
"""

import numpy as np
import random
import tensorflow as tf
from unity_environment import UnityEnvironment
from dqn_network import DeepQNetwork
from tensorflow.python.tools import freeze_graph

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

if __name__ == '__main__':
    tf.reset_default_graph()
    env = UnityEnvironment()

    load_model = False  # Whether to load a saved model.
    input_size = 84 * 84 * 3
    h_size = 512
    learning_rate = 0.0001
    n_actions = 2
    mainQN = DeepQNetwork(h_size, n_actions, learning_rate)
    targetQN = DeepQNetwork(h_size, n_actions, learning_rate)
    pre_train_steps = 10000
    total_steps = 1000000
    anneling_steps = 10000.
    start_e = 1
    end_e = 0.1
    e = start_e
    update_freq = 4
    batch_size = 32
    tau = 0.001
    step_drop_e = (start_e - end_e) / anneling_steps
    experience_buffer = ExperienceBuffer()
    trainables = tf.trainable_variables()
    targetOps = update_target_graph(trainables, tau)
    y = 0.99
    saver = tf.train.Saver()
    path = "./dqn"  # The path to save our model to.
    with tf.Session() as sess:
        sess.run(tf.global_variables_initializer())
        update_target(targetOps, sess)
        s = env.reset()
        rList = []
        for step in range(total_steps + 1):
            if step <= pre_train_steps or np.random.rand(1) < e:
                a = np.random.randint(0, n_actions)
            else:
                a = sess.run(mainQN.predict, feed_dict={mainQN.input_x: [s]})[0]
            s1, r, d = env.step(a)
            rList.append(r)
            # if len(rList) > 25 and np.mean(rList[-25:]) > 20:
            #     print("Action:", a)
            #     cv.imshow("Image", s1)
            #     cv.waitKey()
            experience_buffer.add(np.reshape(np.array([s.reshape([input_size]), a, r, s1.reshape([input_size]), d]), [1, 5]))
            if step > pre_train_steps:
                if e > end_e:
                    e -= step_drop_e
                if step % update_freq == 0:
                    train_batch = experience_buffer.sample(batch_size)
                    inputs = np.reshape(np.vstack(train_batch[:, 3]),
                                        [-1, 84, 84, 3])
                    A = sess.run(mainQN.predict, feed_dict={
                        mainQN.input_x: inputs
                    })
                    Q = sess.run(targetQN.Qout, feed_dict={
                        targetQN.input_x: inputs
                    })
                    doubleQ = Q[range(batch_size), A]
                    targetQ = train_batch[:, 2] + y * doubleQ

                    inputs = np.reshape(np.vstack(train_batch[:, 0]),
                                        [-1, 84, 84, 3])
                    _, loss_val = sess.run([mainQN.update_model, mainQN.loss], feed_dict={
                        mainQN.input_x: inputs,
                        mainQN.actions: train_batch[:, 1],
                        mainQN.targetQ: targetQ})
                    update_target(targetOps, sess)
            s = s1
            if d is True:
                s = env.reset()
            if step > 0 and step % 25 == 0:
                print("step: {}, average reward of last 25 episodes {}, e: {}".format(step, np.mean(rList), e))
                rList = []
            if step % 1000 == 0:
                model_path = "{}/model-{}.ckpt".format(path, step)
                print("===> Step: {}, Saving mode to {}".format(step, model_path))
                saver.save(sess=sess, save_path=model_path)
            if step > 0 and step % 10000 == 0:
                export_graph(sess, path, target_nodes="Qout")
        export_graph(sess, path, target_nodes="Qout")
        print("Training finished.")