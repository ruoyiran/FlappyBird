'''
@version: 1.0
@author: royran
@contact: iranpeng@gmail.com
@file: test_connect.py
@time: 2018/2/28 18:15
'''
import numpy as np
from unity_environment import UnityEnvironment
from data_preprocessing import DataPrerocessing

import tensorflow as tf
import image_utils


env = UnityEnvironment()
s = env.reset()
dp = DataPrerocessing()

with tf.Session() as sess:
    image = dp.run(sess, s)
    image_utils.show_image(image, "gray")

    for i in range(100):
        action = np.random.choice(2)
        s, r, d = env.step(action)
        print(action, r, d)
        image = dp.run(sess, s)
        image_utils.show_image(image, "gray")
        if d:
            print("reset")
            s = env.reset()
            image = dp.run(sess, s)
            image_utils.show_image(image, "gray")


env.close()