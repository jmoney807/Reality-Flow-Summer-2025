using UnityEngine;
using System;

public class ImageGenerationUIModel : MonoBehaviour
{
    public static ImageGenerationUIModel Instance { get; private set; }

    private byte[] rembgResult;
    public byte[] RembgResult
    {
        get => rembgResult;
        set
        {
            rembgResult = value;
            // Notify listeners about the change if needed
        }
    }

    private byte[] stableDiffusionResult;
    public byte[] StableDiffusionResult
    {
        get => stableDiffusionResult;
        set
        {
            stableDiffusionResult = value;
            // Notify listeners about the change if needed
        }
    }

    public static event Action OnImageGenerationStarted;
    public static event Action OnImageGenerationCompleted;

    private bool isGeneratingImage;
    public bool IsGeneratingImage
    {
        get => isGeneratingImage;
        set
        {
            isGeneratingImage = value;
            // Notify listeners about the change if needed
            if (isGeneratingImage)
            {
                OnImageGenerationStarted?.Invoke();
            }
            else
            {
                OnImageGenerationCompleted?.Invoke();
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetImageGenerationUIModelInstance()
    {
        RembgResult = null;
        StableDiffusionResult = null;
        IsGeneratingImage = false;
    }
}
