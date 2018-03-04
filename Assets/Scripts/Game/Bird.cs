using System;
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
        public bool IsTop { get; set; }
        private Animator _animator;
        private Vector3 _initPos;
        private Quaternion _initRotation;
        private float _flapForce = 3f;
        private float _gravityScale = 1f;
        private float _topBoundY = 0;
        private float _bottomBoundY = -2.3f;

        private float birdVelY = 0;    // bird's velocity along Y, default same as birdFlapped
        private float birdMaxVelY = 1f; // max vel along Y, max descend speed
        private float birdAccY = 0.01f;   // birds downward accleration
        private float birdFlapAcc = -0.12f;   // birds speed on flapping
        private bool birdFlapped = false;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _initPos = transform.position;
            _initRotation = transform.rotation;
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
            else if(transform.position.y <= _bottomBoundY)
            {
                transform.position = new Vector3(transform.position.x, _bottomBoundY, transform.position.z);
                _animator.Play("Die");
                IsDead = true;
            }
        }

        private void UpdateBirdPosY()
        {
            transform.position += Vector3.down * birdVelY;
        }

        private float GetBirdVelY()
        {
            return transform.position.y;
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance.CurrentGameState == GameState.Playing)
            {
                if (birdFlapped)
                    birdVelY = birdFlapAcc;
                if (birdVelY < birdMaxVelY && !birdFlapped)
                {
                    birdVelY += birdAccY;
                }
                if (birdFlapped)
                    birdFlapped = false;
                UpdateBirdPosY();
                IsTop = true;
            }
        }

        public void Flap()
        {
            IsTop = false;
            birdFlapped = true;
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
        }

        public void SetFlapForce(float flapForce)
        {
            _flapForce = flapForce;
        }

        public void SetGravity(float gravity)
        {
            _gravityScale = gravity;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "Gap")
            {
                Score += 1;
                BetweenInColums = true;
            }
            else
            {
                BirdDead();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Gap")
            {
                BetweenInColums = false;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == "Gap")
            {
                BetweenInColums = true;
            }
        }

        private void BirdDead()
        {
            _animator.Play("Die");
            BetweenInColums = false;
            IsDead = true;
        }

        public void Reset()
        {
            birdVelY = 0;    // bird's velocity along Y, default same as birdFlapped
            birdFlapped = false;
            IsDead = false;
            Score = 0;
            transform.position = _initPos;
            transform.rotation = _initRotation;
            _animator.Play("Idle");
        }
    }
}