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
data = np.array(list(s), dtype=np.int32)
# show_image(s)
image_width = 683
image_height = 768
n_channels = 3
input_size = 84

with tf.name_scope("Input"):
    input_x = tf.placeholder(dtype=tf.int32, shape=(None, image_height * image_width * n_channels))
with tf.name_scope("ImagePreporcess"):
    x_reshape = tf.reshape(input_x, shape=(-1, image_height, image_width, n_channels))
    x_gray = tf.image.rgb_to_grayscale(x_reshape)
    x = tf.image.resize_bilinear(x_gray, (input_size, input_size))
    x = tf.div(tf.cast(x, tf.float32), 255.0)

with tf.Session() as sess:
    gray_images = sess.run(x, feed_dict={input_x: [data]})
    image = gray_images[0]
    image_utils.show_image(image, "gray")

for i in range(0):
    s, r, d = env.step(np.random.choice(2))
    # show_image(s)

env.close()