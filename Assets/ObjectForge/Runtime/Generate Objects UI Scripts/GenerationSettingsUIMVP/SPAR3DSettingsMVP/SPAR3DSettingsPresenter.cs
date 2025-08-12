using UnityEngine;
using MixedReality.Toolkit.UX;
using System;

public class SPAR3DSettingsUIPresenter : MonoBehaviour
{
    [Header("MVP Components")]
    [SerializeField] private SPAR3DSettingsModel model;
    [SerializeField] private SPAR3DSettingsView view;

    [Header("Default Button")]
    [SerializeField] private PressableButton defaultButton;

    [Header("Sliders")]
    [SerializeField] private Slider foregroundRatioSlider;
    [SerializeField] private Slider guidanceScaleValueSlider;
    [SerializeField] private Slider seedValueSlider;
    [SerializeField] private Slider textureResolutionSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to model events
        model.OnForegroundRatioChanged += view.UpdateForegroundRatioText;
        model.OnGuidanceScaleValueChanged += view.UpdateGuidanceScaleValueText;
        model.OnSeedValueChanged += view.UpdateSeedValueText;
        model.OnTextureResolutionChanged += view.UpdateTextureResolutionText;

        // Subscribe to model events for slider updates
        model.OnForegroundRatioChanged += view.UpdateForegroundRatioSlider;
        model.OnGuidanceScaleValueChanged += view.UpdateGuidanceScaleValueSlider;
        model.OnSeedValueChanged += view.UpdateSeedValueSlider;
        model.OnTextureResolutionChanged += view.UpdateTextureResolutionSlider;

        // Subscribe to slider events
        if (foregroundRatioSlider != null)
            foregroundRatioSlider.OnValueUpdated.AddListener(OnForegroundRatioSliderChanged);

        if (guidanceScaleValueSlider != null)
            guidanceScaleValueSlider.OnValueUpdated.AddListener(OnGuidanceScaleValueSliderChanged);

        if (seedValueSlider != null)
            seedValueSlider.OnValueUpdated.AddListener(OnSeedValueSliderChanged);

        if (textureResolutionSlider != null)
            textureResolutionSlider.OnValueUpdated.AddListener(OnTextureResolutionSliderChanged);
            
        // Subscribe to default button
        if (defaultButton != null)
            defaultButton.OnClicked.AddListener(OnDefaultButtonClicked);

        // Initialize with default values
        InitializeWithDefaults();
    }
    
    private void InitializeWithDefaults()
    {
        // Set model to default values, which will trigger UI updates
        model.ResetToDefaults();
    }

    private void OnDefaultButtonClicked()
    {
        // Reset model to defaults, which will update both sliders and text
        model.ResetToDefaults();
    }

    private void OnForegroundRatioSliderChanged(SliderEventData eventData)
    {
        model.ForegroundRatio = eventData.NewValue;
    }

    private void OnGuidanceScaleValueSliderChanged(SliderEventData eventData)
    {
        model.GuidanceScaleValue = Mathf.RoundToInt(eventData.NewValue);
    }

    private void OnSeedValueSliderChanged(SliderEventData eventData)
    {
        model.SeedValue = Mathf.RoundToInt(eventData.NewValue);
    }

    private void OnTextureResolutionSliderChanged(SliderEventData eventData)
    {
        model.TextureResolution = Mathf.RoundToInt(eventData.NewValue);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (model != null)
        {
            model.OnForegroundRatioChanged -= view.UpdateForegroundRatioText;
            model.OnGuidanceScaleValueChanged -= view.UpdateGuidanceScaleValueText;
            model.OnSeedValueChanged -= view.UpdateSeedValueText;
            model.OnTextureResolutionChanged -= view.UpdateTextureResolutionText;

            // Unsubscribe slider update events
            model.OnForegroundRatioChanged -= view.UpdateForegroundRatioSlider;
            model.OnGuidanceScaleValueChanged -= view.UpdateGuidanceScaleValueSlider;
            model.OnSeedValueChanged -= view.UpdateSeedValueSlider;
            model.OnTextureResolutionChanged -= view.UpdateTextureResolutionSlider;

        }

        if (defaultButton != null)
            defaultButton.OnClicked.RemoveListener(OnDefaultButtonClicked);
    }
}
