'''
@version: 1.0
@author: royran
@contact: iranpeng@gmail.com
@file: unity_environment.py
@time: 2018/1/2 12:12
'''
import atexit
import socket
import struct
import json
from image_utils import process_pixels, normalize
CMD_QUIT  = "QUIT"
CMD_STEP  = "STEP"
CMD_RESET = "RESET"

class UnityEnvironment(object):
    def __init__(self, address="127.0.0.1", port=8008):
        atexit.register(self.close)
        self._socket = None
        self._buffer_size = 10240
        self.port = port
        self._create_socket_server(address, self.port)
        self._listening()

    def _create_socket_server(self, address, port):
        try:
            self._socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self._socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            self._socket.bind((address, port))
        except socket.error:
            self.close()
            raise socket.error("Couldn't launch new environment because worker number {} is still in use. "
                               "You may need to manually close a previously opened environment "
                               "or use a different worker number.".format(self.port))

    def _listening(self):
        try:
            print("Waiting connect...")
            self._socket.listen(1)
            self._conn, _ = self._socket.accept()
        except socket.timeout as e:
            raise socket.error(e.strerror)

    def _recv_bytes(self):
        try:
            data = self._conn.recv(self._buffer_size)
        except Exception as ex:
            raise ex
        return data

    def _recv_bytes_except_header(self):
        s = self._recv_bytes()
        data_length = struct.unpack("I", bytearray(s[:4]))[0]
        s = s[4:]
        while len(s) != data_length:
            s += self._recv_bytes()
        return s

    def _recv(self):
        data = self._recv_bytes()
        if data is None:
            return None
        return data.decode('utf-8')

    def _send_bytes(self, bytes_data):
        if bytes_data is None:
            return
        try:
            self._conn.send(bytes_data)
        except Exception as ex:
            raise ex

    def _send(self, msg):
        if msg is None:
            return
        self._send_bytes(msg.encode('utf-8'))

    def reset(self):
        self._send(CMD_RESET)
        image = self._recv_state_image()
        image = normalize(image)
        return image

    def step(self, action):
        action = int(action)
        self._send(CMD_STEP)
        self._recv_bytes()
        self._send_action(action)
        reward, is_done = self._recv_step_json_data()
        image = self._recv_state_image()
        image = normalize(image)
        return image, reward, is_done

    def _recv_step_json_data(self):
        json_data = self._recv_bytes_except_header()
        step_msg = json.loads(json_data.decode('utf-8'))
        reward = step_msg["Reward"]
        is_done = step_msg["IsDone"]
        return reward, is_done

    def _recv_state_image(self):
        image_data = self._recv_bytes_except_header()
        img = process_pixels(image_data)
        return img

    def close(self):
        if self._socket is not None:
            print("Closing...")
            self._send(CMD_QUIT)
            self._socket.close()
            self._socket = None

    def _send_action(self, action):
        action_message = {"Action": action}
        self._conn.send(json.dumps(action_message).encode('utf-8'))
