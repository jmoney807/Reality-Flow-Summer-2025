using UnityEngine;
using System;
using System.IO;
using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine.UI;


public class LoadPremadesPresenter : MonoBehaviour
{
    // [SerializeField] private UIStateMachine uiStateMachine;
    private UIStateMachine uiStateMachine;

    public static event Action OnRefreshPremadeListRequested;
    public static void RequestRefreshPremadeList() => OnRefreshPremadeListRequested?.Invoke();

    public event Action<IState> OnNewButtonToggled;
    public event Action<IState> OnButtonUnToggled;

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

    private void Awake()
    {
        // uiStateMachine = new UIStateMachine(this);
        // uiStateMachine.Initialize(uiStateMachine.NoState);

    }

    private void OnEnable()
    {
        // LoadPremadesModel.Instance.OnCurrentStateChanged += OnCurrentStateChanged;

        OnRefreshPremadeListRequested += RequestPremadeListRefresh;

        OnNewButtonToggled += ChangeState;
        OnButtonUnToggled += ChangeState;
    }

    private void OnDisable()
    {
        LoadPremadesModel.Instance.OnCurrentStateChanged -= OnCurrentStateChanged;

        OnRefreshPremadeListRequested -= RequestPremadeListRefresh;

        OnNewButtonToggled -= ChangeState;
        OnButtonUnToggled -= ChangeState;
    }

    private void SubscribeToEvents()
    {
        // Now it's safe to access LoadPremadesModel.Instance
        if (LoadPremadesModel.Instance != null)
        {
            LoadPremadesModel.Instance.OnCurrentStateChanged += OnCurrentStateChanged;
        }
        else
        {
            Debug.LogError("LoadPremadesModel.Instance is null in SubscribeToEvents()");
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
        foreach (Transform child in LoadPremadesModel.Instance.buttonContainer)
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
        
        // Check if null which is the case when the state is NoState.
        // NoState is ALWAYS the state when generating new objects
        if (subDirectory == null)
        {
            Debug.LogWarning("Subdirectory is null, setting it to 'All Premades'.");
            subDirectory = "All Premades";
        }
        string basePath = Path.Combine(Application.dataPath, "Premades", subDirectory);

        bool isFavoritesDirectory = LoadPremadesModel.Instance.isFavoritesStateActive;

        string favoritesPath = Path.Combine(Application.dataPath, "Premades", "All Premades", "Favorites");

        // Get all directories in the base path
        string[] allDirectories = Directory.GetDirectories(basePath);

        // Create buttons for each directory
        foreach (string dirPath in allDirectories)
        {
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

                    CreateButton("All Premades/Favorites", favoritePremadeName, glbPathOfFavorites, true);
                }
                continue;
            }

            string modelName = Path.GetFileName(dirPath);

            // Look for GLB files in the directory to verify it has model data
            string[] glbFiles = Directory.GetFiles(dirPath, "*.glb");
            if (glbFiles.Length > 0)
            {
                // Get the first GLB file path
                string glbPath = glbFiles[0];
                CreateButton(subDirectory, modelName, glbPath, isFavoritesDirectory);
            }
        }
        Debug.Log($"Created buttons for {allDirectories.Length} premade sub directories");
    }

    private void CreateButton(string subDirectory, string modelName, string glbPath, bool isFavorited)
    {
        // Instantiate new button
        GameObject buttonObj = Instantiate(LoadPremadesModel.Instance.buttonPrefab, LoadPremadesModel.Instance.buttonContainer);
        PressableButton button = buttonObj.GetComponent<PressableButton>();

        if (button == null)
        {
            Debug.LogError("Button prefab doesn't contain a PressableButton component!");
            return;
        }

        // Adjust button size if needed
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(128f, 32f);
        }

        // Set button text
        TextMeshProUGUI buttonText = buttonObj.transform.Find("Frontplate/AnimatedContent/Text")?.GetComponent<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.gameObject.SetActive(true);
            buttonText.text = modelName;
        }


        // Hide icon initially
        GameObject iconObj = buttonObj.transform.Find("Frontplate/AnimatedContent/Icon/UIButtonFontIcon")?.gameObject;
        if (!isFavorited)
        {
            iconObj.SetActive(false);
        }

        // Set button onClick action (only if we have a valid file path)
        if (!string.IsNullOrEmpty(glbPath))
        {
            // Remove previous listeners to avoid duplicates
            button.OnClicked.RemoveAllListeners();
            button.OnClicked.AddListener(() => ShowObjectImage(subDirectory, modelName, glbPath, isFavorited));
        }
    }

    public void ShowObjectImage(string currentSubDirectory, string premadeName, string glbPath, bool isFavorited)
    {

        Debug.Log("Show object image for: " + premadeName);

        // Set the RawImage texture in the premade canvas
        LoadPremadesModel.Instance.premadeCanvas.SetActive(true);
        RawImage rawImage = LoadPremadesModel.Instance.premadeCanvas.transform.Find("Image Result")?.GetComponent<RawImage>();

        Texture2D texture = LoadPremades.RequestGetPremadeImageTexture(currentSubDirectory, premadeName);
        rawImage.texture = texture;

        SetLoadPremadesModelCurrentPremadeData(premadeName, currentSubDirectory, glbPath);
    
        InitializePremadeDetailsButtonsListeners(isFavorited);
    }
    
    public void SetLoadPremadesModelCurrentPremadeData(string premadeName, string currentSubDirectory, string glbPath)
    {
        LoadPremadesModel.Instance.CurrentSelectedPremadeName = premadeName;
        LoadPremadesModel.Instance.CurrentSelectedPremadeDirectory = currentSubDirectory;
        LoadPremadesModel.Instance.CurrentSelectedPremadeGLB = glbPath;
    }

    public void InitializePremadeDetailsButtonsListeners(bool isFavorited)
    {
        // Clear previous listeners
        foreach (var button in LoadPremadesModel.Instance.premadeDetailsButtons)
        {
            button.OnClicked.RemoveAllListeners();
        }

        LoadPremadesModel.Instance.premadeDetailsButtons[0].OnClicked.AddListener(() => LoadPremades.RequestLoadPremade());

        // Bool value of isFavorited
        Debug.Log($"Is Favorited: {isFavorited}");
        if (!isFavorited)
        {
            LoadPremadesModel.Instance.premadeDetailsButtons[2].OnClicked.AddListener(() => LoadPremades.RequestFavoritePremade());
        }
        else
        {
            Debug.Log("Adding listener for unfavoriting premade");
            LoadPremadesModel.Instance.premadeDetailsButtons[2].OnClicked.AddListener(() => LoadPremades.RequestUnfavoritePremade());
        }


        if (!LoadPremadesModel.Instance.isRecycledStateActive)
        {
            LoadPremadesModel.Instance.premadeDetailsButtons[3].OnClicked.AddListener(() => LoadPremades.RequestDeletePremade());
            // Make Element 4 (restore Premade Button) inactive
            LoadPremadesModel.Instance.premadeDetailsButtons[4].gameObject.SetActive(false);
        }
        else
        {
            // Make Element 4 (restore Premade Button) active and add listener
            LoadPremadesModel.Instance.premadeDetailsButtons[4].gameObject.SetActive(true);
            LoadPremadesModel.Instance.premadeDetailsButtons[4].OnClicked.AddListener(() => LoadPremades.RequestRestorePremade());
        }
    }
    




}
