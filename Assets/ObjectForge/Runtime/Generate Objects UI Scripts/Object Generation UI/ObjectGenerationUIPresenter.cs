using UnityEngine;

public class ObjectGenerationUIPresenter : MonoBehaviour
{
    [SerializeField] private ObjectGenerationUIView objectGenerationUIView;
    [SerializeField] private ImageGenerationUIView imageGenerationUIView;
    [SerializeField] private TranscriptionUIView transcriptionUIView;

    public void OnEnable()
    {
        ObjectGenerationUIModel.OnObjectGenerationStarted += IndicateObjectGenerationStarted;
        ObjectGenerationUIModel.OnObjectGenerationCompleted += IndicateObjectGenerationCompleted;
    }

    public void OnDisable()
    {
        ObjectGenerationUIModel.OnObjectGenerationStarted -= IndicateObjectGenerationStarted;
        ObjectGenerationUIModel.OnObjectGenerationCompleted -= IndicateObjectGenerationCompleted;
    }

    public void IndicateObjectGenerationStarted()
    {
        objectGenerationUIView.IndicateObjectGenerationStarted();

    }
    public void IndicateObjectGenerationCompleted()
    {
        objectGenerationUIView.IndicateObjectGenerationCompleted();
    }

    // Hide all previous UI elements since the object generation is complete (NEEDS TO BE IMPLEMENTED)
    public void ConfirmObjectGenerationResult()
    {
        Debug.Log("Object generation confirmed.");
        objectGenerationUIView.HideObjectGenerationUI();
        imageGenerationUIView.HideImageGenerationUI();
        transcriptionUIView.HideTranscriptionUI();
    }

    public void RegenerateObject()
    {
        Debug.Log("Regenerating object...");
        
        if (CheckIfSettingsAreDifferent())
        {
            ObjectGeneration.RequestObjectGeneration();
        }     
    }

    // Returns the user to the image generation UI so they can regenerate a new image for the input for the object generation.
    public void ReturnToImageGeneration()
    {
        // Hide the object generation UI and show the image generation button options again.
        objectGenerationUIView.HideObjectGenerationUI();
        imageGenerationUIView.ShowHorizontalButtonBar();
    }

    public void CloseObjectGenerationResult()
    {
        Debug.Log("Object generation result rejected.");
        objectGenerationUIView.HideObjectGenerationUI();
    }

    public bool CheckIfSettingsAreDifferent()
    {
        float currentForegroundRatio = SPAR3DSettingsModel.Instance.ForegroundRatio;
        int currentGuidanceScale = SPAR3DSettingsModel.Instance.GuidanceScaleValue;
        int currentSeed = SPAR3DSettingsModel.Instance.SeedValue;
        int currentTextureResolution = SPAR3DSettingsModel.Instance.TextureResolution;

        float previousForegroundRatio = SPAR3DSettingsModel.Instance.PreviousForegroundRatio;
        int previousGuidanceScale = SPAR3DSettingsModel.Instance.PreviousGuidanceScaleValue;
        int previousSeed = SPAR3DSettingsModel.Instance.PreviousSeedValue;
        int previousTextureResolution = SPAR3DSettingsModel.Instance.PreviousTextureResolution;

        // IMPLEMENT WARNINGS TO THE USER TO TELL THEM THE SPECIFIC SETTING HAS NOT CHANGED AND THEY CANNOT REGENERATE UNTIL THEY CHANGE IT
        if (previousForegroundRatio == currentForegroundRatio && previousGuidanceScale == currentGuidanceScale && previousSeed == currentSeed && previousTextureResolution == currentTextureResolution)
        {
            Debug.Log("Settings have not changed, cannot regenerate object.");
            // Show a warning to the user that they need to change the settings before regenerating
            // MAYBE IMPLEMENT A UI POPUP TO TELL THE USER THIS WITH SPECIFIC SETTINGS THAT HAVE NOT CHANGED
            return false;
        }

        return true;
    }


}
