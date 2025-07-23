using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TrellisClient : MonoBehaviour
{
    public static event Action<byte[]> OnTrellisGenerationComplete;
    public static event Action<string> OnTrellisGenerationFailed;

    private string apiKey;
    [SerializeField] private string APIEndpoint = "http://localhost:7860";

    private void Awake()
    {
        // // Load the HuggingFace API key from the .env file
        // apiKey = EnvLoader.Get("HUGGINGFACE_API_KEY");
        // if (string.IsNullOrEmpty(apiKey))
        // {
        //     Debug.LogError("HuggingFace API key is not set. Please check your .env file.");
        // }
    }

    public void GenerateTrellisObject(byte[] inputImage)
    {
        Debug.Log("Generating Trellis Object...");
        StartCoroutine(RequestObjectGeneration(inputImage));
    }

    private IEnumerator RequestObjectGeneration(byte[] inputImage)
    {
        Debug.Log("Requesting Trellis object generation...");

        UnityWebRequest request = new UnityWebRequest(APIEndpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(inputImage);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        request.SetRequestHeader("Content-Type", "image/png");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("3D model received from Trellis!");
            OnTrellisGenerationComplete?.Invoke(request.downloadHandler.data);
        }
        else
        {
            string errorMessage = $"API Error: {request.error}\nResponse: {request.downloadHandler.text}";
            Debug.LogError(errorMessage);
            OnTrellisGenerationFailed?.Invoke(errorMessage);
        }
    }
}
