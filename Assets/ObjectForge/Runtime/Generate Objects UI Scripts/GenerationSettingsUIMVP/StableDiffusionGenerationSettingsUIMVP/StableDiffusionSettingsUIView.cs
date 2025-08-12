using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;

public class StableDiffusionSettingsUIView : MonoBehaviour
{
    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI seedValueText;
    [SerializeField] private TextMeshProUGUI cfgScaleValueText;

    [Header("Sliders")]
    [SerializeField] private Slider seedValueSlider;
    [SerializeField] private Slider cfgScaleSlider;

    public void UpdateSeedValueText(int value)
    {
        if (seedValueText != null)
            seedValueText.text = value.ToString();
    }
    public void UpdateSeedValueSlider(int value)
    {
        if (seedValueSlider != null)
            seedValueSlider.Value = value;
    }

    public void UpdateCFGScaleValueText(int value)
    {
        if (cfgScaleValueText != null)
            cfgScaleValueText.text = value.ToString();
    }
    public void UpdateCFGScaleValueSlider(int value)
    {
        if (cfgScaleSlider != null)
            cfgScaleSlider.Value = value;
    }

}
