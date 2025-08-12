using UnityEngine;
using System;
using MixedReality.Toolkit.UX;
using System.Collections.Generic;

public class LoadPremadesModel : MonoBehaviour
{
    public static LoadPremadesModel Instance { get; private set; }

    [Header("Base Directory for Premades")]
    [SerializeField] public string PremadesBaseDirectory = "Premades/All Premades";

    // Used in LoadPremadesPresenter to check if either of these states are active
    public bool IsFavoritesStateActive = false; 
    public bool IsRecycledStateActive = false; 


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
            OnCurrentStateChanged?.Invoke();
        }
    }

    public event Action<IState> OnExitingState;
    private IState exitingState;
    public IState ExitingState
    {
        get => exitingState;
        set
        {
            exitingState = value;
            Debug.Log($"Exiting state set to: {exitingState.GetType().Name}, Invoking OnExitingState");
            OnExitingState?.Invoke(exitingState);
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

    // EXPERIMENTING WITH TOGGABLE BUTTONS BUT COULD NOT GET LOGIC TO WORK. INSTEAD I AM OPTING FOR A DEFAULT BUTTON SLECTION MODE THAT HAS SHOW DETAILS AND HIDE DETAILS INSTEAD
    // public event Action OnCurrentSelectedPremadeButtonUntoggled;
    // public event Action OnPreviousSelectedPremadeButtonUntoggled;
    // public event Action OnCurrentSelectedPremadeButtonToggled;

    // private PressableButton currentSelectedButton;
    // public PressableButton CurrentSelectedPremadeButton
    // {
    //     get => currentSelectedButton;
    //     set
    //     {

    //         if (currentSelectedButton == null)
    //         {
    //             Debug.Log("Toggling First button");
    //             currentSelectedButton = value; // Set the current selected button to the passed value
    //             OnCurrentSelectedPremadeButtonToggled?.Invoke();
    //             previousSelectedPremadeButton = currentSelectedButton; // Set the previous selected button 
    //         }
    //         else
    //         {
    //             Debug.Log("Current is already set, untoggling it");
    //             OnCurrentSelectedPremadeButtonUntoggled?.Invoke();
    //             previousSelectedPremadeButton = currentSelectedButton;
    //             currentSelectedButton = null;
    //         }

    //     }
    // }

    // private PressableButton previousSelectedPremadeButton;
    // public PressableButton PreviousSelectedPremadeButton
    // {
    //     get => previousSelectedPremadeButton;
    //     set
    //     {
    //         previousSelectedPremadeButton = value;

    //         if (previousSelectedPremadeButton == null)
    //         {
    //             Debug.Log("Toggling current selected button");
    //         }
    //         else
    //         {
    //             Debug.Log("Untoggling previous selected button");
    //             OnPreviousSelectedPremadeButtonUntoggled?.Invoke();

    //             if (previousSelectedPremadeButton == CurrentSelectedPremadeButton)
    //             {
    //                 Debug.Log("Previous selected button is the same as current selected button, not untoggling");
    //                 currentSelectedButton = null;

    //             }
    //         }
    //     }
    // }

    public string CurrentSelectedPremadeName { get; set; }
    public string CurrentSelectedPremadeDirectory { get; set; }
    public string CurrentSelectedPremadeGLB { get; set; }
    public bool IsCurrentSelectedPremadeFavorite { get; set; } = false;
    public bool IsCurrentSelectedPremadeRecycled { get; set; } = false;

    // Add singleton initialization
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

    // Initialize the lists with premades from the directories. NOT IMPLEMENTED BUT PLANNED TO BE USED FOR SEARCH
    private void Start()
    {
        // InitalizeDirectoryLists();
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
