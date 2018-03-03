using UnityEngine;

namespace FlappyBird
{
    public class Bird : MonoBehaviour
    {
        public enum Action
        {
            Idle = 0,
            Flap = 1,
        }
        public bool IsDead { get; set; }
        public bool BetweenInColums { get; set; }
        public int Score { get; set; }
        public bool IsTop {
            get {
                if (_rb2d == null)
                    return true;
                return _rb2d.velocity.y < 0;
            }
        }
        private Rigidbody2D _rb2d;
        private Animator _animator;
        private Vector3 _initPos;
        private Quaternion _initRotation;
        private float _flapForce = 3f;
        private float _gravityScale = 1f;
        private float _topBoundY = 0;

        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _initPos = transform.position;
            _initRotation = transform.rotation;
            _rb2d.gravityScale = _gravityScale;
            _topBoundY = Camera.main.orthographicSize;
            IsDead = false;
            BetweenInColums = false;
        }

        private void Update()
        {
            if(transform.position.y > _topBoundY)
            {
                transform.position = new Vector3(transform.position.x, _topBoundY, transform.position.z);
            }
        }

        public void Flap()
        {
            _rb2d.velocity = Vector2.zero;
            _rb2d.AddForce(_flapForce * Vector2.up * Time.deltaTime * 1000);
            _animator.SetTrigger("Flap");
        }

        public void Flap(Action action)
        {
            if (action == Action.Flap)
                Flap();
            else if (action == Action.Idle)
            {
            }
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