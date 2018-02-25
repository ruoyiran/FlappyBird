using System;
using System.Collections.Generic;
using Network;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

public class EnvironmentObserver : SingletonMono<EnvironmentObserver> {
    private string _ipAddress = "127.0.0.1";
    private int _port = 8008;
    private static Communicator _communicator;
    private Dictionary<string, Command> _commands;
    private BirdObserver _birdObserver;
    private int _envImageWidth;
    private int _envImageHeight;

    void Start () {
        _envImageWidth = Screen.width;
        _envImageHeight = Screen.height;
        _commands = new Dictionary<string, Command>();
        _birdObserver = new BirdObserver();
        InitIpAddrAndPort();
	}

    private void LateUpdate()
    {
        if (_communicator == null ||
            !_communicator.IsConnected)
            return;
        Command cmd = GetCommand();
        DoAction(cmd);
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

    private void ConnectToNetwork()
    {
        if (_communicator == null)
            _communicator = new Communicator();
        if (!_communicator.IsConnected)
            _communicator.ConnectToServer(_ipAddress, _port);
    }

    private void DisconnectNetwork()
    {
        if (_communicator != null && _communicator.IsConnected)
            _communicator.Disconnect();
    }


    public void StartMonitoring()
    {
        ConnectToNetwork();
    }

    public void StopMonitoring()
    {
        DisconnectNetwork();
    }

    private Command GetCommand()
    {
        string data = _communicator.ReceiveFromServer();
        if (data == Command.RESET.ToString())
            return Command.RESET;
        else if (data == Command.STEP.ToString())
            return Command.STEP;
        else if (data == Command.QUIT.ToString())
            return Command.QUIT;
        return Command.UNKNOWN;
    }

    private void DoAction(Command cmd)
    {
        switch (cmd)
        {
            case Command.RESET:
                Reset();
                break;
            case Command.STEP:
                Step();
                break;
            case Command.QUIT:
                DisconnectNetwork();
                break;
            default:
                break;
        }
    }

    private void Reset()
    {
        SendDataBytesToServer(GetEnvironmentImageBytes());
    }

    private void Step()
    {
        NotifyServerDataReceived();
        string jsonData = _communicator.ReceiveFromServer();
        AgentMessage message = JsonConvert.DeserializeObject<AgentMessage>(jsonData);
        SendDataBytesToServer(GetEnvironmentImageBytes());
    }

    private void NotifyServerDataReceived()
    {
        _communicator.SendToServer("Received");
    }

    private void SendDataBytesToServer(byte[] data)
    {
        byte[] bytes = Algorithm.AppendLength(data);
        _communicator.SendToServer(bytes);
    }

    public byte[] GetEnvironmentImageBytes()
    {
        Texture2D tex = ImageTool.RenderToTex(Camera.main, _envImageWidth, _envImageHeight);
        byte[] imageBytes = tex.EncodeToPNG();
        DestroyImmediate(tex);
        Resources.UnloadUnusedAssets();
        return imageBytes;
    }
}
