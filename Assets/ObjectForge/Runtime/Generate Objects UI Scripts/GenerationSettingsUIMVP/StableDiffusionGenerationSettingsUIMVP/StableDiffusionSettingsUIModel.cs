using Unity.VisualScripting;
using UnityEngine;
using System;

public class StableDiffusionSettingsUIModel : MonoBehaviour
{
    public static StableDiffusionSettingsUIModel Instance { get; private set; }

    [Header("Default Values")]
    [SerializeField] private int defaultSeed = 0;
    [SerializeField] private int defaultCFGScale = 4;

    // Public properties to access default values
    public int DefaultSeed => defaultSeed;
    public int DefaultCFGScale => defaultCFGScale;

    // Events for notifying when values change
    public event Action<int> OnSeedChanged;
    public event Action<int> OnCFGScaleChanged;

    // Store the previous settings to allow resetting if they are different then the current values
    private int previousSeed;
    public int PreviousSeed
    {
        get => previousSeed;
        set
        {
            previousSeed = value;
        }
    }
    private int previousCFGScale;
    public int PreviousCFGScale
    {
        get => previousCFGScale;
        set
        {
            previousCFGScale = value;
        }
    }

    private bool isSettingsDifferent;
    public bool IsSettingsDifferent
    {
        get => isSettingsDifferent;
        set
        {
            isSettingsDifferent = value;
            if (isSettingsDifferent)
            {
                Debug.Log("Settings have been changed.");
            }
            else
            {
                Debug.Log("Settings have not been changed.");
            }
        }
    }


    private int seed;
    public int Seed
    {
        get { return seed; }
        set
        {
            seed = value;
            OnSeedChanged?.Invoke(seed);
        }
    }

    private int cfgScale;
    public int CFGScale
    {
        get { return cfgScale; }
        set
        {
            cfgScale = value;
            OnCFGScaleChanged?.Invoke(cfgScale);
        }
    }

    /// <summary>
    /// Resets all values to their defaults
    /// </summary>
    public void ResetToDefaults()
    {
        Seed = defaultSeed;
        CFGScale = defaultCFGScale;
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
