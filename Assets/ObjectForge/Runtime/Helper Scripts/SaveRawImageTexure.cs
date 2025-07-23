using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveRawImageTexture : MonoBehaviour
{
    public RawImage rawImage;

    [ContextMenu("Save RawImage Texture to PNG")]
    public void SaveTexture()
    {
        if (rawImage == null || rawImage.texture == null)
        {
            Debug.LogWarning("RawImage or texture is null.");
            return;
        }

        Texture texture = rawImage.texture;

        Texture2D tex2D = texture as Texture2D;
        if (tex2D == null)
        {
            // If it's a RenderTexture, convert it
            RenderTexture rt = texture as RenderTexture;
            if (rt != null)
            {
                tex2D = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
                RenderTexture.active = rt;
                tex2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                tex2D.Apply();
                RenderTexture.active = null;
            }
            else
            {
                Debug.LogError("Texture is not Texture2D or RenderTexture.");
                return;
            }
        }

        // Save as PNG
        byte[] bytes = tex2D.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, "SavedImage.png");
        File.WriteAllBytes(path, bytes);
        Debug.Log("Saved image to: " + path);
    }
}
