using UnityEngine;

public class ImageGenerationUIPresenter : MonoBehaviour
{
    [SerializeField] private ImageGenerationUIView imageGenerationUIView;
    [SerializeField] private TranscriptionUIView transcriptionUIView;

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
        transcriptionUIView.ShowTranscriptionButtons();


        // CleanupImageGenerationUIModel();
        Debug.Log("Image Generation Cancelled, Performing Cleanup!");
        ImageGenerationUIModel.Instance.ResetImageGenerationUIModelInstance();
    }

    public void RegenerateImage()
    {
        Debug.Log("Regenerating image...");
        // Reset the seed value to a random number if it has not already been adjusted
        // StableDiffusionSettingsUIModel.Instance.Seed = Random.Range(0, 10001);

        if (CheckIfSettingsAreDifferent())
        {
            // MIGHT NEED TO ADD LOGIC TO CHECK IF SEED HAS BEEN CHANGED FROM 0, AND IF SO THEN WE MIGHT HAVE TO REVERT TO THE DEFAULT SEED OR ELSE GENERATION COULD BE THE SAME? BUT NOT SURE IF THATS THE CASE
            ImageGeneration.RequestImageGeneration();
        }
        else
        {
            Debug.Log("Settings have not changed, cannot regenerate image.");
            // Show a warning to the user that they need to change the settings before regenerating
            // MAYBE IMPLEMENT A UI POPUP TO TELL THE USER THIS WITH SPECIFIC SETTINGS THAT HAVE NOT CHANGED
        }
    }

    public bool CheckIfSettingsAreDifferent()
    {
        int currentSeed = StableDiffusionSettingsUIModel.Instance.Seed;
        int prevSeed = StableDiffusionSettingsUIModel.Instance.PreviousSeed;
        int currentCFGScale = StableDiffusionSettingsUIModel.Instance.CFGScale;
        int prevCFGScale = StableDiffusionSettingsUIModel.Instance.PreviousCFGScale;

        // IMPLEMENT WARNINGS TO THE USER TO TELL THEM THE SPECIFIC SETTING HAS NOT CHANGED AND THEY CANNOT REGENERATE UNTIL THEY CHANGE IT
        if (prevSeed == currentSeed && prevCFGScale == currentCFGScale)
        {
            Debug.Log("Settings have not changed, cannot regenerate image.");
            return false;
        }

        return true;
    }
   
}
