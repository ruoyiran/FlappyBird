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

dp = DataPrerocessing()
env = UnityEnvironment()
s = env.reset()

with tf.Session() as sess:
    # image = dp.resize_and_normalize(sess, s)
    # image = dp.resize(sess, s)
    # image = dp.resize_and_to_gray(sess, s)
    image = dp.resize_and_threshold(sess, s)
    image_utils.show_image(image, "gray")

    for i in range(100):
        action = np.random.choice(2)
        s, r, d = env.step(action)
        print(action, r, d)
        image = dp.resize_and_threshold(sess, s)
        image_utils.show_image(image, "gray")
        if d:
            print("reset")
            s = env.reset()
            image = dp.resize_and_threshold(sess, s)
            image_utils.show_image(image, "gray")


env.close()