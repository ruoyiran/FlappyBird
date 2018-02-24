using System;
using System.Net.Sockets;
using System.Text;

namespace GridWorld.Network
{
    public class Communicator
    {
        private const int MAX_BUF_SIZE = 10240;
        private Socket _connSocket;
        private byte[] _recvBuffer;
        private bool _connected = false;
        public bool IsConnected
        {
            get
            {
                return _connected;
            }
        }

        public Communicator()
        {
            _recvBuffer = new byte[MAX_BUF_SIZE];
        }

        public bool ConnectToServer(string ipAddress, int port)
        {
            if (_connected)
                return true;
            try
            {
                _connSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _connSocket.Connect(ipAddress, port);
                _connected = true;
                Logger.Print("Connected to {0}:{1}", ipAddress, port);
            }
            catch (Exception ex)
            {
                Disconnect();
                Logger.Exception(ex);
            }
            return true;
        }

        public string ReceiveFromServer()
        {
            if (!_connected)
                return "";
            try
            {
                int recvLength = _connSocket.Receive(_recvBuffer);
                string recvData = Encoding.Default.GetString(_recvBuffer, 0, recvLength);
                return recvData;
            }
            catch (SocketException ex)
            {
                Disconnect();
                Logger.Exception(ex);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return "";
        }

        public void Disconnect()
        {
            if (_connSocket != null)
            {
                try
                {
                    _connSocket.Close();
                    Logger.Print("Disconnected.");
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
                _connSocket = null;
            }
            _connected = false;
        }

        public void SendToServer(string message)
        {
            if (!_connected || string.IsNullOrEmpty(message))
                return;
            SendToServer(Encoding.ASCII.GetBytes(message));
        }

        public void SendToServer(byte[] data)
        {
            if (!_connected)
                return;
            try
            {
                _connSocket.Send(data);
            }
            catch (SocketException ex)
            {
                Disconnect();
                Logger.Exception(ex);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}