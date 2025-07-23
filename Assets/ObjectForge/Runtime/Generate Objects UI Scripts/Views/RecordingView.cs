using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;
using System;

public class RecordingView : MonoBehaviour
{
    [SerializeField] private PressableButton recordButton;
    [SerializeField] private TextMeshProUGUI recordIcon;
    [SerializeField] private FontIconSelector recordFontIconSelector;
    [SerializeField] private TextMeshProUGUI recordingText;

    public void UpdateRecordingIcon(Color color, string iconName)
    {
        if (recordIcon != null)
        {
            recordIcon.colorGradient = new VertexGradient(color);
        }

        if (recordFontIconSelector != null)
        {
            recordFontIconSelector.CurrentIconName = iconName;
        }
    }

    public void UpdateRecordingText(string text)
    {
        if (recordingText != null)
        {
            recordingText.text = text;
        }
    }

    public void SetButtonToggleState(bool isToggled)
    {
        if (recordButton != null)
        {
            recordButton.ForceSetToggled(isToggled);
        }
    }

    public void FlashRecordingIcon(bool isVisible)
    {
        if (recordIcon != null)
        {
            if (isVisible)
            {
                recordIcon.colorGradient = new VertexGradient(Color.red);
            }
            else
            {
                Color dimmedRed = new Color(0.5f, 0, 0, 0.5f);
                recordIcon.colorGradient = new VertexGradient(dimmedRed);
            }
        }
    }
}
