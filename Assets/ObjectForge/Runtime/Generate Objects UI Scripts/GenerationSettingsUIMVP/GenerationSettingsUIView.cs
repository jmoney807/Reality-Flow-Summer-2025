using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using MixedReality.Toolkit.UX;

public class GenerationSettingsUIView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI paddingRatioText;
    [SerializeField] private TextMeshProUGUI guidanceScaleValueText;
    [SerializeField] private TextMeshProUGUI seedValueText;
    [SerializeField] private TextMeshProUGUI textureResolutionText;

    [Header("Sliders")]
    [SerializeField] private Slider paddingRatioSlider;
    [SerializeField] private Slider guidanceScaleValueSlider;
    [SerializeField] private Slider seedValueSlider;
    [SerializeField] private Slider textureResolutionSlider;

    public void UpdatePaddingRatioText(float value)
    {
        if (paddingRatioText != null)
            paddingRatioText.text = value.ToString("F1"); //1 decimal place
    }

    public void UpdateGuidanceScaleValueText(int value)
    {
        if (guidanceScaleValueText != null)
            guidanceScaleValueText.text = value.ToString();
    }

    public void UpdateSeedValueText(int value)
    {
        if (seedValueText != null)
            seedValueText.text = value.ToString();
    }

    public void UpdateTextureResolutionText(int value)
    {
        if (textureResolutionText != null)
            textureResolutionText.text = value.ToString();
    }

    // Methods to update sliders when model values change
    public void UpdatePaddingRatioSlider(float value)
    {
        if (paddingRatioSlider != null)
            paddingRatioSlider.Value = value;
    }

    public void UpdateGuidanceScaleValueSlider(int value)
    {
        if (guidanceScaleValueSlider != null)
            guidanceScaleValueSlider.Value = value;
    }

    public void UpdateSeedValueSlider(int value)
    {
        if (seedValueSlider != null)
            seedValueSlider.Value = value;
    }

    public void UpdateTextureResolutionSlider(int value)
    {
        if (textureResolutionSlider != null)
            textureResolutionSlider.Value = value;
    }



    // public void UpdateCfgValueText(float value)
    // {
    //     if (cfgValueText != null)
    //         cfgValueText.text = value.ToString("F2"); // 2 decimal places
    // }
}
