using UnityEngine;
namespace FlappyBird
{
    public class BasePlayMode : MonoBehaviour, IPlayMode
    {
        protected bool _isPlaying = false;

        protected void Update()
        {
            if(_isPlaying)
                CheckInput();
        }

        public void Play()
        {
            _isPlaying = true;
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        private void CheckInput()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetKeyDown(KeyCode.Space))
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
            {
                Bird bird = GameManager.Instance.bird;
                if (bird != null)
                    bird.Flap();
            }
        }
    }
}