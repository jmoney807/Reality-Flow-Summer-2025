using UnityEngine;
using MixedReality.Toolkit.UX;
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
