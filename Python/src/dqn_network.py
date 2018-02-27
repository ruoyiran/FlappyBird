"""
@version: 1.0
@author: Roy
@contact: iranpeng@gmail.com
@file: dqn_network.py
@time: 2018/1/6
"""
import tflearn
import tensorflow as tf

class DeepQNetwork(object):

    def __init__(self, h_size=512, n_actions=4, learning_rate=1e-4):
        input_x = tf.placeholder(dtype=tf.float32, shape=[None, 84, 84, 3], name="Input")
        conv1 = tflearn.conv_2d(input_x, nb_filter=32, filter_size=8, strides=4,
                                padding='VALID', weights_init='xavier',
                                activation='relu', name="Conv1")
        conv2 = tflearn.conv_2d(conv1, nb_filter=64, filter_size=4, strides=2,
                                padding='VALID', weights_init='xavier',
                                activation='relu', name="Conv2")
        conv3 = tflearn.conv_2d(conv2, nb_filter=64, filter_size=3, strides=1,
                                padding='VALID', weights_init='xavier',
                                activation='relu', name="Conv3")
        conv4 = tflearn.conv_2d(conv3, nb_filter=h_size, filter_size=7, strides=1,
                                padding='VALID', weights_init='xavier',
                                activation='relu', name="Conv4")

        # We take the output from the final convolutional layer and split it into separate advantage and value streams.
        streamAC, streamVC = tf.split(conv4, 2, 3)
        streamA = tflearn.flatten(streamAC, name="streamA")
        streamV = tflearn.flatten(streamVC, name="streamV")

        Advantage = tflearn.fully_connected(streamA, n_units=n_actions, activation='linear',
                                            weights_init='xavier',  name="Advantage")
        Value = tflearn.fully_connected(streamV, n_units=1, activation='linear',
                                        weights_init='xavier', name="Value")
        Qout = tf.add(Value, tf.subtract(Advantage, tf.reduce_mean(Advantage, axis=1, keep_dims=True)), name="Qout")
        predict = tf.argmax(Qout, 1, name="Predict")

        targetQ = tf.placeholder(shape=[None], dtype=tf.float32)
        actions = tf.placeholder(shape=[None], dtype=tf.int32)
        actions_onehot = tf.one_hot(actions, n_actions, dtype=tf.float32)
        Q = tf.reduce_sum(tf.multiply(Qout, actions_onehot), axis=1)

        td_error = tf.square(targetQ - Q)
        loss = tf.reduce_mean(td_error)
        trainer = tf.train.AdamOptimizer(learning_rate=learning_rate)
        update_model = trainer.minimize(loss)

        self.input_x = input_x
        self.predict = predict
        self.loss = loss
        self.Qout = Qout
        self.targetQ = targetQ
        self.actions = actions
        self.update_model = update_model
