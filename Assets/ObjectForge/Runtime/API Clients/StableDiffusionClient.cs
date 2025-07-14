using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class StableDiffusionClient : MonoBehaviour
{
    public static event Action<byte[]> OnImageGenerationComplete;
    public static event Action<string> OnImageGenerationFailed;

    [SerializeField] private string apiEndpoint = "https://api.stability.ai/v2beta/stable-image/generate/sd3";
    private string apiKey;

    private void Awake()
    {
        // Load the API key from the .env file
        apiKey = EnvLoader.Get("STABILITYAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API key is not set. Please check your .env file.");
        }
    }

    public void GenerateStableDiffusionImage(string prompt)
    {
        Debug.Log("NEW Generating image...");
        StartCoroutine(RequestImageGeneration(prompt));
    }

    private IEnumerator RequestImageGeneration(string transcriptionResult)
    {
        string prompt = transcriptionResult + " at a 3/4 perspective with no shadows";
        Debug.Log($"Prompt: {prompt}");

        // Create the multipart form with a string field that contains the prompt 
        List<IMultipartFormSection> formSections = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("prompt", prompt), // Required
                                                           // Add other parameters as needed 
                                                           // mode default: text-to-image
                                                           // Can potentially add "negative_prompt" to omit certain things like nudity, etc.
            new MultipartFormDataSection("style_preset", "3d-model")
            // new MultipartFormDataSection("style_preset", "low-poly") // Example of a style
        };

        UnityWebRequest request = UnityWebRequest.Post(apiEndpoint, formSections);

        request.SetRequestHeader("authorization", $"Bearer {apiKey}");
        // request.SetRequestHeader("content-type", "multipart/form-data"); Not needed as specified by StablityAI
        request.SetRequestHeader("accept", "image/*");

        Debug.Log("Sending request to Stable Diffusion API...");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image generated successfully!");

            OnImageGenerationComplete?.Invoke(request.downloadHandler.data);
        }
        else
        {
            string errorMessage = $"Error: {request.error}\nResponse: {request.downloadHandler.text}";
            Debug.LogError(errorMessage);
            OnImageGenerationFailed?.Invoke(errorMessage);
        }
    }
}
