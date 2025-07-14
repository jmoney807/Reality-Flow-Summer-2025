using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SPAR3DClient : MonoBehaviour
{
    public static event Action<byte[]> OnSPAR3DGenerationComplete;
    public static event Action<string> OnSPAR3DGenerationFailed;

    private string apiKey;
    [SerializeField] private string APIEndpoint = "https://api.stability.ai/v2beta/3d/stable-point-aware-3d";

    private void Awake()
    {
        // Load the API key from the .env file
        apiKey = EnvLoader.Get("STABILITYAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API key is not set. Please check your .env file.");
        }
    }

    public void GenerateSPAR3DObject(byte[] inputImage)
    {
        Debug.Log("Generating SPAR3D Object...");
        StartCoroutine(RequestObjectGeneration(inputImage));
    }

    private IEnumerator RequestObjectGeneration(byte[] inputImage)
    {
        Debug.Log("Requesting object generation...");

        WWWForm form = new WWWForm();
        form.AddBinaryData("image", inputImage, "input.png", "image/png");

        // // Create the multipart form with a string field that contains the prompt 
        // List<IMultipartFormSection> form = new List<IMultipartFormSection>
        // {
        //     new MultipartFormDataSection("image", inputImage) // Required
        //                                                    // Add other parameters as needed 
        // };
        
        // Create a POST request to the API endpoint, sending the form data.
        UnityWebRequest request = UnityWebRequest.Post(APIEndpoint, form);
        request.SetRequestHeader("authorization", $"Bearer {apiKey}");
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("3D model received from Stability AI!");
            OnSPAR3DGenerationComplete?.Invoke(request.downloadHandler.data);
        }
        else
        {
            string errorMessage = $"API Error: {request.error}\nResponse: {request.downloadHandler.text}";
            Debug.LogError(errorMessage);
            OnSPAR3DGenerationFailed?.Invoke(errorMessage);
        }
    }
}