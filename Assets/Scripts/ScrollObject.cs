using UnityEngine;

public class ScrollObject : MonoBehaviour {
    private Rigidbody2D _rb2d;

    void Start () {
        _rb2d = GetComponent<Rigidbody2D>();
        _rb2d.velocity = new Vector2(GameManager.Instance.scrollSpeed, 0);
    }

    void Update () {
        if (GameManager.Instance.IsGameOver == true)
            _rb2d.velocity = Vector2.zero;
    }
}
