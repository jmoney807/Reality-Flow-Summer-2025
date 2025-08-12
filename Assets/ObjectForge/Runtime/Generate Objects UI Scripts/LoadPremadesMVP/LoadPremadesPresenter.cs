using UnityEngine;
using System;
using System.IO;
using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Analytics;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class LoadPremadesPresenter : MonoBehaviour
{
    private UIStateMachine uiStateMachine;

    public LoadPremadesView LoadPremadesView;

    public static event Action OnRefreshPremadeListRequested;
    public static void RequestRefreshPremadeList() => OnRefreshPremadeListRequested?.Invoke();

    public static event Action OnUpdateViewsActiveButtonsRequested;
    public static void RequestUpdateViewsActiveButtons() => OnUpdateViewsActiveButtonsRequested?.Invoke();

    // Events for state changes
    public event Action<IState> OnNewButtonToggled;
    public event Action<IState> OnButtonUnToggled;

    // Toggle methods for different states
    public void ToggleAllPremadesState() => OnNewButtonToggled?.Invoke(uiStateMachine.AllPremadesState);
    public void ToggleFavoritesState() => OnNewButtonToggled?.Invoke(uiStateMachine.FavoritesState);
    public void ToggleRecycleBinState() => OnNewButtonToggled?.Invoke(uiStateMachine.RecycleBinState);
    public void UnToggleCurrentState() => OnButtonUnToggled?.Invoke(uiStateMachine.NoState);


    private void Start()
    {
        uiStateMachine = new UIStateMachine(this);
        uiStateMachine.Initialize(uiStateMachine.NoState);

        SubscribeToEvents();
    }

    private void OnEnable()
    {
        OnRefreshPremadeListRequested += RequestPremadeListRefresh;
        OnUpdateViewsActiveButtonsRequested += UpdateViewsActiveButtons;

        OnNewButtonToggled += ChangeState;
        OnButtonUnToggled += ChangeState;
    }

    private void OnDisable()
    {
        LoadPremadesModel.Instance.OnCurrentStateChanged -= OnCurrentStateChanged;

        OnRefreshPremadeListRequested -= RequestPremadeListRefresh;
        OnUpdateViewsActiveButtonsRequested -= UpdateViewsActiveButtons;

        OnNewButtonToggled -= ChangeState;
        OnButtonUnToggled -= ChangeState;
    }

    private void SubscribeToEvents()
    {
        // Now it's safe to access LoadPremadesModel.Instance
        if (LoadPremadesModel.Instance != null)
        {
            LoadPremadesModel.Instance.OnCurrentStateChanged += OnCurrentStateChanged;
            LoadPremadesModel.Instance.OnExitingState += ExitState;
            // LoadPremadesModel.Instance.OnCurrentSelectedPremadeButtonUntoggled += UntoggleCurrentSelectedButton;
            // LoadPremadesModel.Instance.OnPreviousSelectedPremadeButtonUntoggled += UntogglePreviousSelectedButton;
            // LoadPremadesModel.Instance.OnCurrentSelectedPremadeButtonToggled += ToggleCurrentSelectedButton;
        }
        else
        {
            Debug.LogError("LoadPremadesModel.Instance is null in SubscribeToEvents()");
        }
    }

    public void ExitState(IState exitingState)
    {
        if (exitingState.GetType() == typeof(AllPremadesState))
        {
            Debug.Log("Exiting AllPremadesState");
            LoadPremadesView.ForceUntoggleAllPremadesButton();
        }
        else if (exitingState.GetType() == typeof(FavoritesState))
        {
            Debug.Log("Exiting FavoritesState");
            LoadPremadesView.ForceUntoggleFavoritesButton();
        }
        else if (exitingState.GetType() == typeof(RecycleBinState))
        {
            Debug.Log("Exiting RecycleBinState");
            LoadPremadesView.ForceUntoggleRecycleBinButton();
        }
    }

    public void ChangeState(IState newState)
    {
        Debug.Log($"Changing state to: {newState.GetType().Name}");
        uiStateMachine.ChangeState(newState);
    }

    public void OnCurrentStateChanged()
    {
        ClearButtonList();
        PopulateButtonList();
        CloseCurrentPremadeImageCanvas();
    }

    public void CloseCurrentPremadeImageCanvas()
    {
        LoadPremadesView.PremadeImageCanvas.SetActive(false);
        LoadPremadesModel.Instance.CurrentSelectedPremadeName = string.Empty;
        LoadPremadesModel.Instance.CurrentSelectedPremadeDirectory = string.Empty;
        LoadPremadesModel.Instance.CurrentSelectedPremadeGLB = string.Empty;
    }

    public void RequestPremadeListRefresh()
    {
        Debug.Log("Requesting premade list refresh");
        ClearButtonList();
        PopulateButtonList();
    }

    public void ClearButtonList()
    {
        // Remove all existing buttons
        foreach (Transform child in LoadPremadesView.PremadeButtonContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void PopulateButtonList()
    {
        CreateButtonsForPremades(LoadPremadesModel.Instance.WorkingDirectory);
    }

    public void CreateButtonsForPremades(string subDirectory)
    {
        // Check if null or empty which is the case when the state is NoState.
        // NoState is ALWAYS the state when generating new objects
        if (string.IsNullOrEmpty(subDirectory))
        {
            Debug.LogWarning("Subdirectory is null or empty, setting it to 'All Premades'.");
            // subDirectory = "All Premades";
            return;
        }
        string basePath = Path.Combine(Application.dataPath, "Premades", subDirectory);

        string favoritesPath = Path.Combine(Application.dataPath, "Premades", "All Premades", "Favorites");

        // Get all directories in the base path
        string[] allDirectories = Directory.GetDirectories(basePath);

        // Create buttons for each directory
        foreach (string dirPath in allDirectories)
        {
            // Add the button to the List of Toggles

            Debug.Log($"Processing directory: {dirPath}");
            // Create buttons for Favorites directory as well
            if (dirPath == favoritesPath)
            {
                Debug.Log("Creating buttons for Favorites directory");

                string[] allFavorites = Directory.GetDirectories(favoritesPath);

                foreach (string favoritePremade in allFavorites)
                {
                    string favoritePremadeName = Path.GetFileName(favoritePremade);


                    string[] glbFilesOfFavorites = Directory.GetFiles(favoritePremade, "*.glb");
                    if (glbFilesOfFavorites.Length == 0)
                    {
                        Debug.LogWarning($"No GLB files found in favorites directory: {favoritePremade}");
                        continue;
                    }
                    string glbPathOfFavorites = glbFilesOfFavorites[0];

                    LoadPremadesModel.Instance.IsFavoritesStateActive = true;
                    CreateButton("All Premades/Favorites", favoritePremadeName, glbPathOfFavorites);

                }
                LoadPremadesModel.Instance.IsFavoritesStateActive = false;
                continue;
            }

            string modelName = Path.GetFileName(dirPath);

            // Look for GLB files in the directory to verify it has model data
            string[] glbFiles = Directory.GetFiles(dirPath, "*.glb");
            if (glbFiles.Length > 0)
            {
                // Get the first GLB file path
                string glbPath = glbFiles[0];
                CreateButton(subDirectory, modelName, glbPath);
            }
        }
        Debug.Log($"Created buttons for {allDirectories.Length} premade sub directories");
    }

    private void CreateButton(string subDirectory, string premadeName, string glbPath)
    {
        // Instantiate new button
        GameObject premadeButton = Instantiate(LoadPremadesView.PremadeButtonPrefab, LoadPremadesView.PremadeButtonContainer);
        PressableButton pressableButton = premadeButton.GetComponent<PressableButton>();

        bool isFavoritePremade = LoadPremadesModel.Instance.IsFavoritesStateActive;
        bool isRecycledPremade = LoadPremadesModel.Instance.IsRecycledStateActive;

        if (pressableButton == null)
        {
            Debug.LogError("Button prefab doesn't contain a PressableButton component!");
            return;
        }

        // Adjust button size if needed
        RectTransform rectTransform = premadeButton.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(128f, 32f);
        }

        // Set button text
        TextMeshProUGUI buttonText = premadeButton.transform.Find("Frontplate/AnimatedContent/Text")?.GetComponent<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.gameObject.SetActive(true);
            buttonText.text = premadeName;
        }

        // Hide fav icon initially
        GameObject iconObj = premadeButton.transform.Find("Frontplate/AnimatedContent/Icon/UIButtonFontIcon")?.gameObject;

        if (!isFavoritePremade)
        {
            iconObj.SetActive(false);
        }

        // Set button onClick action (only if we have a valid file path)
        if (!string.IsNullOrEmpty(glbPath))
        {
            // Remove previous listeners to avoid duplicates
            pressableButton.OnClicked.RemoveAllListeners();
            pressableButton.OnClicked.AddListener(() => UpdateSelectedPremade(pressableButton, subDirectory, premadeName, glbPath, isFavoritePremade, isRecycledPremade));
        }
    }

    // Update the selected premade data and premade view
    public void UpdateSelectedPremade(PressableButton pressableButton, string premadeDirectory, string premadeName, string glbPath, bool isFavoritePremade, bool isRecycledPremade)
    {

        // MY STOPPING POINT WAS HERE. MIGHT HAVE TO MOVE THIS METHOD AFTER DO THE CANVAS TO BE HIDDEN WHEN CURRENT SELECTED BUTTON IS UNTOGGLED. 
        // ALSO THEN WE NEED TO FIX THE CURRENT SELECTED BUTTON TO BE TOGGLED AFTER BEING UNTOGGLED
        SetLoadPremadesModelCurrentPremadeData(pressableButton, premadeName, premadeDirectory, glbPath, isFavoritePremade, isRecycledPremade);

        SetPremadeImage(premadeDirectory, premadeName);

        UpdateViewsActiveButtons();
    }

    // Set the RawImage texture in the premade canvas
    public void SetPremadeImage(string premadeDirectory, string premadeName)
    {
        LoadPremadesView.PremadeImageCanvas.SetActive(true);
        RawImage rawImage = LoadPremadesView.PremadeRawImageComponent;

        Texture2D texture = LoadPremades.RequestGetPremadeImageTexture(premadeDirectory, premadeName);
        rawImage.texture = texture;

        LoadPremadesView.PremadeNameText.text = premadeName;
    }

    // public void UntoggleCurrentSelectedButton()
    // {
    //     LoadPremadesModel.Instance.CurrentSelectedPremadeButton.ForceSetToggled(false);
    //     LoadPremadesView.PremadeImageCanvas.SetActive(false);
    // }

    // public void UntogglePreviousSelectedButton()
    // {
    //     LoadPremadesModel.Instance.PreviousSelectedPremadeButton.ForceSetToggled(false);
    // }

    // public void ToggleCurrentSelectedButton()
    // {
    //     LoadPremadesModel.Instance.CurrentSelectedPremadeButton.ForceSetToggled(true);
    //     LoadPremadesView.PremadeImageCanvas.SetActive(true);
    // }


    public void SetLoadPremadesModelCurrentPremadeData(PressableButton currentSelectedButton, string premadeName, string premadeDirectory, string glbPath, bool isFavoritePremade, bool isRecycledPremade)
    {

        // LoadPremadesModel.Instance.PreviousSelectedPremadeButton = LoadPremadesModel.Instance.CurrentSelectedPremadeButton; // Make the current selected button the previous one
        // LoadPremadesModel.Instance.CurrentSelectedPremadeButton = currentSelectedButton;                                    // Set the current selected button to the passed btn

        LoadPremadesModel.Instance.CurrentSelectedPremadeName = premadeName;
        LoadPremadesModel.Instance.CurrentSelectedPremadeDirectory = premadeDirectory;
        LoadPremadesModel.Instance.CurrentSelectedPremadeGLB = glbPath;

        // Testing if the current selected premade is a favorite or recycled
        LoadPremadesModel.Instance.IsCurrentSelectedPremadeFavorite = isFavoritePremade;
        LoadPremadesModel.Instance.IsCurrentSelectedPremadeRecycled = isRecycledPremade;
    }

    public void UpdateViewsActiveButtons()
    {
        if (LoadPremadesModel.Instance.IsCurrentSelectedPremadeFavorite)
        {
            LoadPremadesView.ShowButtonsForFavorite();
        }
        else
        {
            LoadPremadesView.ShowButtonsForNonFavorite();
        }
        if (LoadPremadesModel.Instance.IsCurrentSelectedPremadeRecycled)
        {
            LoadPremadesView.ShowButtonsForRecycle();
        }
        else
        {
            LoadPremadesView.ShowButtonsForNonRecycle();
        }
    }


}
