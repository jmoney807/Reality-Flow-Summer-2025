using UnityEngine;
using MixedReality.Toolkit.UX.Experimental;
using System.Collections;

public class KeyboardPresenter : MonoBehaviour
{
    public KeyboardModel KeyboardModel;
    [SerializeField] private KeyboardView keyboardView;
    [SerializeField] private NonNativeKeyboard nonNativeKeyboardComponent;

    private void OnEnable()
    {
        KeyboardModel.OnKeyboardIsActivated += HandleKeyboardActivated;
        KeyboardModel.OnKeyboardIsDeactivated += HandleKeyboardDeactivated;
    }

    private void OnDisable()
    {
        KeyboardModel.OnKeyboardIsActivated -= HandleKeyboardActivated;
        KeyboardModel.OnKeyboardIsDeactivated -= HandleKeyboardDeactivated;
    }


    public void ShowNonNativeKeyboard()
    {
        KeyboardModel.IsKeyboardVisible = true;
    }

    public void HideNonNativeKeyboard()
    {
        KeyboardModel.IsKeyboardVisible = false;
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
        Debug.Log("Keyboard text submitted: " + submittedText);

        nonNativeKeyboardComponent.OnTextUpdate.RemoveListener(OnKeyboardTextUpdated); // Temporary fix to prevent resetting transcription result but ideally this should be done in its own method after ForceToggleKeyboardButton is called.

        // Set the final transcription result when Enter is pressed
        if (TranscriptionUIModel.Instance != null)
        {
            Debug.Log("Changing transcription result to: " + submittedText);
            TranscriptionUIModel.Instance.TranscriptionResult = submittedText;
            TranscriptionUIModel.Instance.IsFinalResult = true;
        }

        // Force untoggle of the keyboard button (THIS IS/was CAUSING A BUG THAT RESETS THE TRANSCRIPTION RESULT AFTER )
        keyboardView.ForceToggleKeyboardButton();
    }

    private void OnKeyboardClosed(string currentText)
    {
        nonNativeKeyboardComponent.OnTextUpdate.RemoveListener(OnKeyboardTextUpdated); // Temporary fix to prevent resetting transcription result but ideally this should be done in its own method after ForceToggleKeyboardButton is called.

        // Update the transcription result with each character typed
        if (TranscriptionUIModel.Instance != null)
        {
            Debug.Log("Keyboard closed with text: " + currentText);
            // If the string is not empty, set it as the transcription result (I.e., editing). Otherwise do nothing 
            if (!string.IsNullOrEmpty(currentText))
            {
                TranscriptionUIModel.Instance.TranscriptionResult = currentText;
                TranscriptionUIModel.Instance.IsFinalResult = true; // Still typing
            }
        }
        keyboardView.ForceToggleKeyboardButton();
    }

    public void HandleKeyboardActivated()
    {
        keyboardView.ShowNonNativeKeyboard();

        StartCoroutine(SubscribeAfterActivation());
    }

    private IEnumerator SubscribeAfterActivation()
    {
        yield return new WaitForEndOfFrame();

        SubscribeToKeyboardEvents();
    }

    private void SubscribeToKeyboardEvents()
    {
        if (nonNativeKeyboardComponent != null)
        {
            nonNativeKeyboardComponent.OnTextSubmit.AddListener(OnKeyboardTextSubmitted);
            nonNativeKeyboardComponent.OnTextUpdate.AddListener(OnKeyboardTextUpdated);
            nonNativeKeyboardComponent.OnClose.AddListener(OnKeyboardClosed);
        }
    }

    public void HandleKeyboardDeactivated()
    {
        // This must come before the keyboard is hidden to ensure the OnKeyboardTextUpdated is not called with an empty string (This resolves the bug where the transcription result is reset after hiding the keyboard)
        UnsubscribeFromKeyboardEvents();

        keyboardView.HideNonNativeKeyboard();
    }

    private void UnsubscribeFromKeyboardEvents()
    {
        Debug.Log("Unsubscribing from keyboard events");
        if (nonNativeKeyboardComponent != null)
        {
            nonNativeKeyboardComponent.OnTextSubmit.RemoveListener(OnKeyboardTextSubmitted);
            nonNativeKeyboardComponent.OnTextUpdate.RemoveListener(OnKeyboardTextUpdated);
            nonNativeKeyboardComponent.OnClose.RemoveListener(OnKeyboardClosed);
        }
    }
}
