using UnityEngine;

namespace FlappyBird
{
    public class Bird : MonoBehaviour
    {
        public bool IsDead { get; set; }
        public bool BetweenInColums { get; set; }
        public int Score { get; set; }

        private Rigidbody2D _rb2d;
        private Animator _animator;
        private Vector3 _initPos;
        private Quaternion _initRotation;
        private float _flapForce = 3f;
        private float _gravityScale = 1f;

        void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _initPos = transform.position;
            _initRotation = transform.rotation;
            _rb2d.gravityScale = _gravityScale;
            IsDead = false;
            BetweenInColums = false;
        }

        public void Flap()
        {
            _rb2d.velocity = Vector2.zero;
            _rb2d.AddForce(_flapForce * Vector2.up * Time.deltaTime * 1000);
            _animator.SetTrigger("Flap");
        }

        public void SetIsSimulated(bool simulated)
        {
            _rb2d.simulated = simulated;
        }

        public void SetFlapForce(float flapForce)
        {
            _flapForce = flapForce;
        }

        public void SetGravity(float gravity)
        {
            _gravityScale = gravity;
            _rb2d.gravityScale = _gravityScale;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _animator.Play("Die");
            _rb2d.velocity = Vector2.zero;
            IsDead = true;
            BetweenInColums = false;
            StartCoroutine(StopFlying());
        }

        private System.Collections.IEnumerator StopFlying()
        {
            yield return null;
            _rb2d.velocity = Vector2.zero;
        }

        public void Reset()
        {
            IsDead = false;
            Score = 0;
            _rb2d.velocity = Vector2.zero;
            _rb2d.gravityScale = _gravityScale;
            transform.position = _initPos;
            transform.rotation = _initRotation;
            _animator.Play("Idle");
        }
    }
}