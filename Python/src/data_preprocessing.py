'''
@version: 1.0
@author: royran
@contact: iranpeng@gmail.com
@file: data_preprocessing.py
@time: 2018/3/3 20:19
'''

import tensorflow as tf
import cv2
import numpy as np
from game_config import GameConfig

class DataPrerocessing(object):
    def __init__(self):
        with tf.name_scope("Input"):
            self.input_x = tf.placeholder(dtype=tf.uint8, shape=(None, GameConfig.image_height * GameConfig.image_width * 3),
                                     name="input_x")
        with tf.name_scope("Reshape"):
            x_reshape = tf.reshape(self.input_x, shape=(-1, GameConfig.image_height, GameConfig.image_width, 3),
                                   name="reshaped_x")
            x_reshape = tf.cast(x_reshape, tf.uint8)
        with tf.name_scope("RGB2Gray"):
            x_gray = tf.image.rgb_to_grayscale(x_reshape, name="gray_x")
            x_gray = tf.cast(x_gray, tf.float32)
        with tf.name_scope("Threshold"):
            x_threshold = tf.where(x_gray > 1, 255 * tf.ones_like(x_gray), tf.zeros_like(x_gray),
                                   name="threshold_x")
        with tf.name_scope("Target"):
            resized_x = tf.image.resize_images(x_threshold, (GameConfig.target_size, GameConfig.target_size),
                                               method=tf.image.ResizeMethod.AREA)
            resized_x = tf.squeeze(resized_x, [3])
            self.stacked_x = tf.stack((resized_x, resized_x, resized_x, resized_x), axis=-1, name="stacked_x")

        with tf.name_scope("NextTarget"):
            self.input_stacked_x = tf.placeholder(dtype=tf.float32,
                                             shape=(None, GameConfig.target_size, GameConfig.target_size, 4),
                                             name="input_stacked_x")
            self.next_stacked_x = tf.stack(
                (resized_x, self.input_stacked_x[:, :, :, 0], self.input_stacked_x[:, :, :, 1], self.input_stacked_x[:, :, :, 2]),
                axis=-1,
                name="output_stacked_x")

    def get_state_stacked_image(self, session, data, stacked_x=None):
        if stacked_x is None:
            return session.run(self.stacked_x, feed_dict={self.input_x: [data]})[0]
        else:
            return session.run(self.next_stacked_x,
                               feed_dict={
                                   self.input_x: [data],
                                   self.input_stacked_x: [stacked_x]})[0]

