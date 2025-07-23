using UnityEngine;
using MixedReality.Toolkit.UX.Experimental;

public class KeyboardPresenter : MonoBehaviour
{
    [SerializeField] private KeyboardView keyboardView;

    private void OnEnable()
    {
        // Subscribe to keyboard events
        if (NonNativeKeyboard.Instance != null)
        {
            NonNativeKeyboard.Instance.OnTextSubmit.AddListener(OnKeyboardTextSubmitted);
            NonNativeKeyboard.Instance.OnTextUpdate.AddListener(OnKeyboardTextUpdated);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from keyboard events
        if (NonNativeKeyboard.Instance != null)
        {
            NonNativeKeyboard.Instance.OnTextSubmit.RemoveListener(OnKeyboardTextSubmitted);
            NonNativeKeyboard.Instance.OnTextUpdate.RemoveListener(OnKeyboardTextUpdated);
        }
    }

    private void OnKeyboardTextUpdated(string currentText)
    {
        // Update the transcription result with each character typed
        if (TranscriptionUIModel.Instance != null)
        {
            TranscriptionUIModel.Instance.TranscriptionResult = currentText;
            TranscriptionUIModel.Instance.IsFinalResult = false; // Still typing
        }
        
        Debug.Log("Keyboard text updated: " + currentText);
    }

    private void OnKeyboardTextSubmitted(string submittedText)
    {
        // Set the final transcription result when Enter is pressed
        if (TranscriptionUIModel.Instance != null)
        {
            TranscriptionUIModel.Instance.TranscriptionResult = submittedText;
            TranscriptionUIModel.Instance.IsFinalResult = true;
        }

        Debug.Log("Keyboard text submitted: " + submittedText);

        // Optionally hide the keyboard after submission
        //HideNonNativeKeyboard();
        // Force untoggle of the keyboard button
        keyboardView.ForceToggleKeyboardButton();
    }

    public void ShowNonNativeKeyboard()
    {
        keyboardView.ShowNonNativeKeyboard();
        
        // Re-subscribe in case the keyboard instance changed
        if (NonNativeKeyboard.Instance != null)
        {
            NonNativeKeyboard.Instance.OnTextSubmit.AddListener(OnKeyboardTextSubmitted);
            NonNativeKeyboard.Instance.OnTextUpdate.AddListener(OnKeyboardTextUpdated);
        }
    }
    
    public void HideNonNativeKeyboard()
    {
        keyboardView.HideNonNativeKeyboard();
    }
}
