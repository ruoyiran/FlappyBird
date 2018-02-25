"""
@version: 1.0
@author: Roy
@contact: iranpeng@gmail.com
@file: dqn.py
@time: 2018/2/26 2:09
"""
import matplotlib.pyplot as plt
from unity_environment import UnityEnvironment
env = UnityEnvironment(address="127.0.0.1", port=8008)

state = env.reset()
# plt.imshow(state)
# plt.show()
env.close()