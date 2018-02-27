using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBird
{
    public class UIManager : MonoBehaviour
    {
        struct GameParams
        {
            public float scrollSpeed;
            public float flapForce;
            public float gravity;
            public float maxGapY;
            public GameParams(float scrollSpeed = -2f, float flapForce = 15f, float gravity = 1.0f, float maxGapY = 1.5f)
            {
                this.scrollSpeed = scrollSpeed;
                this.flapForce = flapForce;
                this.gravity = gravity;
                this.maxGapY = maxGapY;
            }
        }

        public Text scoreText;
        public Text finalScoreText;
        public Text bestScoreText;
        public Button restartButton;
        public Button startGameButton;
        public Dropdown playingModeDropdown;
        public Dropdown speedModeDropdown;
        public GameObject readyStatePanel;
        public GameObject gameOverStatePanel;

        private const string BEST_SCORE_PREPREF = "BEST_SCORE_PREPREF";
        private static PlayingMode _playingMode = PlayingMode.Free;
        private static SpeedMode _speedMode = SpeedMode.Normal;
        private int _score;
        private int _bestScore = 0;
        private Dictionary<SpeedMode, GameParams> _gameConfig = new Dictionary<SpeedMode, GameParams>()
        {
            { SpeedMode.Easy, new GameParams(-4f, 14f, 1.5f, 1.5f) },
            { SpeedMode.Normal, new GameParams(-6f, 16f, 1.8f, 1.5f) },
            { SpeedMode.Hard, new GameParams(-8f, 18f, 2.0f, 1.3f) }
        };

        void Start()
        {
            _bestScore = PlayerPrefs.GetInt(BEST_SCORE_PREPREF, 0);
            InitPlayingModeDropdown();
            InitSpeedModeDropdown();
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            startGameButton.onClick.AddListener(OnStartGameButtonClicked);
            speedModeDropdown.onValueChanged.AddListener(OnSpeedModeDropdownValueChanged);
            SetGameParams();
        }

        private void Update()
        {
            GameState gameState = GameManager.Instance.CurrentGameState;
            switch (gameState)
            {
                case GameState.Ready:
                    break;
                case GameState.Playing:
                    _score = GameManager.Instance.bird.Score;
                    SetScoreText();
                    break;
                case GameState.GameOver:
                    GameOverState();
                    SetBoxScoreTexts();
                    break;
                default:
                    break;
            }
        }

        private void InitPlayingModeDropdown()
        {
            List<string> options = new List<string>();
            foreach (PlayingMode item in Enum.GetValues(typeof(PlayingMode)))
            {
                options.Add(item.ToString());
            }
            playingModeDropdown.AddOptions(options);
            speedModeDropdown.value = (int)_playingMode;
        }

        private void InitSpeedModeDropdown()
        {
            List<string> options = new List<string>();
            foreach (SpeedMode item in Enum.GetValues(typeof(SpeedMode)))
            {
                options.Add(item.ToString());
            }
            speedModeDropdown.AddOptions(options);
            speedModeDropdown.value = (int)_speedMode;
        }

        private void OnStartGameButtonClicked()
        {
            ResetScore();
            SetUIVisibleStates();
            GameManager.Instance.StartGame((PlayingMode)playingModeDropdown.value);
        }

        private void OnRestartButtonClicked()
        {
            ResetScore();
            SetUIVisibleStates();
            GameManager.Instance.RestartGame((PlayingMode)playingModeDropdown.value);
        }

        private void SetUIVisibleStates()
        {
            scoreText.gameObject.SetActive(true);
            readyStatePanel.SetActive(false);
            gameOverStatePanel.SetActive(false);
        }

        private void ResetScore()
        {
            _score = 0;
        }

        private void SetScoreText()
        {
            scoreText.text = _score.ToString();
        }

        private void GameOverState()
        {
            scoreText.gameObject.SetActive(false);
            readyStatePanel.SetActive(false);
            gameOverStatePanel.SetActive(true);
        }

        private void SetBoxScoreTexts()
        {
            finalScoreText.text = _score.ToString();
            if (_score > _bestScore)
            {
                _bestScore = _score;
                PlayerPrefs.SetInt(BEST_SCORE_PREPREF, _bestScore);
            }
            bestScoreText.text = _bestScore.ToString();
        }

        private void OnSpeedModeDropdownValueChanged(int index)
        {
            _speedMode = (SpeedMode)index;
            SetGameParams();
        }

        private void SetGameParams()
        {
            GameParams param = _gameConfig[_speedMode];
            GameManager.Instance.enviroment.SetMaxColumnGapY(param.maxGapY);
            GameManager.Instance.enviroment.SetScrollSpeed(param.scrollSpeed);
            GameManager.Instance.bird.SetFlapForce(param.flapForce);
            GameManager.Instance.bird.SetGravity(param.gravity);
        }
    }
}