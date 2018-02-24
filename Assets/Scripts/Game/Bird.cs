using UnityEngine;

public class Bird : MonoBehaviour {
    public float upForce = 200f;
    private Rigidbody2D _rb2d;
    private Animator _animator;

	void Start () {
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }
	
	void Update() { 
        GameState gameState = GameManager.Instance.CurrentGameState;
        switch (gameState)
        {
            case GameState.Ready:
                _rb2d.simulated = false;
                _animator.SetTrigger("Flap");
                break;
            case GameState.Playing:
                _rb2d.simulated = true;
                CheckInput();
                break;
            case GameState.GameOver:
                break;
            default:
                break;
        }
    }

    private void CheckInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Space))
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
        {
            BirdFlying();
        }
    }

    private void BirdFlying()
    {
        _rb2d.velocity = Vector2.zero;
        _rb2d.AddForce(new Vector2(0, upForce));
        _animator.SetTrigger("Flap");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BirdDied();
    }

    private void BirdDied()
    {
        _rb2d.velocity = Vector2.zero;
        _animator.Play("Die");
        GameManager.Instance.CurrentGameState = GameState.GameOver;
    }
}
