using UnityEngine;
using System;

public class SPAR3DSettingsModel : MonoBehaviour
{
    public static SPAR3DSettingsModel Instance { get; private set; }

    [Header("Default Values")]
    [SerializeField] private float defaultForegroundRatio = 1.3f;
    [SerializeField] private int defaultGuidanceScaleValue = 3;
    [SerializeField] private int defaultSeedValue = 0;
    [SerializeField] private int defaultTextureResolution = 1024;

    // Variables to store previous values used in last generation
    private float previousForegroundRatio;
    public float PreviousForegroundRatio
    {
        get => previousForegroundRatio;
        set => previousForegroundRatio = value;
    }
    private int previousGuidanceScaleValue;
    public int PreviousGuidanceScaleValue
    {
        get => previousGuidanceScaleValue;
        set => previousGuidanceScaleValue = value;
    }
    private int previousSeedValue;
    public int PreviousSeedValue
    {
        get => previousSeedValue;
        set => previousSeedValue = value;
    }
    private int previousTextureResolution;
    public int PreviousTextureResolution
    {
        get => previousTextureResolution;
        set => previousTextureResolution = value;
    }

    // Public properties to access default values
    public float DefaultForegroundRatio => defaultForegroundRatio;
    public int DefaultGuidanceScaleValue => defaultGuidanceScaleValue;
    public int DefaultSeedValue => defaultSeedValue;
    public int DefaultTextureResolution => defaultTextureResolution;


    // Events for notifying when values change
    public event Action<float> OnForegroundRatioChanged;
    public event Action<int> OnGuidanceScaleValueChanged;
    public event Action<int> OnSeedValueChanged;
    public event Action<int> OnTextureResolutionChanged;

    private float foregroundRatio;
    public float ForegroundRatio
    {
        get { return foregroundRatio; }
        set
        {
            foregroundRatio = value;

            Debug.Log($"Foreground Ratio set to: {foregroundRatio}");
            OnForegroundRatioChanged?.Invoke(foregroundRatio);

        }
    }

    private int guidanceScaleValue;
    public int GuidanceScaleValue
    {
        get { return guidanceScaleValue; }
        set
        {
            guidanceScaleValue = value;
            Debug.Log($"Guidance Scale Value set to: {guidanceScaleValue}");
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
        ForegroundRatio = defaultForegroundRatio;
        GuidanceScaleValue = defaultGuidanceScaleValue;
        SeedValue = defaultSeedValue;
        TextureResolution = defaultTextureResolution;
    }
    
    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            ResetToDefaults(); // Initialize with default values
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
