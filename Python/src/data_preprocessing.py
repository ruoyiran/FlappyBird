'''
@version: 1.0
@author: royran
@contact: iranpeng@gmail.com
@file: data_preprocessing.py
@time: 2018/3/3 20:19
'''

import tensorflow as tf
from game_config import GameConfig

class DataPrerocessing(object):
    def __init__(self):
        with tf.name_scope("ImagePreporcess"):
            self.input_x = tf.placeholder(dtype=tf.int32, shape=(None, GameConfig.image_height * GameConfig.image_width * GameConfig.n_channels))
            x_reshape = tf.reshape(self.input_x, shape=(-1, GameConfig.image_height, GameConfig.image_width, GameConfig.n_channels))
            # x_gray = tf.image.rgb_to_grayscale(x_reshape)
            x = tf.image.resize_bilinear(x_reshape, (GameConfig.target_size, GameConfig.target_size))
            self.x = tf.div(tf.cast(x, tf.float32), 255.0)

    def run(self, session, data):
        return session.run(self.x, feed_dict={self.input_x: [data]})[0]