using System;
using UnityEngine;

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
        Normal = 1,
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
        public GameState CurrentGameState { get { return _gameState; } }

        private IPlayMode _playMode;
        private GameState _gameState = GameState.Ready;
        private bool _autoStart = false;

        private void Awake()
        {
            _instance = this;
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
                if (_autoStart)
                {
                    ResetGame();
                    PlayGame();
                }
                else
                {
                    _gameState = GameState.GameOver;
                    StopCurrentPlayingMode();
                }
            }
        }

        public void SetAutoStart(bool autoStart)
        {
            _autoStart = autoStart;
        }

        private void StopCurrentPlayingMode()
        {
            if (_playMode != null)
                _playMode.Stop();
        }

        public void StartGame(PlayingMode mode)
        {
            StopCurrentPlayingMode();
            _playMode = GetPlayMode(mode);
            _autoStart = false;
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

        private void ResetGame()
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