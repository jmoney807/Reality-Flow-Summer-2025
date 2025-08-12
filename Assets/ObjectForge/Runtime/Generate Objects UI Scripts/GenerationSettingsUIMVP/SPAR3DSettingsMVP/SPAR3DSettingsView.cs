using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using MixedReality.Toolkit.UX;

public class SPAR3DSettingsView : MonoBehaviour
{
    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI foregroundRatioText;
    [SerializeField] private TextMeshProUGUI guidanceScaleValueText;
    [SerializeField] private TextMeshProUGUI seedValueText;
    [SerializeField] private TextMeshProUGUI textureResolutionText;

    [Header("Sliders")]
    [SerializeField] private Slider foregroundRatioSlider;
    [SerializeField] private Slider guidanceScaleValueSlider;
    [SerializeField] private Slider seedValueSlider;
    [SerializeField] private Slider textureResolutionSlider;

    public void UpdateForegroundRatioText(float value)
    {
        if (foregroundRatioText != null)
            foregroundRatioText.text = value.ToString("F1"); //1 decimal place
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
    public void UpdateForegroundRatioSlider(float value)
    {
        if (foregroundRatioSlider != null)
            foregroundRatioSlider.Value = value;
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
}
