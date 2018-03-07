"""
@version: 1.0
@author: Roy
@contact: iranpeng@gmail.com
@file: double_dueling_dqn.py.py
@time: 2018/1/6
"""
import tensorflow as tf
from game_config import GameConfig

class DeepQNetwork(object):

    def __init__(self, h_size=512, n_actions=4, learning_rate=1e-4):
        with tf.name_scope("Input"):
            x = tf.placeholder(tf.float32, shape=[None, GameConfig.target_size, GameConfig.target_size,  GameConfig.n_channels], name="x")

        conv1 = tf.layers.conv2d(x, filters=32, kernel_size=8, strides=4,
                                 padding='valid', activation=tf.nn.relu, name="Conv1")
        conv2 = tf.layers.conv2d(conv1, filters=64, kernel_size=4, strides=2,
                                 padding='valid', activation=tf.nn.relu, name="Conv2")
        conv3 = tf.layers.conv2d(conv2, filters=64, kernel_size=3, strides=1,
                                 padding='valid', activation=tf.nn.relu, name="Conv3")
        conv4 = tf.layers.conv2d(conv3, filters=h_size, kernel_size=7, strides=1,
                                 padding='valid', activation=tf.nn.relu, name="Conv4")

        # We take the output from the final convolutional layer and split it into separate advantage and value streams.
        streamAC, streamVC = tf.split(conv4, 2, 3)
        streamA = tf.layers.flatten(streamAC, "streamA")
        streamV = tf.layers.flatten(streamVC, name="streamV")
        Advantage = tf.layers.dense(streamA, units=n_actions, name="Advantage")
        Value = tf.layers.dense(streamV, units=1, name="Value")

        with tf.variable_scope("Qout"):
            Qout = tf.add(Value, tf.subtract(Advantage, tf.reduce_mean(Advantage, axis=1, keep_dims=True)), name="QValue")
            predict = tf.argmax(Qout, 1, name="Predict")

        with tf.variable_scope("TargetQ"):
            targetQ = tf.placeholder(shape=[None], dtype=tf.float32)
        with tf.variable_scope("Action"):
            actions = tf.placeholder(shape=[None], dtype=tf.int32)
        with tf.variable_scope("Q"):
            actions_onehot = tf.one_hot(actions, n_actions, dtype=tf.float32)
            Q = tf.reduce_sum(tf.multiply(Qout, actions_onehot), axis=1)
        with tf.variable_scope("TD_Error"):
            td_error = tf.square(targetQ - Q)
        with tf.variable_scope("Loss"):
            loss = tf.reduce_mean(td_error)

        lr = tf.Variable(learning_rate, name="learning_rate")
        global_step = tf.Variable(0, trainable=False)
        with tf.variable_scope("Trainer"):
            trainer = tf.train.AdamOptimizer(learning_rate=lr)
        update_model = trainer.minimize(loss, global_step=global_step)

        print("x", x.get_shape())
        print("conv1", conv1.get_shape())
        print("conv2", conv2.get_shape())
        print("conv3", conv3.get_shape())
        print("conv4", conv4.get_shape())
        print("streamAC", streamAC.get_shape())
        print("streamVC", streamVC.get_shape())
        print("streamA", streamA.get_shape())
        print("streamV", streamV.get_shape())
        print("Advantage", Advantage.get_shape())
        print("Value", Value.get_shape())
        print()

        self.input_x = x
        self.predict = predict
        self.loss = loss
        self.Qout = Qout
        self.targetQ = targetQ
        self.actions = actions
        self.update_model = update_model
        self.lr = lr
        self.global_step = global_step
