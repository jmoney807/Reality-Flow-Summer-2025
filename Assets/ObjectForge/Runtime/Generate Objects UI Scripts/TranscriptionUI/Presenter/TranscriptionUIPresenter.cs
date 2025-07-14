using UnityEngine;
using MixedReality.Toolkit.UX.Experimental;

public class TranscriptionUIPresenter : MonoBehaviour
{
    [SerializeField] private TranscriptionUIView transcriptionUIView;
    [SerializeField] private ImageGeneration imageGeneration;
    [SerializeField] private KeyboardPresenter keyboardPresenter; 


    public void OnEnable()
    {
        TranscriptionUIModel.OnTranscriptionResultChanged += UpdateTranscriptionResult;

        TranscriptionUIModel.OnIsFinalResultChanged += UpdateIsFinalResult;
    }

    public void OnDisable()
    {
        TranscriptionUIModel.OnTranscriptionResultChanged -= UpdateTranscriptionResult;

        TranscriptionUIModel.OnIsFinalResultChanged -= UpdateIsFinalResult;
    }

    private void UpdateTranscriptionResult(string newTranscriptionResult)
    {
        // Update the UI with the new transcription result
        Debug.Log("Transcription result updated: " + newTranscriptionResult);

        transcriptionUIView.ShowTranscriptionUI(newTranscriptionResult);
    }

    private void UpdateIsFinalResult(bool isFinal)
    {
        // Update the UI based on whether the result is final
        Debug.Log("Is final result updated: " + isFinal);
        if (isFinal)
        {
            transcriptionUIView.ShowTranscriptionButtons();
        }
        else
        {
            transcriptionUIView.HideTranscriptionButtons();
        }
    }

    public void ConfirmTranscriptionResult()
    {
        Debug.Log("Transcription result confirmed.");

        imageGeneration.GenerateImage();

        transcriptionUIView.HideTranscriptionButtons();
    }

    public void RejectTranscriptionResult()
    {
        Debug.Log("Transcription result rejected.");

        TranscriptionUIModel.Instance.ResetTranscriptionUIModelInstance();
        transcriptionUIView.HideTranscriptionUI();
    }

    public void EditTranscriptionResult()
    {
         Debug.Log("Editing transcription result.");

        // Get the current transcription text
        string currentText = TranscriptionUIModel.Instance.TranscriptionResult;
        
        // Show the keyboard with the current text for editing
        if (NonNativeKeyboard.Instance != null)
        {
            NonNativeKeyboard.Instance.Open(currentText, NonNativeKeyboard.LayoutType.Alpha);
        }
        
        // Show the keyboard through your presenter if you have one
        if (keyboardPresenter != null)
        {
            keyboardPresenter.ShowNonNativeKeyboard();
        }
        
        // Set the transcription as not final since we're editing
        TranscriptionUIModel.Instance.IsFinalResult = false;
        
        // Hide the transcription buttons while editing
        transcriptionUIView.HideTranscriptionButtons();
    }
}
