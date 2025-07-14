using UnityEngine;
using TMPro;

public class TranscriptionUIView : MonoBehaviour
{
    [SerializeField] public GameObject TranscriptionUI;
    [SerializeField] public TextMeshProUGUI TranscriptionTextComponent;
    [SerializeField] public GameObject TranscriptionVerticalButtonBar;

    public void Start()
    {
        TranscriptionUI.SetActive(false);
        TranscriptionVerticalButtonBar.SetActive(false);
    }

    public void ShowTranscriptionUI(string transcriptionResult)
    {
        TranscriptionUI.SetActive(true);
        TranscriptionTextComponent.text = transcriptionResult;
    }

    public void HideTranscriptionUI()
    {
        TranscriptionUI.SetActive(false);
        TranscriptionTextComponent.text = string.Empty;
    }

    public void ShowTranscriptionButtons()
    {
        TranscriptionVerticalButtonBar.SetActive(true);
    }

    public void HideTranscriptionButtons()
    {
        TranscriptionVerticalButtonBar.SetActive(false);
    }
    
    
}
