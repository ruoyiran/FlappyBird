"""
@version: 1.0
@author: Roy
@contact: iranpeng@gmail.com
@file: dqn.py
@time: 2018/2/26 2:09
"""
import matplotlib.pyplot as plt
from unity_environment import UnityEnvironment
import numpy as np

BIRD_ACTION_IDLE = 0
BIRD_ACTION_FLAP = 1
bird_actions = [BIRD_ACTION_IDLE, BIRD_ACTION_FLAP]
env = UnityEnvironment(address="127.0.0.1", port=8008)

state = env.reset()
for i in range(100):
    action = np.random.choice(bird_actions)
    env.step(action)
#
# plt.imshow(state)
# plt.show()
env.close()