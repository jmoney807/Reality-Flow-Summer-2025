using UnityEngine;
using System.Collections;
using GenerativeAI.Whisper;
using System;

public class RecordingPresenter : MonoBehaviour
{
    [SerializeField] private RecordingView recordingView;
    [SerializeField] private float recordingDuration = 5f;
    [SerializeField] private float flashInterval = 0.5f;

    private RecordingModel recordingModel;
    private Coroutine countdownCoroutine;
    private Coroutine flashCoroutine;

    public static event Action OnRecordingStarted;
    public static void RequestStartRecording() => OnRecordingStarted?.Invoke();

    private void Awake()
    {
        recordingModel = new RecordingModel();
    }

    private void OnEnable()
    {
        // Subscribe to events
        OnRecordingStarted += ToggleRecording;
        recordingModel.OnRecordingStateChanged += OnRecordingStateChanged;
        recordingModel.OnTimeRemainingChanged += OnTimeRemainingChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        OnRecordingStarted -= ToggleRecording;
        recordingModel.OnRecordingStateChanged -= OnRecordingStateChanged;
        recordingModel.OnTimeRemainingChanged -= OnTimeRemainingChanged;
        
    }

    private void Update()
    {
        if (recordingModel.IsRecording)
        {
            recordingModel.UpdateTimeRemaining(Time.deltaTime);
        }
    }

    public void ToggleRecording()
    {
        // // Create an instance of RecordingModel if it doesn't exist or if it does, clear it
        // if (recordingModel == null)
        // {
        //     recordingModel = new RecordingModel();
        // }
        
        if (recordingModel.IsRecording)
        {
            Debug.Log("Stopping recording...");
            StopRecording();
        }
        else
        {
            Debug.Log("Starting recording...");
            StartRecording();
        }
    }

    private void StartRecording()
    {
        recordingModel.StartRecording(recordingDuration);
        Whisper.RequestRecordingStart();
    }

    private void StopRecording()
    {
        recordingModel.StopRecording();
        Whisper.RequestRecordingEnd();
        
        // Stop all coroutines
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
    }

    private void OnRecordingStateChanged(bool isRecording)
    {
        if (isRecording)
        {
            recordingView.UpdateRecordingIcon(Color.red, "Icon 135");
            countdownCoroutine = StartCoroutine(CountdownTimer());
            flashCoroutine = StartCoroutine(FlashIcon());
        }
        else
        {
            recordingView.UpdateRecordingIcon(Color.white, "Icon 128");
            recordingView.UpdateRecordingText("<size=8>Record</size><size=6>\n<alpha=#88>Describe the object you want to generate</size>");
            // recordingView.SetButtonToggleState(false);
        }
    }

    private void OnTimeRemainingChanged(float timeRemaining)
    {
        string text = $"<size=8>Recording</size><size=6>\n<alpha=#88>Time Remaining: {timeRemaining:F1}s</size>";
        recordingView.UpdateRecordingText(text);
    }

    private IEnumerator CountdownTimer()
    {
        while (recordingModel.IsRecording && recordingModel.TimeRemaining > 0)
        {
            yield return null;
        }
        // Show the IsRecording flag
        Debug.Log("The IsRecording flag is " + recordingModel.IsRecording);
        recordingView.SetButtonToggleState(false);
    }

    private IEnumerator FlashIcon()
    {
        bool isVisible = true;
        
        while (recordingModel.IsRecording)
        {
            recordingView.FlashRecordingIcon(isVisible);
            isVisible = !isVisible;
            yield return new WaitForSeconds(flashInterval);
        }
    }
}
