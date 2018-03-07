using System;
using UnityEngine;
using UTJ.FrameCapturer;

namespace FlappyBird
{
    public enum GameState
    {
        Ready,
        Playing,
        GameOver,
    }

    public enum PlayingMode
    {
        Free = 0,
        AI = 1,
        Network = 2,
    }

    public enum SpeedMode
    {
        Easy = 0,
        Middle = 1,
        Hard = 2,
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get
            {
                return _instance;
            }
        }
        private static GameManager _instance;

        public Bird bird;
        public Environment enviroment;
        public FrameRecorder frameRecorder;
        public GameState CurrentGameState { get { return _gameState; } }

        private IPlayMode _playMode;
        private PlayingMode _currPlayingMode = PlayingMode.Free;
        private GameState _gameState = GameState.Ready;

        private void Awake()
        {
#if UNITY_ANDROID && ENABLE_TENSORFLOW
            Logger.Print("GameManager.Awake - TensorFlowSharp.Android.NativeBinding.Init called.");
            TensorFlowSharp.Android.NativeBinding.Init();
#endif
            _instance = this;
            Application.runInBackground = true;
        }

        private void Start()
        {
            _playMode = GetComponent<FreePlayMode>();
            if (_playMode == null)
                _playMode = gameObject.AddComponent<FreePlayMode>();
            bird.SetIsSimulated(false);
        }

        private void Update()
        {
            if (bird.IsDead)
            {
                if(_currPlayingMode != PlayingMode.Network)
                {
                    _gameState = GameState.GameOver;
                    StopCurrentPlayingMode();
                }
            }
        }

        private void StopCurrentPlayingMode()
        {
            if (_playMode != null)
                _playMode.Stop();
        }

        public void StartGame(PlayingMode mode)
        {
            _currPlayingMode = mode;
            StopCurrentPlayingMode();
            _playMode = GetPlayMode(mode);
            PlayGame();
        }

        private IPlayMode GetPlayMode(PlayingMode mode)
        {
            IPlayMode playMode = null;
            switch (mode)
            {
                case PlayingMode.Free:
                    playMode = GetComponent<FreePlayMode>();
                    if (playMode == null)
                        playMode = gameObject.AddComponent<FreePlayMode>();
                    break;
                case PlayingMode.AI:
                    playMode = GetComponent<AIPlayMode>();
                    if (playMode == null)
                        playMode = gameObject.AddComponent<AIPlayMode>();
                    break;
                case PlayingMode.Network:
                    playMode = GetComponent<NetworkPlayMode>();
                    if (playMode == null)
                        playMode = gameObject.AddComponent<NetworkPlayMode>();
                    break;
                default:
                    break;
            }
            return playMode;
        }

        public void RestartGame(PlayingMode mode)
        {
            ResetGame();
            StartGame(mode);
        }

        public void ResetGame()
        {
            bird.Reset();
            enviroment.Reset();
        }

        private void PlayGame()
        {
            if (_playMode != null)
            {
                _playMode.Play();
                bird.SetIsSimulated(true);
                _gameState = GameState.Playing;
            }
        }
    }
}