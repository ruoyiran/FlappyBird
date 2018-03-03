using System;
using System.Collections;
using System.Text;
using Network;
using Newtonsoft.Json;

namespace FlappyBird
{
    public class NetworkPlayMode : BasePlayMode, IPlayMode
    {
        private delegate void CaptureFrameDoneHandler(string imagePath);
        private delegate void CaptureFrameDataDoneHandler(byte[] imageData);

        private Communicator _communicator;
        private string _ipAddress = "127.0.0.1";
        private int _port = 8008;

        private void Start()
        {
            Logger.Print("NetworkPlayMode.Start");
            InitIpAddrAndPort();
        }

        private new void Update()
        {
            base.Update();
        }

        private void LateUpdate()
        {
            if (_isPlaying && NetworkIsConnected())
            {
                GameCommand cmd = GetCommand();
                DoCommand(cmd);
            }
        }

        private void OnDestroy()
        {
            DisconnectNetwork();
        }

        public new void Play()
        {
            base.Play();
            ConnectToNetwork();
        }

        public new void Stop()
        {
            base.Stop();
        }

        private void ConnectToNetwork()
        {
            if (_communicator == null)
                _communicator = new Communicator();
            if (!_communicator.IsConnected)
                _communicator.ConnectToServer(_ipAddress, _port);
        }

        private void DisconnectNetwork()
        {
            if (_communicator != null &&
                _communicator.IsConnected)
                _communicator.Disconnect();
        }

        private bool NetworkIsConnected()
        {
            if (_communicator == null)
                return false;
            return _communicator.IsConnected;
        }

        private void InitIpAddrAndPort()
        {
            string ipAddr = DevSwitch.GetDevSwitchValue("IpAddr");
            if (!string.IsNullOrEmpty(ipAddr))
                _ipAddress = ipAddr;
            string strPort = DevSwitch.GetDevSwitchValue("Port");
            if (!string.IsNullOrEmpty(strPort))
            {
                int nPort = 0;
                if (int.TryParse(strPort, out nPort))
                    _port = nPort;
            }
        }

        private GameCommand GetCommand()
        {
            string data = _communicator.ReceiveFromServer();
            if (data == GameCommand.RESET.ToString())
                return GameCommand.RESET;
            else if (data == GameCommand.STEP.ToString())
                return GameCommand.STEP;
            else if (data == GameCommand.QUIT.ToString())
                return GameCommand.QUIT;
            return GameCommand.UNKNOWN;
        }

        private void DoCommand(GameCommand cmd)
        {
            switch (cmd)
            {
                case GameCommand.RESET:
                    Reset();
                    break;
                case GameCommand.STEP:
                    Step();
                    break;
                case GameCommand.QUIT:
                    DisconnectNetwork();
                    break;
                default:
                    break;
            }
        }

        private void Reset()
        {
            GameManager.Instance.ResetGame();
            StartCoroutine(CaptureFrame((imageData) => {
                SendDataBytesToServer(Algorithm.AppendLength(imageData));
            }));
        }

        private void Step()
        {
            NotifyServerDataReceived();
            ExecStepAction();
            StartCoroutine(CaptureFrame((imageData) => { SendEnvronmentStateToServer(imageData); }));
        }

        private IEnumerator CaptureFrame(CaptureFrameDataDoneHandler doneHandler)
        {
            GameManager.Instance.frameRecorder.BeginRecording();
            while (!GameManager.Instance.frameRecorder.IsCaptured)
            {
                yield return null;
            }
            byte[] imageData = GameManager.Instance.frameRecorder.GetFrameImageData();
            if (doneHandler != null)
                doneHandler(imageData);
            GameManager.Instance.frameRecorder.EndRecording();
        }

        private void ExecStepAction()
        {
            string jsonData = _communicator.ReceiveFromServer();
            AgentMessage message = JsonConvert.DeserializeObject<AgentMessage>(jsonData);
            Bird.Action action = (Bird.Action)message.Action;
            GameManager.Instance.bird.Flap(action);
        }

        private void SendEnvronmentStateToServer(byte[] imageData)
        {
            AgentStepMessage stepMsg = new AgentStepMessage();
            stepMsg.IsDone = GameManager.Instance.bird.IsDead;
            stepMsg.Reward = GetReward();
            string stepData = JsonConvert.SerializeObject(stepMsg, Formatting.Indented);
            SendDataBytesToServer(Algorithm.AppendLength(Encoding.ASCII.GetBytes(stepData)));
            SendDataBytesToServer(Algorithm.AppendLength(imageData));
        }

        private float GetReward()
        {
            if (GameManager.Instance.bird.IsDead)
                return -10.0f;
            if (GameManager.Instance.bird.BetweenInColums)
                return 10.0f;
            return 0.1f;
        }

        private void SendDataBytesToServer(byte[] data)
        {
            _communicator.SendToServer(data);
        }

        private void NotifyServerDataReceived()
        {
            _communicator.SendToServer("Received");
        }
    }
}