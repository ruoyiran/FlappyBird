namespace FlappyBird
{
    public class FreePlayMode : BasePlayMode, IPlayMode
    {
        private void Start()
        {
            Logger.Print("FreePlayMode.Start");
        }

        private new void Update()
        {
            base.Update();
        }
    }
}