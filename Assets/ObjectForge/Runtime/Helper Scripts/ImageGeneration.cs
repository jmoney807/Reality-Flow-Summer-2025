using UnityEngine;
using System;
using System.Collections;

public class ImageGeneration : MonoBehaviour
{
    public static event Action OnGenerateImageRequested;
    public static void RequestImageGeneration() => OnGenerateImageRequested?.Invoke();

    private bool stableDiffusionCompleted = false;
    private bool rembgCompleted = false;
    private byte[] generatedImageData;
    private byte[] rembgImageData;


    [Header("Image Generation and Background Removal Clients")]
    [SerializeField] private StableDiffusionClient stableDiffusionClient;
    [SerializeField] private RembgClient rembgClient;


    private void OnEnable()
    {
        OnGenerateImageRequested += GenerateImage;

        StableDiffusionClient.OnImageGenerationComplete += HandleStableDiffusionComplete;
        StableDiffusionClient.OnImageGenerationFailed += HandleStableDiffusionFailed;

        RembgClient.OnBackgroundRemovalComplete += HandleRembgComplete;
        RembgClient.OnBackgroundRemovalFailed += HandleRembgFailed;
    }

    private void OnDisable()
    {
        OnGenerateImageRequested -= GenerateImage;

        StableDiffusionClient.OnImageGenerationComplete -= HandleStableDiffusionComplete;
        StableDiffusionClient.OnImageGenerationFailed -= HandleStableDiffusionFailed;

        RembgClient.OnBackgroundRemovalComplete -= HandleRembgComplete;
        RembgClient.OnBackgroundRemovalFailed -= HandleRembgFailed;
    }

    private void HandleStableDiffusionComplete(byte[] imageData)
    {
        this.generatedImageData = imageData;
        ImageGenerationUIModel.Instance.StableDiffusionResult = imageData;
        stableDiffusionCompleted = true;
    }

    private void HandleStableDiffusionFailed(string errorMessage)
    {
        Debug.LogError($"Stable Diffusion generation failed: {errorMessage}");
        stableDiffusionCompleted = true; // Set to true to break the waiting loop
        ImageGenerationUIModel.Instance.IsGeneratingImage = false;
    }

    private void HandleRembgComplete(byte[] rembgImageData)
    {
        this.rembgImageData = rembgImageData;
        ImageGenerationUIModel.Instance.RembgResult = rembgImageData;
        rembgCompleted = true;
    }

    private void HandleRembgFailed(string errorMessage)
    {
        Debug.LogError($"Background removal failed: {errorMessage}");
        rembgCompleted = true; // Set to true to break the waiting loop
        ImageGenerationUIModel.Instance.IsGeneratingImage = false;
    }

    public void GenerateImage()
    {
        Debug.Log("Image generation started.");
        StartCoroutine(GenerateImageProcess());
    }

    private IEnumerator GenerateImageProcess()
    {
        // Sets the field and invokes the event to notify that image generation has started
        ImageGenerationUIModel.Instance.IsGeneratingImage = true;

        // Get the transcription result and pass it to the API
        string prompt = TranscriptionUIModel.Instance.TranscriptionResult;
        stableDiffusionClient.GenerateStableDiffusionImage(prompt);

        // Wait for image generation to complete
        yield return StartCoroutine(WaitForCompletion(() => stableDiffusionCompleted, "Image generation timed out"));

        // Reset the completion flag for later generations
        stableDiffusionCompleted = false;

        // Check if generation was successful
        if (generatedImageData == null)
        {
            Debug.LogError("Image generation failed, stopping process");
            ImageGenerationUIModel.Instance.IsGeneratingImage = false;
            yield break;
        }

        Debug.Log("Image generated successfully, removing background");

        // Remove background from the generated image
        // RembgClient.RequestRembg();
        byte[] stableDiffusionResult = ImageGenerationUIModel.Instance.StableDiffusionResult;
        rembgClient.RemoveBackground(stableDiffusionResult);


        // Wait for background removal to complete
        yield return StartCoroutine(WaitForCompletion(() => rembgCompleted, "Background removal timed out"));
        // Reset the completion flag
        rembgCompleted = false;

        // Check if background removal was successful
        if (rembgImageData == null)
        {
            Debug.LogError("Background removal failed, stopping process");
            ImageGenerationUIModel.Instance.IsGeneratingImage = false;
            yield break;
        }

        Debug.Log("Background removed successfully, displaying image");

        // Sets the field and invokes the event to notify that image generation has completed
        ImageGenerationUIModel.Instance.IsGeneratingImage = false;
        
        // Clear the data
        generatedImageData = null;
        rembgImageData = null;
    }

    private IEnumerator WaitForCompletion(Func<bool> isCompleted, string timeoutMessage, float timeoutDuration = 60f, float pollingInterval = 0.5f)
    {
        float elapsedTime = 0f;

        while (!isCompleted())
        {
            // Check for timeout
            if (elapsedTime > timeoutDuration)
            {
                Debug.LogError(timeoutMessage);
                yield break;
            }

            yield return new WaitForSeconds(pollingInterval);
            elapsedTime += pollingInterval;
        }
    }
}
