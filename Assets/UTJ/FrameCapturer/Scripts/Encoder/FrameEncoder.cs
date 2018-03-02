using UnityEngine;

namespace UTJ.FrameCapturer
{
    public class FrameEncoder : MovieEncoder
    {
        fcAPI.fcPngConfig m_config;
        int m_frame;

        public override void Release() {}
        public override bool IsValid() { return true; }
        public override Type type { get { return Type.Frame; } }

        public override void Initialize(object config, string outPath)
        {
            if (!fcAPI.fcPngIsSupported())
            {
                Debug.LogError("Png encoder is not available on this platform.");
                return;
            }

            m_config = (fcAPI.fcPngConfig)config;
            m_frame = 0;
        }

        public override void AddVideoFrame(byte[] frame, fcAPI.fcPixelFormat format, double timestamp = -1.0)
        {
            int channels = System.Math.Min(m_config.channels, (int)format & 7);
            ++m_frame;
        }

        public override void AddAudioSamples(float[] samples)
        {
            // not supported
        }

    }
}