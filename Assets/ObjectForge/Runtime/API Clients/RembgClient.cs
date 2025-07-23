using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RembgClient : MonoBehaviour
{
   
    public static event Action<byte[]> OnBackgroundRemovalComplete;
    public static event Action<string> OnBackgroundRemovalFailed;


    // [SerializeField] private string serverUrl = "http://localhost:8000/remove-bg/";
    [SerializeField] private string serverUrl = "http://host.docker.internal:8000/remove-bg/";

    public void RemoveBackground(byte[] imageData)
    {
        StartCoroutine(RequestRembg(imageData));
    }

    IEnumerator RequestRembg(byte[] image)
    {
        Debug.Log("Sending image to Rembg server...");

        if (image == null || image.Length == 0)
        {
            string errorMessage = "Input image data is null or empty!";
            Debug.LogError(errorMessage);
            OnBackgroundRemovalFailed?.Invoke(errorMessage);
            yield break;
        }

        // Create a form and add the image data
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", image, "input.png", "image/png");

        UnityWebRequest www = UnityWebRequest.Post(serverUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorMessage = $"Error from Rembg server: {www.error}";
            Debug.LogError(errorMessage);
            OnBackgroundRemovalFailed?.Invoke(errorMessage);
        }
        else
        {
            Debug.Log("Background removal completed successfully!");
            OnBackgroundRemovalComplete?.Invoke(www.downloadHandler.data);
        }
    }
}
