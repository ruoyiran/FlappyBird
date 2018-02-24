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

public enum PlayingModel
{
    Free = 0,
    Network = 1,
    AI = 2,
    Training = 3,
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
    public Dropdown playingModelDropdown;
    public GameObject readyStatePanel;
    public GameObject gameOverStatePanel;
    private static PlayingModel _lastPlayingModel;

    public float scrollSpeed = -2f;
    private const string BEST_SCORE_PREPREF = "BEST_SCORE_PREPREF";
    private static GameManager _instance;
    private bool _gameOver = false;
    private int _score = 0;
    private int _bestScore = 0;
    private static PlayingModel _playingModel = PlayingModel.Free;
    private static Communicator _communicator;
    private string _ipAddress = "127.0.0.1";
    private int _port = 8008;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        
        if (_lastPlayingModel == PlayingModel.Training)
        {
            _playingModel = _lastPlayingModel;
            _gameState = GameState.Playing;
        }
        else
        {
            _gameState = GameState.Ready;
            _bestScore = PlayerPrefs.GetInt(BEST_SCORE_PREPREF, 0);
            InitPlayingModelDropdown();
            restarButton.onClick.AddListener(OnRestartButtonClicked);
            playingModelDropdown.onValueChanged.AddListener(OnPlayingModelDropdownValueChanged);
        }
    }

    private void InitPlayingModelDropdown()
    {
        List<string> options = new List<string>();
        foreach (PlayingModel item in Enum.GetValues(typeof(PlayingModel)))
        {
            options.Add(item.ToString());
        }
        playingModelDropdown.AddOptions(options);
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
                CheckPlayingModel();
                break;
            case GameState.GameOver:
                if (_playingModel != PlayingModel.Training)
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

    private void CheckPlayingModel()
    {
        if(_playingModel == PlayingModel.Training)
        {
            playingModelDropdown.gameObject.SetActive(false);
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

    private void OnPlayingModelDropdownValueChanged(int index)
    {
        PlayingModel model = (PlayingModel)index;
        switch (model)
        {
            case PlayingModel.Free:
                DisconnectNetwork();
                break;
            case PlayingModel.Network:
                ConnectToNetwork();
                break;
            case PlayingModel.AI:
                DisconnectNetwork();
                break;
            case PlayingModel.Training:
                ConnectToNetwork();
                break;
            default:
                break;
        }
        _playingModel = model;
        _lastPlayingModel = model;
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
