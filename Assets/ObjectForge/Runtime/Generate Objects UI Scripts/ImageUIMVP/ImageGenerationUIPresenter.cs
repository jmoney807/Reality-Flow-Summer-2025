using UnityEngine;

public class ImageGenerationUIPresenter : MonoBehaviour
{
    [SerializeField] private ImageGenerationUIView imageGenerationUIView;

    public void OnEnable()
    {
        ImageGenerationUIModel.OnImageGenerationStarted += IndicateImageGenerating;
        ImageGenerationUIModel.OnImageGenerationCompleted += IndicateImageGenerationCompleted;
    }

    public void OnDisable()
    {
        ImageGenerationUIModel.OnImageGenerationStarted -= IndicateImageGenerating;
        ImageGenerationUIModel.OnImageGenerationCompleted -= IndicateImageGenerationCompleted;
    }

    public void IndicateImageGenerating()
    {
        // Show the image generation UI
        imageGenerationUIView.ShowImageGenerationUI();
        imageGenerationUIView.SetHeaderText("Generating Image");
        imageGenerationUIView.HideImageResult();
        imageGenerationUIView.ShowLoadingSpinner();
        imageGenerationUIView.HideHorizontalButtonBar();
    }

    public void IndicateImageGenerationCompleted()
    {
        // Hide the loading spinner and show the image result
        imageGenerationUIView.HideLoadingSpinner();

        imageGenerationUIView.ShowImageResult(ImageGenerationUIModel.Instance.RembgResult);

        imageGenerationUIView.ShowHorizontalButtonBar();
        imageGenerationUIView.SetHeaderText("Generate this image as a 3D Object?");
    }

    // Im not sure if this should go here or in the ImageGenerationUIView
    public void ConfirmImageResult()
    {
        ObjectGeneration.RequestObjectGeneration();

        imageGenerationUIView.HideHorizontalButtonBar();
    }

    public void CancelImageGeneration()
    {
        // Hide the image generation UI
        imageGenerationUIView.HideImageGenerationUI();

        // CleanupImageGenerationUIModel();
        Debug.Log("Image Generation Cancelled, Performing Cleanup!");
        ImageGenerationUIModel.Instance.ResetImageGenerationUIModelInstance();
    }

    public void RegenerateImage()
    {
        Debug.Log("Regenerating image...");

        // Add an advanced generation settings panel that opens up
        // Here the user can adjust settings like seed, style, etc
    }
}
