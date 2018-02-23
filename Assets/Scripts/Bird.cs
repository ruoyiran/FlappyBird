using UnityEngine;

public class Bird : MonoBehaviour {
    public float upForce = 200f;
    private Rigidbody2D _rb2d;
    private bool _isDead = false;
    private Animator _animator;

	void Start () {
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }
	
	void Update () {
        if (!_isDead && Input.GetKeyDown(KeyCode.Space))
        {
            _rb2d.velocity = Vector2.zero;
            _rb2d.AddForce(new Vector2(0, upForce));
            _animator.SetTrigger("Flap");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _rb2d.velocity = Vector2.zero;
        _isDead = true;
        _animator.Play("Die");
        GameManager.Instance.BirdDie();
    }
}
