using UnityEngine;
using System;

public class TranscriptionUIModel : MonoBehaviour
{
    public static TranscriptionUIModel Instance { get; private set; }

    public static event Action<string> OnTranscriptionResultChanged;

    private string transcriptionResult;
    public string TranscriptionResult
    {
        get => transcriptionResult;
        set
        {
            transcriptionResult = value;
            // Notify listeners about the change
            OnTranscriptionResultChanged?.Invoke(transcriptionResult);
        }
    }

    public static event Action<bool> OnIsFinalResultChanged;

    private bool isFinalResult;
    public bool IsFinalResult
    {
        get => isFinalResult;
        set
        {
            isFinalResult = value;
            // Notify listeners about the final result state change
            OnIsFinalResultChanged?.Invoke(isFinalResult);
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

    public void ResetTranscriptionUIModelInstance()
    {
        TranscriptionResult = string.Empty;
        IsFinalResult = false;
    }
}
