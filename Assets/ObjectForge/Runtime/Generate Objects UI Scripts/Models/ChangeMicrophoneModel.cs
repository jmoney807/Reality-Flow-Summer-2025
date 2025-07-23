using MixedReality.Toolkit.UX;
using UnityEngine;
using System;

public class ChangeMicrophoneModel : MonoBehaviour
{
    public static ChangeMicrophoneModel Instance { get; private set; }

    [SerializeField] public GameObject MicrophoneListPanel;
    [SerializeField] public GameObject MicrophoneListContainer;
    [SerializeField] public GameObject MicrophoneOptionButtonPrefab;

    // [SerializeField] public PressableButton ChangeMicrophoneButton; // NOT SURE IF NEEDED

    // private bool isMicrophoneListVisible;
    // public bool IsMicrophoneListVisible
    // {
    //     get => isMicrophoneListVisible;
    //     set
    //     {
    //         isMicrophoneListVisible = value;
    //         // Invoke the method to show or hide the microphone list and populate it
    //     }
    // }

    public event Action OnSelectedMicrophoneChanged;

    private string selectedMicrophone;
    public string SelectedMicrophone
    {
        get => selectedMicrophone;
        set
        {
            selectedMicrophone = value;
            // Notify any listeners that the selected microphone has changed
            Debug.Log($"Selected microphone changed to: {selectedMicrophone}");
            // OnSelectedMicrophoneChanged?.Invoke();
        }
    }

    private GameObject currentSelectedButtonPrefab;
    public GameObject CurrentSelectedButtonPrefab
    {
        get => currentSelectedButtonPrefab;
        set
        {
            currentSelectedButtonPrefab = value;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
