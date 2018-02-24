using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public GameObject readyStatePanel;
    public GameObject gameOverStatePanel;

    public float scrollSpeed = -2f;
    private const string BEST_SCORE_PREPREF = "BEST_SCORE_PREPREF";
    private static GameManager _instance;
    private bool _gameOver = false;
    private int _score = 0;
    private int _bestScore = 0;

    private void Awake()
    {
        _instance = this;
        _bestScore = PlayerPrefs.GetInt(BEST_SCORE_PREPREF, 0);
        _gameState = GameState.Ready;
    }

    private void Start()
    {
        restarButton.onClick.AddListener(OnRestartButtonClicked);
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
                break;
            case GameState.GameOver:
                GameOverState();
                SetScoreTexts();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
