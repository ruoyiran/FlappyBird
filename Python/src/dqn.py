"""
@version: 1.0
@author: Roy
@contact: iranpeng@gmail.com
@file: dqn.py
@time: 2018/2/26 2:09
"""

from unity_environment import UnityEnvironment
from image_utils import show_image
import numpy as np

BIRD_ACTION_IDLE = 0
BIRD_ACTION_FLAP = 1
bird_actions = [BIRD_ACTION_IDLE, BIRD_ACTION_FLAP]
env = UnityEnvironment(address="127.0.0.1", port=8008)

state = env.reset()

for i in range(100):
    action = np.random.choice(bird_actions)
    state, reward, is_done = env.step(action)
    print(reward, is_done)
    show_image(state)

env.close()