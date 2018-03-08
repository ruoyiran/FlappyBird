"""
@version: 1.0
@author: Roy
@contact: iranpeng@gmail.com
@file: game_playing.py
@time: 2018/3/8 20:11
"""
"""
@version: 1.0
@author: Roy
@contact: iranpeng@gmail.com
@file: dueling_dqn_training.py.py
@time: 2018/1/7
"""

import tensorflow as tf
from unity_environment import UnityEnvironment
from dueling_dqn_network import DeepQNetwork
from data_preprocessing import DataPrerocessing
from game_config import GameConfig

if __name__ == "__main__":
    GameConfig.target_size = 84
    GameConfig.n_channels = 4
    tf.reset_default_graph()
    h_size = 512
    learning_rate = 1e-4
    decay_steps = 300000
    n_actions = 2
    with tf.variable_scope("MainQNetwork"):
        mainQN = DeepQNetwork(h_size, n_actions, learning_rate)
    y = 0.99
    saver = tf.train.Saver()
    model_dir = "../models"  # The model_dir to save our model to.
    dp = DataPrerocessing()
    env = UnityEnvironment()
    with tf.Session() as sess:
        checkpoint = tf.train.get_checkpoint_state(model_dir)
        if checkpoint and checkpoint.model_checkpoint_path:
            saver.restore(sess, checkpoint.model_checkpoint_path)
            print("Successfully loaded:", checkpoint.model_checkpoint_path)
        else:
            print("Could not find old network weights")
        s = env.reset()
        s_t = dp.get_state_stacked_image(sess, s)

        rList = []
        while True:
            a = sess.run(mainQN.predict, feed_dict={mainQN.input_x: [s_t]})[0]
            qvalues = sess.run(mainQN.Qout, feed_dict={mainQN.input_x: [s_t]})[0]
            s1, r, d = env.step(a)
            if d:
                s1 = env.reset()
            prev_reward = r
            s_t1 = dp.get_state_stacked_image(sess, s1, s_t)
            s_t = s_t1
        print("Training finished.")
    env.close()
