using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

public class GameManager : MonoBehaviour {
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public bool IsGameOver
    {
        get
        {
            return _gameOver;
        }
    }

    public GameState CurrentGameState
    {
        get
        {
            return _gameState;
        }
        set
        {
            _gameState = value;
        }
    }

    private GameState _gameState;
    public Text scoreText;
    public Text finalScoreText;
    public Text bestScoreText;
    public Button restarButton;
    public Dropdown playingModeDropdown;
    public Dropdown speedModeDropdown;
    public GameObject readyStatePanel;
    public GameObject gameOverStatePanel;
    private static PlayingMode _lastPlayingMode;

    public float ScrollSpeed {
        get
        {
            return scrollSpeed[_speedMode];
        }
    }
    private Dictionary<SpeedMode, float> scrollSpeed = new Dictionary<SpeedMode, float>()
    {
        { SpeedMode.Easy, -2f}, { SpeedMode.Normal, -4}, { SpeedMode.Hard, -8}
    };
    private static Communicator _communicator;
    private static PlayingMode _playingMode = PlayingMode.Free;
    private static SpeedMode _speedMode = SpeedMode.Normal;
    private const string BEST_SCORE_PREPREF = "BEST_SCORE_PREPREF";
    private static GameManager _instance;
    private bool _gameOver = false;
    private int _score = 0;
    private int _bestScore = 0;
    private string _ipAddress = "127.0.0.1";
    private int _port = 8008;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        
        if (_lastPlayingMode == PlayingMode.Network)
        {
            _playingMode = _lastPlayingMode;
            _gameState = GameState.Playing;
        }
        else
        {
            _gameState = GameState.Ready;
            _bestScore = PlayerPrefs.GetInt(BEST_SCORE_PREPREF, 0);
            InitPlayingModeDropdown();
            InitSpeedModeDropdown();
            restarButton.onClick.AddListener(OnRestartButtonClicked);
            playingModeDropdown.onValueChanged.AddListener(OnPlayingModeDropdownValueChanged);
            speedModeDropdown.onValueChanged.AddListener(OnSpeedModeDropdownValueChanged);
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

    private void Update () {
        switch (_gameState)
        {
            case GameState.Ready:
                ReadyState();
                CheckInput();
                break;
            case GameState.Playing:
                PlayingState();
                CheckPlayingMode();
                break;
            case GameState.GameOver:
                if (_playingMode != PlayingMode.Network)
                {
                    GameOverState();
                    SetScoreTexts();
                    DisconnectNetwork();
                }
                else
                {
                    RestartGame();
                }
                break;
            default:
                break;
        }
    }

    private void ReadyState()
    {
        _score = 0;
        scoreText.gameObject.SetActive(true);
        readyStatePanel.SetActive(true);
        gameOverStatePanel.SetActive(false);
        SetBoxScoreTexts();
    }

    private void PlayingState()
    {
        scoreText.gameObject.SetActive(true);
        readyStatePanel.SetActive(false);
        gameOverStatePanel.SetActive(false);
        SetBoxScoreTexts();
    }

    private void CheckPlayingMode()
    {
        if(_playingMode == PlayingMode.Network)
        {
            playingModeDropdown.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(false);
            readyStatePanel.SetActive(false);
            gameOverStatePanel.SetActive(false);
        }
    }

    private void GameOverState()
    {
        scoreText.gameObject.SetActive(false);
        readyStatePanel.SetActive(false);
        gameOverStatePanel.SetActive(true);
    }

    private void SetBoxScoreTexts()
    {
        scoreText.text = _score.ToString();
    }

    private void CheckInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _gameState = GameState.Playing;
        }
#else
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _gameState = GameState.Playing;
            }
        }
#endif
    }

    private void SetScoreTexts()
    {
        finalScoreText.text = _score.ToString();
        if (_score > _bestScore)
        {
            _bestScore = _score;
            PlayerPrefs.SetInt(BEST_SCORE_PREPREF, _bestScore);
        }
        bestScoreText.text = _bestScore.ToString();
    }

    public void BirdScored()
    {
        _score += 1;
        SetScoreTexts();
    }

    private void OnRestartButtonClicked()
    {
        RestartGame();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnPlayingModeDropdownValueChanged(int index)
    {
        PlayingMode model = (PlayingMode)index;
        switch (model)
        {
            case PlayingMode.Free:
                DisconnectNetwork();
                break;
            case PlayingMode.AI:
                DisconnectNetwork();
                break;
            case PlayingMode.Network:
                ConnectToNetwork();
                break;
            default:
                break;
        }
        _playingMode = model;
        _lastPlayingMode = model;
    }

    private void OnSpeedModeDropdownValueChanged(int index)
    {
        _speedMode = (SpeedMode)index;
    }

    private void ConnectToNetwork()
    {
        if (_communicator == null)
            _communicator = new Communicator();
        if(!_communicator.IsConnected)
            _communicator.ConnectToServer(_ipAddress, _port);
    }

    private void DisconnectNetwork()
    {
        if (_communicator != null && _communicator.IsConnected)
            _communicator.Disconnect();
    }
}
