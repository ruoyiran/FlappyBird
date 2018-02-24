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
    public Text scoreText;
    public Text bestScoreText;
    public GameObject gameOverText;
    public float scrollSpeed = -2f;
    private const string BEST_SCORE_PREPREF = "BEST_SCORE_PREPREF";
    private static GameManager _instance;
    private bool _gameOver = false;
    private int _score = 0;
    private int _bestScore = 0;

    private void Awake()
    {
        _instance = this;
        _gameOver = false;
        _bestScore = PlayerPrefs.GetInt(BEST_SCORE_PREPREF, 0);
        SetBestScoreText();
    }

    private void Start () {
        gameOverText.SetActive(false);
    }

    private void Update () {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (_gameOver && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
#else
        if (Input.touchCount > 0)
        {
            if (_gameOver && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
#endif
        SetScoreTexts();
    }

    private void SetScoreTexts()
    {
        scoreText.text = "SCORE: " + _score.ToString();
        if (_score > _bestScore)
        {
            _bestScore = _score;
            SetBestScoreText();
            PlayerPrefs.SetInt(BEST_SCORE_PREPREF, _bestScore);
        }
    }

    private void SetBestScoreText()
    {
        bestScoreText.text = "BEST: " + _bestScore.ToString();
    }

    public void BirdDie()
    {
        _gameOver = true;
        gameOverText.SetActive(true);
    }

    public void BirdScored()
    {
        _score += 1;
    }
}
