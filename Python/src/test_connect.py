'''
@version: 1.0
@author: royran
@contact: iranpeng@gmail.com
@file: test_connect.py
@time: 2018/2/28 18:15
'''
import numpy as np
from game_config import GameConfig
from unity_environment import UnityEnvironment
from data_preprocessing import DataPrerocessing

import tensorflow as tf
import image_utils

GameConfig.target_size = 80

def rgb2gray(r, g, b):
    return int((299*r + 587*g + 114*b)/1000.0)

dp = DataPrerocessing()
env = UnityEnvironment()
s = env.reset()
image_data = np.array(list(s))
print(image_data.shape, image_data.dtype)
image_channel_r = image_data[:GameConfig.image_height*GameConfig.image_width]
# gray = image_channel_r.reshape((GameConfig.image_width, GameConfig.image_height))
gray = np.zeros((GameConfig.image_height, GameConfig.image_width), np.uint8)
binary = np.zeros((GameConfig.image_height, GameConfig.image_width), np.uint8)
for i in range(GameConfig.image_height):
    for j in range(GameConfig.image_width):
        index = j + i * GameConfig.image_width
        r = image_data[3*index]
        g = image_data[3*index + 1]
        b = image_data[3*index + 2]
        gray[i][j] = rgb2gray(r, g, b)
        binary[i][j] = 255 if gray[i][j] > 0 else 0

import image_utils
image_utils.show_image(gray, "gray")
image_utils.show_image(binary, "gray")
image_utils.show_image(image_data.reshape((GameConfig.image_height, GameConfig.image_width, 3)).astype(np.uint8), "gray")
if False:
    with tf.Session() as sess:
        # image = dp.resize_and_normalize(sess, s)
        # image = dp.resize(sess, s)
        # image = dp.resize_and_to_gray(sess, s)
        image = dp.get_state_stacked_image(sess, s)
        image_utils.show_image(image[:,:,0], "gray")

        for i in range(100):
            action = np.random.choice(2)
            s, r, d = env.step(action)
            print(action, r, d)
            image = dp.get_state_stacked_image(sess, s)
            image_utils.show_image(image[:, :, 0], "gray")

            if d:
                print("reset")
                s = env.reset()
                image = dp.get_state_stacked_image(sess, s)
                image_utils.show_image(image[:, :, 0], "gray")

env.close()