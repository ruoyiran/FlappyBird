'''
@version: 1.0
@author: royran
@contact: iranpeng@gmail.com
@file: export_graph.py
@time: 2018/3/6 18:04
'''

import tensorflow as tf
from double_dueling_dqn import DeepQNetwork
from game_config import GameConfig
from tensorflow.python.tools import freeze_graph

def export_graph(sess, model_dir, target_nodes):
    print("Exporting graph...")
    tf.train.write_graph(sess.graph_def, model_dir, 'raw_graph_def.pb', as_text=False)
    ckpt = tf.train.get_checkpoint_state(model_dir)
    output_graph = model_dir + "/graph_def.bytes"
    freeze_graph.freeze_graph(input_graph=model_dir + '/raw_graph_def.pb',
                              input_binary=True,
                              input_checkpoint=ckpt.model_checkpoint_path,
                              output_node_names=target_nodes,
                              output_graph=output_graph,
                              clear_devices=True, initializer_nodes="", input_saver="",
                              restore_op_name="save/restore_all", filename_tensor_name="save/Const:0")

def export_image_preprocessing_graph():
    with tf.name_scope("Input"):
        input_x = tf.placeholder(dtype=tf.uint8, shape=(None, GameConfig.image_height * GameConfig.image_width * 3),
                                 name="input_x")
    with tf.name_scope("Reshape"):
        x_reshape = tf.reshape(input_x, shape=(-1, GameConfig.image_height, GameConfig.image_width, 3),
                               name="reshaped_x")
        x_reshape = tf.cast(x_reshape, tf.uint8)
    with tf.name_scope("RGB2Gray"):
        x_gray = tf.image.rgb_to_grayscale(x_reshape, name="gray_x")
    with tf.name_scope("Threshold"):
        x_threshold = tf.where(x_gray > 1, 255 * tf.ones_like(x_gray), tf.zeros_like(x_gray),
                               name="threshold_x")
    with tf.name_scope("Target"):
        resized_x = tf.image.resize_area(x_threshold, (GameConfig.target_size, GameConfig.target_size),
                                             name="resized_x")
        resized_x = tf.squeeze(resized_x, [3])
        stacked_x = tf.stack((resized_x, resized_x, resized_x, resized_x), axis=-1, name="stacked_x")

    with tf.name_scope("NextTarget"):
        input_stacked_x = tf.placeholder(dtype=tf.float32,
                                         shape=(None, GameConfig.target_size, GameConfig.target_size, 4),
                                         name="input_stacked_x")
        next_stacked_x = tf.stack((resized_x, input_stacked_x[:, :, :, 0], input_stacked_x[:, :, :, 1], input_stacked_x[:, :, :, 2]),
                                  axis=-1,
                                  name="output_stacked_x")

    tf.train.write_graph(tf.get_default_graph(), "../graphs", 'image_preprocess_graph_def.pb', as_text=False)
    return input_x, stacked_x

def export_training_model():
    GameConfig.target_size = 84
    GameConfig.n_channels = 4

    tf.reset_default_graph()
    h_size = 512
    learning_rate = 0.0001
    n_actions = 2
    with tf.variable_scope("MainQNetwork"):
        mainQN = DeepQNetwork(h_size, n_actions, learning_rate)

    model_path = "../models"
    with tf.Session() as sess:
        if False:
            writer = tf.summary.FileWriter("../log", graph=sess.graph)
            writer.close()
        export_graph(sess, model_path, target_nodes="MainQNetwork/Qout/QValue")

def test_tfgraph():
    from PIL import Image
    import numpy as np
    import matplotlib.pyplot as plt
    image_path = r"C:\Users\royran\AppData\Local\Temp\Tencent\FlappyBird\Capture\Image_0000.png"
    image = Image.open(image_path)
    data = np.asarray(image)
    data = data.flatten()
    input_x, stacked_x = export_training_model()
    with tf.Session() as sess:
        writer = tf.summary.FileWriter("../log2", graph=sess.graph)
        writer.close()
        stacked_image = sess.run(stacked_x, feed_dict={input_x: [data]})[0]
        print(stacked_image.shape)
        plt.figure()
        plt.imshow(np.squeeze(stacked_image[:, :, 0]), cmap="gray")
        plt.figure()
        plt.imshow(np.squeeze(stacked_image[:, :, 1]), cmap="gray")
        plt.figure()

        plt.imshow(np.squeeze(stacked_image[:, :, 2]), cmap="gray")
        plt.figure()

        plt.imshow(np.squeeze(stacked_image[:, :, 3]), cmap="gray")
        plt.show()

if __name__ == '__main__':
    export_training_model()
    # export_image_preprocessing_graph()


