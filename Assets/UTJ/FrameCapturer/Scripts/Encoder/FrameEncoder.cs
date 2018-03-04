//#define FRAME_TEST
using UnityEngine;

namespace UTJ.FrameCapturer
{
    public class FrameEncoder : MovieEncoder
    {
        fcAPI.fcPngContext m_ctx;
        fcAPI.fcPngConfig m_config;
        int m_frame;
        byte[] m_frameData;
        string m_outPath;
        string m_imageFullPath;
        const int kMaxFrameCount = 100;

        public override void Release() {
#if FRAME_TEST
            m_ctx.Release();
#endif
        }
        public override bool IsValid() {
#if FRAME_TEST
            return m_ctx; 
#else
            return true;
#endif
        }
        public override Type type { get { return Type.Frame; } }

        public override void Initialize(object config, string outPath)
        {
#if FRAME_TEST
            if (!fcAPI.fcPngIsSupported())
            {
                Debug.LogError("Png encoder is not available on this platform.");
                return;
            }
            m_config = (fcAPI.fcPngConfig)config;
            m_ctx = fcAPI.fcPngCreateContext(ref m_config);
            m_outPath = outPath;
            m_frame = 0;
#endif
        }

        public override void AddVideoFrame(byte[] frame, fcAPI.fcPixelFormat format, double timestamp = -1.0)
        {
#if FRAME_TEST
            if (m_ctx)
            {
                m_imageFullPath = m_outPath + "Image_" + m_frame.ToString("0000") + ".png";
                int channels = System.Math.Min(m_config.channels, (int)format & 7);
                fcAPI.fcPngExportPixels(m_ctx, m_imageFullPath, frame, m_config.width, m_config.height, format, channels);
            }
            ++m_frame;
            if (m_frame >= kMaxFrameCount)
                m_frame = 0;
#endif
            m_frameData = frame;
        }

        public override void AddAudioSamples(float[] samples)
        {
            // not supported
        }

        public override byte[] GetFrameImageData()
        {
            return m_frameData;
        }

        public override string GetFrameImagePath()
        {
            return m_imageFullPath;
        }
    }
}