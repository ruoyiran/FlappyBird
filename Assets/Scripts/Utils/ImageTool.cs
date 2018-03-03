using UnityEngine;
using System.IO;

public class ImageTool {

    public static void BytesToImage(byte[] bytes)
    {
        string path = "FrameImage.jpg";
        FileStream fs = new FileStream(path, FileMode.Create);//注意123.jpg为你的图片名称
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(bytes, 0, bytes.Length);
        fs.Flush(); //数据写入图片文件
        fs.Close();
    }

    public static void SaveTexture2DToFile(Texture2D tex, string path)
    {
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }

    public static Texture2D RenderToTex(Camera camera, int width, int height)
    {
        if (camera == null)
            return null;
        Camera cam = camera;
        Rect oldRec = camera.rect;
        cam.rect = new Rect(0f, 0f, 1f, 1f);
        bool supportsAntialiasing = false;
        var depth = 24;
        var format = RenderTextureFormat.Default;
        var readWrite = RenderTextureReadWrite.Default;
        var antiAliasing = (supportsAntialiasing) ? Mathf.Max(1, QualitySettings.antiAliasing) : 1;

        var renderRT = RenderTexture.GetTemporary(width, height, depth, format, readWrite, antiAliasing);
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        var prevActiveRT = RenderTexture.active;
        var prevCameraRT = cam.targetTexture;

        // render to offscreen texture (readonly from CPU side)
        RenderTexture.active = renderRT;
        cam.targetTexture = renderRT;

        cam.Render();

        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();
        cam.targetTexture = prevCameraRT;
        cam.rect = oldRec;
        RenderTexture.active = prevActiveRT;
        RenderTexture.ReleaseTemporary(renderRT);
        return tex;
    }
}
