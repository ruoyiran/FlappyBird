using UnityEngine;

public class ScrollObject : MonoBehaviour {
    private Rigidbody2D _rb2d;
    private Vector2 scrollSpeed;
    void Start () {
        _rb2d = GetComponent<Rigidbody2D>();
        scrollSpeed = new Vector2(GameManager.Instance.ScrollSpeed, 0);
    }

    void Update () {
        scrollSpeed.x = GameManager.Instance.ScrollSpeed;
        GameState gameState = GameManager.Instance.CurrentGameState;
        switch (gameState)
        {
            case GameState.Ready:
                if (gameObject.CompareTag("Background"))
                    _rb2d.velocity = scrollSpeed;
                break;
            case GameState.Playing:
                _rb2d.velocity = scrollSpeed;
                break;
            case GameState.GameOver:
                _rb2d.velocity = Vector2.zero;
                break;
            default:
                break;
        }
    }
}
