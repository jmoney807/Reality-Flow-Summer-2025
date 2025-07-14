using UnityEngine;
using MixedReality.Toolkit.UX;
using System;


public class GenerationSettingsUIPresenter : MonoBehaviour
{
    [Header("MVP Components")]
    [SerializeField] private GenerationSettingsUIModel model;
    [SerializeField] private GenerationSettingsUIView view;

    [Header("Default Button")]
    [SerializeField] private PressableButton defaultButton;

    [Header("Sliders")]
    [SerializeField] private Slider paddingRatioSlider;
    [SerializeField] private Slider guidanceScaleValueSlider;
    [SerializeField] private Slider seedValueSlider;
    [SerializeField] private Slider textureResolutionSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to model events
        model.OnPaddingRatioChanged += view.UpdatePaddingRatioText;
        model.OnGuidanceScaleValueChanged += view.UpdateGuidanceScaleValueText;
        model.OnSeedValueChanged += view.UpdateSeedValueText;
        model.OnTextureResolutionChanged += view.UpdateTextureResolutionText;

        // Subscribe to model events for slider updates
        model.OnPaddingRatioChanged += view.UpdatePaddingRatioSlider;
        model.OnGuidanceScaleValueChanged += view.UpdateGuidanceScaleValueSlider;
        model.OnSeedValueChanged += view.UpdateSeedValueSlider;
        model.OnTextureResolutionChanged += view.UpdateTextureResolutionSlider;

        // Subscribe to slider events
        if (paddingRatioSlider != null)
            paddingRatioSlider.OnValueUpdated.AddListener(OnPaddingRatioSliderChanged);

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

        // Initialize with current values
        // InitializeValues();
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

    private void InitializeValues()
    {
        // Set initial model values (these will trigger the UI updates)
        if (paddingRatioSlider != null)
            // model.PaddingRatio = Mathf.RoundToInt(paddingRatioSlider.Value);
            model.PaddingRatio = paddingRatioSlider.Value;

        if (guidanceScaleValueSlider != null)
            model.GuidanceScaleValue = Mathf.RoundToInt(guidanceScaleValueSlider.Value);

        if (seedValueSlider != null)
            model.SeedValue = Mathf.RoundToInt(seedValueSlider.Value);

        if (textureResolutionSlider != null)
            model.TextureResolution = Mathf.RoundToInt(textureResolutionSlider.Value);
    }

    private void OnPaddingRatioSliderChanged(SliderEventData eventData)
    {
        model.PaddingRatio = eventData.NewValue;
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
            model.OnPaddingRatioChanged -= view.UpdatePaddingRatioText;
            model.OnGuidanceScaleValueChanged -= view.UpdateGuidanceScaleValueText;
            model.OnSeedValueChanged -= view.UpdateSeedValueText;
            model.OnTextureResolutionChanged -= view.UpdateTextureResolutionText;

            // Unsubscribe slider update events
            model.OnPaddingRatioChanged -= view.UpdatePaddingRatioSlider;
            model.OnGuidanceScaleValueChanged -= view.UpdateGuidanceScaleValueSlider;
            model.OnSeedValueChanged -= view.UpdateSeedValueSlider;
            model.OnTextureResolutionChanged -= view.UpdateTextureResolutionSlider;

        }

        if (defaultButton != null)
            defaultButton.OnClicked.RemoveListener(OnDefaultButtonClicked);
    }
}
