using System;
using UnityEngine;

public class KeyboardModel : MonoBehaviour
{
    public event Action OnKeyboardIsActivated;
    public event Action OnKeyboardIsDeactivated;
    private bool isKeyboardVisible = false;
    public bool IsKeyboardVisible
    {
        get { return isKeyboardVisible; }
        set
        {
            isKeyboardVisible = value;
            if (isKeyboardVisible)
            {
                OnKeyboardIsActivated?.Invoke();
            }
            else
            {
                OnKeyboardIsDeactivated?.Invoke();
            }
        }
    }
}
