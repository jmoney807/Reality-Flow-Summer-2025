using UnityEngine;
using System;
using Unity.VisualScripting;

public class RecordingModel : MonoBehaviour
{
    public bool IsRecording { get; private set; }
    public float RecordingDuration { get; private set; }
    public float TimeRemaining { get; private set; }

    public event Action<bool> OnRecordingStateChanged;
    public event Action<float> OnTimeRemainingChanged;

    public void StartRecording(float duration)
    {
        if (IsRecording) return;

        IsRecording = true;
        RecordingDuration = duration;
        TimeRemaining = duration;
        
        OnRecordingStateChanged?.Invoke(IsRecording);
    }

    public void StopRecording()
    {
        if (!IsRecording) return;

        IsRecording = false;
        TimeRemaining = 0f;
        
        OnRecordingStateChanged?.Invoke(IsRecording);
    }

    public void UpdateTimeRemaining(float deltaTime)
    {
        if (!IsRecording) return;

        TimeRemaining -= deltaTime;
        OnTimeRemainingChanged?.Invoke(TimeRemaining);


        // This causes a bug where the IsRecording Flag is set to false,.
        // Therefore when the force toggle is called when the timer runs out, the recording starts again instead of stopping.
        // if (TimeRemaining <= 0f)
        // {
        //     StopRecording();
        // }
    }

    // // Make a constructor to initialize the model
    // public RecordingModel()
    // {
    //     IsRecording = false;
    //     RecordingDuration = 0f;
    //     TimeRemaining = 0f;
    // }
}
