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
    public Text scoreText;
    public GameObject gameOverText;
    public float scrollSpeed = -2f;
    private static GameManager _instance;
    private bool _gameOver = false;
    private int _score = 0;

    private void Awake()
    {
        _instance = this;
        _gameOver = false;
    }

    private void Start () {
        gameOverText.SetActive(false);
    }

    private void Update () {
		if(_gameOver && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        scoreText.text = "SCORE: " + _score.ToString();
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
