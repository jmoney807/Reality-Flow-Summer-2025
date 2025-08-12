using System;
using UnityEngine;

public class GenerationSettingsPresenter : MonoBehaviour
{
    public GenerationSettingsView View;
    public GenerationSettingsModel Model;

    // Make Enum for the two buttons
    public enum SettingsType
    {
        StableDiffusion,
        SPAR3D
    }

    // Assign to button click events in the Unity Inspector
    public void ShowSettings(int settingsType)
    {
        switch (settingsType)
        {
            case (int)SettingsType.StableDiffusion:
                View.ShowStableDiffusionSettings();
                View.SPAR3DSettingsButton.ForceSetToggled(false);
                break;
            case (int)SettingsType.SPAR3D:
                View.ShowSPAR3DSettings();
                View.StableDiffusionSettingsButton.ForceSetToggled(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(settingsType), settingsType, null);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
