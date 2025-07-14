using UnityEngine;

public class KeyboardView : MonoBehaviour
{
    [SerializeField] private GameObject nonNativeKeyboardPrefab;


    public void ShowNonNativeKeyboard()
    {
        nonNativeKeyboardPrefab.SetActive(true);
    }

    public void HideNonNativeKeyboard()
    {
        nonNativeKeyboardPrefab.SetActive(false);
    }
}
