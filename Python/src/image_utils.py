"""
@version: 1.0
@author: Roy
@contact: iranpeng@gmail.com
@file: image_utils.py
@time: 2018/1/4
"""
import io
from PIL import Image
import numpy as np
import matplotlib.pyplot as plt
import cv2 as cv

def process_pixels(image_bytes=None):
    s = bytearray(image_bytes)
    # image = Image.open(io.BytesIO(s))
    return np.array(list(s))

def normalize(image):
    return image/255.0

def show_image(image, cmap=None):
    plt.imshow(np.squeeze(image), cmap=cmap)
    plt.show()