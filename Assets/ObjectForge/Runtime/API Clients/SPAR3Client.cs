using System;
using System.Collections;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class SPAR3DClient : MonoBehaviour
{
    public static event Action<byte[]> OnSPAR3DGenerationComplete;
    public static event Action<string> OnSPAR3DGenerationFailed;

    private string apiKey;

    [Header("Server URL")]
    [SerializeField] private string serverUrl = "https://api.stability.ai/v2beta/3d/stable-point-aware-3d";

    [Header("Request Body Variables")]
    public int textureResolution = 1024;

    [Range(1f, 2f)]
    public float foregroundRatio = 1.3f;

    public string remesh = "none";
    public string targetType = "none";

    [Range(100, 20000)]
    public int targetCount = 1000;

    [Range(1, 10)]
    public int guidanceScale = 3;

    [Range(0, 10000)]
    public int seed = 0;
    
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

        // WWWForm formData = new WWWForm();
        // formData.AddBinaryData("image", inputImage, "input.png", "image/png");

        Debug.Log($"Sending request to {serverUrl} with texture resolution {textureResolution}, foreground ratio {foregroundRatio}, remesh {remesh}, target type {targetType}, target count {targetCount}, guidance scale {guidanceScale}, seed {seed}");
        
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>
        {
            new MultipartFormFileSection("image", inputImage, "input.png", "image/png"),
            new MultipartFormDataSection("texture_resolution", textureResolution.ToString()),
            new MultipartFormDataSection("foreground_ratio", foregroundRatio.ToString("F1")),
            new MultipartFormDataSection("remesh", remesh),
            new MultipartFormDataSection("target_type", targetType),
            new MultipartFormDataSection("target_count", targetCount.ToString()),
            new MultipartFormDataSection("guidance_scale", guidanceScale.ToString()),
            new MultipartFormDataSection("seed", seed.ToString())
        };
       
        // Create a POST request to the API endpoint, sending the form data.
        UnityWebRequest request = UnityWebRequest.Post(serverUrl, formData);
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