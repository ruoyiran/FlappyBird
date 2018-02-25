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

def process_pixels(image_bytes=None):
    s = bytearray(image_bytes)
    image = Image.open(io.BytesIO(s))
    return np.array(image)

def normalize(image):
    return image/255.0