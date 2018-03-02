using System.Collections.Generic;

namespace FlappyBird
{
    public struct GameParams
    {
        public float scrollSpeed;
        public float flapForce;
        public float gravity;
        public float maxGapY;
        public GameParams(float scrollSpeed = -2f, float flapForce = 15f, float gravity = 1.0f, float maxGapY = 1.5f)
        {
            this.scrollSpeed = scrollSpeed;
            this.flapForce = flapForce;
            this.gravity = gravity;
            this.maxGapY = maxGapY;
        }
    }

    public class GameConfig
    {
        public static Dictionary<SpeedMode, GameParams> ConfigParams = new Dictionary<SpeedMode, GameParams>()
        {
            { SpeedMode.Easy, new GameParams(-4f, 14f, 1.5f, 1.5f) },
            { SpeedMode.Middle, new GameParams(-6f, 16f, 1.8f, 1.5f) },
            { SpeedMode.Hard, new GameParams(-8f, 18f, 2.0f, 1.3f) }
        };
    }
}