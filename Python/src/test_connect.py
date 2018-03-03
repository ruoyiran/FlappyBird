'''
@version: 1.0
@author: royran
@contact: iranpeng@gmail.com
@file: test_connect.py
@time: 2018/2/28 18:15
'''
import numpy as np
from unity_environment import UnityEnvironment
from PIL import Image
from data_preprocessing import DataPrerocessing

import tensorflow as tf
import image_utils
def show_image(data_bytes):
    data = np.asarray(list(data_bytes), dtype=np.uint8)
    data = np.reshape(data, (768, 1366//2, 3))
    img_3 = Image.fromarray(data)
    img_3.show()

def preprocess_data(data):
    data = np.array(list(data), dtype=np.float32)
    data /= 255.0
    return data

env = UnityEnvironment()
s = env.reset()
dp = DataPrerocessing()

with tf.Session() as sess:
    image = dp.run(sess, s)
    image_utils.show_image(image, "gray")

    for i in range(1):
        s, r, d = env.step(np.random.choice(2))
        image = dp.run(sess, s)
        image_utils.show_image(image, "gray")

env.close()