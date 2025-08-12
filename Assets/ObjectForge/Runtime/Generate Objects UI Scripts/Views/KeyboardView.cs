using UnityEngine;
using MixedReality.Toolkit.UX;
using Unity.VisualScripting; // Assuming you are using Mixed Reality Toolkit for UI components
using MixedReality.Toolkit.UX.Experimental;

public class KeyboardView : MonoBehaviour
{
    [SerializeField] private GameObject nonNativeKeyboardPrefab;
    [SerializeField] private PressableButton toggleKeyboardButton;
    

    public void ShowNonNativeKeyboard()
    {
        nonNativeKeyboardPrefab.SetActive(true);
    }

    public void HideNonNativeKeyboard()
    {
        nonNativeKeyboardPrefab.SetActive(false);
    }

    public void ForceToggleKeyboardButton()
    {
        toggleKeyboardButton.ForceSetToggled(false);
    }

}
