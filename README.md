# Flappy Bird Game with Deep Reinforment Learning
![GameScreenshot](https://github.com/Ruoyiran/FlappyBird/blob/master/Images/GameScreenshot.png)

## Automatic Game Play Video
[![IMAGE ALT TEXT](http://img.youtube.com/vi/Ag6F9ubaMxo/0.jpg)](http://www.youtube.com/watch?v=Ag6F9ubaMxo)  

**Summary:** This project follows the description of the Deep Q Learning algorithm described in [Dueling Network Architectures for Deep Reinforcement Learning](https://arxiv.org/abs/1511.06581), and generalized to the notorious Flappy Bird game.

![Network Description](https://github.com/Ruoyiran/FlappyBird/blob/master/Images/Network%20Description.png)


## TensorFlow Network Structure Graph
![Network Structure](https://github.com/Ruoyiran/FlappyBird/blob/master/Images/TF%20Network%20Graph.png)

## Requirements
* Python3
* TensorFlow 1.5
* Unity 2017.3 or above
* Unity Tensorflow Plugin [(Download here)](https://s3.amazonaws.com/unity-agents/0.2/TFSharpPlugin.unitypackage)

## Note
Please download [Unity Tensorflow Plugin](https://s3.amazonaws.com/unity-agents/0.2/TFSharpPlugin.unitypackage) firstly, open the Unity project and import the plugin you downloaded, and then Go to Edit -> Player Settings and add **ENABLE_TENSORFLOW** to the Scripting Define Symbols for each type of device you want to use (PC, Mac and Linux Standalone, iOS or Android).
