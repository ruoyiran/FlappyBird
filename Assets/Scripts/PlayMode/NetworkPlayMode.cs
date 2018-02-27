namespace FlappyBird
{
    public class NetworkPlayMode : BasePlayMode, IPlayMode
    {
        private void Start()
        {
            Logger.Print("NetworkPlayMode.Start");
        }

        private void Update()
        {
            if (!_isPlaying)
                return;
        }

        public void Play()
        {
            _isPlaying = true;
        }

        public void Stop()
        {
            _isPlaying = false;
        }
    }
}