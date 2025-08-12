using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TrellisClient : MonoBehaviour
{
    public static event Action<byte[]> OnTrellisGenerationComplete;
    public static event Action<string> OnTrellisGenerationFailed;

    // private string apiKey;
    [SerializeField] private string APIEndpoint = "http://host.docker.internal:8003/get-glb";


    public void GenerateTrellisObject(byte[] inputImage)
    {
        Debug.Log("Generating Trellis Object...");
        StartCoroutine(RequestObjectGeneration(inputImage));
    }

    private IEnumerator RequestObjectGeneration(byte[] inputImage)
    {
        Debug.Log("Requesting Trellis object generation...");

        // Create multipart form data
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", inputImage, "input_image.png", "image/png");

        UnityWebRequest request = UnityWebRequest.Post(APIEndpoint, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("3D model received from Trellis!");
            OnTrellisGenerationComplete?.Invoke(request.downloadHandler.data);
        }
        else
        {
            string errorMessage = $"API Error: {request.error}\nResponse: {request.downloadHandler.text}";
            
            // // Check for quota exceeded error
            // if (request.responseCode == 429 || request.downloadHandler.text.Contains("quota"))
            // {
            //     errorMessage = "GPU quota exceeded. Please try again later or upgrade to Hugging Face Pro.";
            //     Debug.LogWarning("TRELLIS GPU quota exceeded");
            // }
            
            Debug.LogError(errorMessage);
            OnTrellisGenerationFailed?.Invoke(errorMessage);
        }
    }
}
