using UnityEngine;
using MixedReality.Toolkit.UX;

public class GenerationSettingsView : MonoBehaviour
{
    [Header("Settings Buttons")]
    public PressableButton StableDiffusionSettingsButton;
    public PressableButton SPAR3DSettingsButton;

    [Header("Settings Panels")]
    public GameObject StableDiffusionSettingsPanel;
    public GameObject SPAR3DSettingsPanel;

    public void ShowStableDiffusionSettings()
    {
        StableDiffusionSettingsPanel.SetActive(true);
        SPAR3DSettingsPanel.SetActive(false);
    }

    public void ShowSPAR3DSettings()
    {
        StableDiffusionSettingsPanel.SetActive(false);
        SPAR3DSettingsPanel.SetActive(true);
    }

}
