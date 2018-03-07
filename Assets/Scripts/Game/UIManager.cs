using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBird
{
    public class UIManager : MonoBehaviour
    {
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
        private static SpeedMode _speedMode = SpeedMode.Middle;
        private int _score;
        private int _bestScore = 0;

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
            GameParams param = GameConfig.ConfigParams[_speedMode];
            GameManager.Instance.enviroment.SetMaxColumnGapY(param.maxGapY);
            GameManager.Instance.enviroment.SetScrollSpeed(param.scrollSpeed);
            GameManager.Instance.bird.SetFlapForce(param.flapForce);
        }
    }
}