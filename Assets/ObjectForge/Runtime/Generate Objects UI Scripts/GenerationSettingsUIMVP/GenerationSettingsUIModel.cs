using UnityEngine;
using System;

public class GenerationSettingsUIModel : MonoBehaviour
{
    [Header("Default Values")]
    [SerializeField] private float defaultPaddingRatio = 1.3f;
    [SerializeField] private int defaultGuidanceScaleValue = 3;
    [SerializeField] private int defaultSeedValue = 0;
    [SerializeField] private int defaultTextureResolution = 1024;

    // Public properties to access default values
    public float DefaultPaddingRatio => defaultPaddingRatio;
    public int DefaultGuidanceScaleValue => defaultGuidanceScaleValue;
    public int DefaultSeedValue => defaultSeedValue;
    public int DefaultTextureResolution => defaultTextureResolution;


    // Events for notifying when values change
    public event Action<float> OnPaddingRatioChanged;
    public event Action<int> OnGuidanceScaleValueChanged;
    public event Action<int> OnSeedValueChanged;
    public event Action<int> OnTextureResolutionChanged;

    private float paddingRatio;
    public float PaddingRatio
    {
        get { return paddingRatio; }
        set
        {
            paddingRatio = value;
            OnPaddingRatioChanged?.Invoke(paddingRatio);
        }
    }

    private int guidanceScaleValue;
    public int GuidanceScaleValue
    {
        get { return guidanceScaleValue; }
        set
        {
            guidanceScaleValue = value;
            OnGuidanceScaleValueChanged?.Invoke(guidanceScaleValue);
        }
    }

    private int seedValue;
    public int SeedValue
    {
        get { return seedValue; }
        set
        {
            seedValue = value;
            OnSeedValueChanged?.Invoke(seedValue);
        }
    }

    private int textureResolution;
    public int TextureResolution
    {
        get { return textureResolution; }
        set
        {
            textureResolution = value;
            OnTextureResolutionChanged?.Invoke(textureResolution);
        }
    }
    
    /// <summary>
    /// Resets all values to their defaults
    /// </summary>
    public void ResetToDefaults()
    {
        PaddingRatio = defaultPaddingRatio;
        GuidanceScaleValue = defaultGuidanceScaleValue;
        SeedValue = defaultSeedValue;
        TextureResolution = defaultTextureResolution;
    }
}
