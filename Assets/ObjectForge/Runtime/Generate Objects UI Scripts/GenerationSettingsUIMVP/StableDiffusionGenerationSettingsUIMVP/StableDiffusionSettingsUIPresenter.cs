using UnityEngine;
using MixedReality.Toolkit.UX;

public class StableDiffusionSettingsUIPresenter : MonoBehaviour
{
    [Header("MVP Components")]
    [SerializeField] private StableDiffusionSettingsUIModel model;
    [SerializeField] private StableDiffusionSettingsUIView view;

    [Header("Default Button")]
    [SerializeField] private PressableButton defaultButton;

    [Header("Sliders")]
    [SerializeField] private Slider seedValueSlider;
    [SerializeField] private Slider cfgScaleSlider;

    void Start()
    {
        // Subscribe to model text update events
        model.OnSeedChanged += view.UpdateSeedValueText;
        model.OnCFGScaleChanged += view.UpdateCFGScaleValueText;

        // Subscribe to model events for slider updates
        model.OnSeedChanged += view.UpdateSeedValueSlider;
        model.OnCFGScaleChanged += view.UpdateCFGScaleValueSlider;

        // Subscribe to slider events
        if (seedValueSlider != null)
            seedValueSlider.OnValueUpdated.AddListener(OnSeedValueSliderChanged);

        if (cfgScaleSlider != null)
            cfgScaleSlider.OnValueUpdated.AddListener(OnCFGScaleSliderChanged);

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

    private void OnSeedValueSliderChanged(SliderEventData eventData)
    {
        model.Seed = Mathf.RoundToInt(eventData.NewValue);
    }

    private void OnCFGScaleSliderChanged(SliderEventData eventData)
    {
        model.CFGScale = Mathf.RoundToInt(eventData.NewValue);
    }

    private void OnDefaultButtonClicked()
    {
        // Reset model to defaults, which will update both sliders and text
        model.ResetToDefaults();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (model != null)
        {
            model.OnSeedChanged -= view.UpdateSeedValueText;
            model.OnCFGScaleChanged -= view.UpdateCFGScaleValueText;

            // Unsubscribe slider update events
            model.OnSeedChanged -= view.UpdateSeedValueSlider;
            model.OnCFGScaleChanged -= view.UpdateCFGScaleValueSlider;
        }

        if (defaultButton != null)
            defaultButton.OnClicked.RemoveListener(OnDefaultButtonClicked);
    }


}
