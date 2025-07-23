using UnityEngine;
using System;
using Unity.VisualScripting;
using System.IO;
using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoadPremadesModel : MonoBehaviour
{
    // Singleton instance
    public static LoadPremadesModel Instance { get; private set; }

    // [SerializeField] private UpdateLoadPremadesUI updateLoadPremadesUI;
    [SerializeField] public GameObject buttonPrefab;
    [SerializeField] public Transform buttonContainer;
    [SerializeField] public GameObject premadeCanvas;
    [SerializeField] public PressableButton[] premadeDetailsButtons;
    [SerializeField] public PressableButton[] stateButtons;

    [SerializeField] public string PremadesBaseDirectory = "Premades/All Premades"; // Base directory for all premades

    public bool isFavoritesStateActive = false; // USE THIS TO TRACK IF THE FAVORITES STATE IS ACTIVE INSTEAD OF IN LOADPREMADESPRESENTER
    public bool isRecycledStateActive = false; // USE THIS TO TRACK IF THE RECYCLE BIN STATE IS ACTIVE INSTEAD OF IN LOADPREMADESPRESENTER


    // Use a list to store the premades in each directory
    public List<string> allPremades = new List<string>();
    public List<string> favoritesPremades = new List<string>();
    public List<string> recycleBinPremades = new List<string>();


    public event Action OnCurrentStateChanged;

    public IState currentState;
    public IState CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;
            Debug.Log($"Current state changed to: {currentState.GetType().Name}, Invoking OnCurrentStateChanged");
            // Notify listeners that the state has changed
            OnCurrentStateChanged?.Invoke();
        }
    }

    private string workingDirectory;
    public string WorkingDirectory
    {
        get => workingDirectory;
        set
        {
            workingDirectory = value;
        }
    }

    public string CurrentSelectedPremadeName { get; set; }
    public string CurrentSelectedPremadeDirectory { get; set; }
    public string CurrentSelectedPremadeGLB { get; set; }

    // Add singleton initialization
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initialize the lists with premades from the directories
    private void Start()
    {
        InitalizeDirectoryLists();
    }
    
    private void InitalizeDirectoryLists()
    {
        // // Initialize the lists with premades from the directories
        // allPremades = LoadPremades.GetPremadesFromDirectory("All Premades");
        // favoritesPremades = LoadPremades.GetPremadesFromDirectory("All Premades/Favorites");
        // recycleBinPremades = LoadPremades.GetPremadesFromDirectory("All Premades/Recycle Bin");

        // // Set the initial working directory
        // WorkingDirectory = "All Premades";
    }
    

}
