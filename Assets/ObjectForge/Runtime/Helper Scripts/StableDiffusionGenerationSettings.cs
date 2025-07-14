using UnityEngine;
using MixedReality.Toolkit.UX;
using TMPro;

public class StableDiffusionGenerationSettings : MonoBehaviour
{
    // [SerializeField] private MRTKUGUIInputField inputField;
    [SerializeField] private TextMeshProUGUI cfgValueText;
    [SerializeField] private TextMeshProUGUI seedValueText;


    // Method for Inspector Unity Events - receives SliderEventData
    public void OnSliderValueChanged(SliderEventData eventData)
    {
        UpdateCFGInputFieldText(eventData.NewValue);
    }

    public void UpdateCFGInputFieldText(float sliderValue)
    {
        // Debug.Log($"Slider value changed: {sliderValue}");
        if (cfgValueText != null)
        {
            int intValue = Mathf.RoundToInt(sliderValue);
            Debug.Log($"Slider value changed: {intValue}");
            // inputField.text = intValue.ToString();
            cfgValueText.text = intValue.ToString();
        }
    }

    public void OnSeedSliderValueChanged(SliderEventData eventData)
    {
        UpdateSeedInputFieldText(eventData.NewValue);
    }
    public void UpdateSeedInputFieldText(float sliderValue)
    {
        // Debug.Log($"Slider value changed: {sliderValue}");
        if (seedValueText != null)
        {
            int intValue = Mathf.RoundToInt(sliderValue);
            Debug.Log($"Slider value changed: {intValue}");
            seedValueText.text = intValue.ToString();
        }
    }
}
