using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.UX;
using TMPro;
using GLTFast;
using UnityEngine.UI;
using UnityEditor;

//  Handles Directory and File Operations for Premades
public class LoadPremades : MonoBehaviour
{
    // Event for Loading the Premade
    public static event Action OnLoadPremadeRequested;
    public static void RequestLoadPremade() => OnLoadPremadeRequested?.Invoke();

    // Events for favorite/unfavorite actions
    public static event Action OnFavoritePremadeRequested;
    public static void RequestFavoritePremade() => OnFavoritePremadeRequested?.Invoke();
    public static event Action OnUnfavoritePremadeRequested;
    public static void RequestUnfavoritePremade() => OnUnfavoritePremadeRequested?.Invoke();

    // Events for delete/restore actions
    public static event Action OnDeletePremadeRequested;
    public static void RequestDeletePremade() => OnDeletePremadeRequested?.Invoke();
    public static event Action OnRestorePremadeRequested;
    public static void RequestRestorePremade() => OnRestorePremadeRequested?.Invoke();

    public static event Func<string, string, Texture2D> OnGetPremadeImageTextureRequested;
    public static Texture2D RequestGetPremadeImageTexture(string subDirectory, string premadeName) => OnGetPremadeImageTextureRequested?.Invoke(subDirectory, premadeName);

    [Header("Settings")]
    [Tooltip("Base directory for premade data (relative to Assets folder)")]
    [SerializeField] private string premadeBaseDirectory = "Premades";

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject premadeCanvas;

    [Tooltip("Position to load models")]
    [SerializeField] private Vector3 modelLoadPosition = new Vector3(0.246f, 1.388f, 0.576f);

    [Tooltip("Scale for loaded models")]
    [SerializeField] private float modelScale = 0.5f;

    [Tooltip("Load models automatically on start")]
    [SerializeField] private bool loadOnStart = true;


    [Tooltip("Reference to the NearBaseMenu object")]
    [SerializeField] private Transform nearBaseMenu;

    [Tooltip("Distance range from NearBaseMenu to spawn models")]
    [SerializeField] private float spawnRadius = 1.0f;

    [Tooltip("Minimum distance between models")]
    [SerializeField] private float minDistanceBetweenModels = 0.3f;

    private GameObject currentLoadedModel;
    private List<Vector3> occupiedPositions = new List<Vector3>();


    public void OnEnable()
    {
        OnLoadPremadeRequested += LoadPremade;
        OnFavoritePremadeRequested += FavoritePremade;
        OnUnfavoritePremadeRequested += UnfavoritePremade;

        OnDeletePremadeRequested += DeletePremade;
        OnRestorePremadeRequested += RestorePremade;

        OnGetPremadeImageTextureRequested += GetPremadeImageTexture;
    }

    public void OnDisable()
    {
        OnLoadPremadeRequested -= LoadPremade;
        OnFavoritePremadeRequested -= FavoritePremade;
        OnUnfavoritePremadeRequested -= UnfavoritePremade;

        OnDeletePremadeRequested -= DeletePremade;
        OnRestorePremadeRequested -= RestorePremade;

        OnGetPremadeImageTextureRequested -= GetPremadeImageTexture;
    }

    public Texture2D GetPremadeImageTexture(string subDirectory, string premadeName)
    {
        // Get the base path for premade directories
        string basePath = Path.Combine(Application.dataPath, premadeBaseDirectory);

        // Construct the directory path
        string directoryPath = Path.Combine(basePath, subDirectory, premadeName);

        // Look for image files in the directory
        string[] imageFiles = Directory.GetFiles(directoryPath, "*.png");
        if (imageFiles.Length > 0)
        {
            // Load the first image file as a texture
            byte[] imageData = File.ReadAllBytes(imageFiles[0]);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            return texture;
        }

        Debug.LogWarning($"No image files found in: {directoryPath}");
        return null;
    }


    public void FavoritePremade()
    {
        string premadeName = LoadPremadesModel.Instance.CurrentSelectedPremadeName;

        Debug.Log($"Favoriting premade: {premadeName}");

        MovePremadeToNewDirectory(premadeName, "All Premades", "All Premades/Favorites");

        LoadPremadesPresenter.RequestRefreshPremadeList();
    }

    public void UnfavoritePremade()
    {
        string premadeName = LoadPremadesModel.Instance.CurrentSelectedPremadeName;

        Debug.Log($"Unfavoriting premade: {premadeName}");

        MovePremadeToNewDirectory(premadeName, "All Premades/Favorites", "All Premades");

        LoadPremadesPresenter.RequestRefreshPremadeList();
    }

    public void MovePremadeToNewDirectory(string premadeName, string sourceDir, string newDir)
    {
        // string sourcePath = Path.Combine(Application.dataPath, premadeBaseDirectory, sourceDir, premadeName);
        // string destinationPath = Path.Combine(Application.dataPath, premadeBaseDirectory, newDir, premadeName);

        // Directory.Move(sourcePath, destinationPath);
        // Debug.Log($"Moved premade from {sourcePath} to {destinationPath}");

#if UNITY_EDITOR
        // Convert to relative paths from Assets folder
        string relativeSourcePath = Path.Combine(premadeBaseDirectory, sourceDir, premadeName);
        string relativeDestinationPath = Path.Combine(premadeBaseDirectory, newDir, premadeName);

        // Ensure destination directory exists
        string destinationDir = Path.Combine(premadeBaseDirectory, newDir);
        string fullDestinationDir = Path.Combine(Application.dataPath, destinationDir);
        if (!Directory.Exists(fullDestinationDir))
        {
            Directory.CreateDirectory(fullDestinationDir);
            UnityEditor.AssetDatabase.Refresh();
        }

        // Use Unity's AssetDatabase to move the folder
        string result = UnityEditor.AssetDatabase.MoveAsset(
            Path.Combine("Assets", relativeSourcePath),
            Path.Combine("Assets", relativeDestinationPath)
        );

        if (string.IsNullOrEmpty(result))
        {
            Debug.Log($"Successfully moved premade from {relativeSourcePath} to {relativeDestinationPath}");
        }
        else
        {
            Debug.LogError($"Failed to move premade: {result}");
        }
#else
        // Fallback for runtime (though this shouldn't be used in builds)
        string sourcePath = Path.Combine(Application.dataPath, premadeBaseDirectory, sourceDir, premadeName);
        string destinationPath = Path.Combine(Application.dataPath, premadeBaseDirectory, newDir, premadeName);
        Directory.Move(sourcePath, destinationPath);
        Debug.Log($"Moved premade from {sourcePath} to {destinationPath}");
#endif
    }

    public void DeletePremade()
    {
        string premadeName = LoadPremadesModel.Instance.CurrentSelectedPremadeName;

        string currentOpenedPremadeDirectory = LoadPremadesModel.Instance.CurrentSelectedPremadeDirectory;

        Debug.Log($"Moving premade object to deleted folder: {premadeName}");

        MovePremadeToNewDirectory(premadeName, currentOpenedPremadeDirectory, "Recycle Bin");

        LoadPremadesPresenter.RequestRefreshPremadeList();

        Debug.Log("Premade object moved to deleted folder and button list refreshed.");
        // // Get the directory paths
        // string basePath = Path.Combine(Application.dataPath, premadeBaseDirectory);
        // string sourceDirectoryPath = Path.Combine(basePath, premadeModelName);
        // string deletedBasePath = Path.Combine(basePath, "RecycleBin");
        // string destinationDirectoryPath = Path.Combine(deletedBasePath, premadeModelName);

        // try
        // {
        //     // Check if source directory exists
        //     if (Directory.Exists(sourceDirectoryPath))
        //     {
        //         // Create the "deleted" directory if it doesn't exist
        //         // if (!Directory.Exists(deletedBasePath))
        //         // {
        //         //     Directory.CreateDirectory(deletedBasePath);
        //         //     Debug.Log($"Created deleted directory: {deletedBasePath}");
        //         // }

        //         // Handle case where destination already exists (rename with timestamp)
        //         if (Directory.Exists(destinationDirectoryPath))
        //         {
        //             string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        //             destinationDirectoryPath = Path.Combine(deletedBasePath, $"{premadeModelName}_{timestamp}");
        //             Debug.Log($"Destination exists, using timestamped name: {destinationDirectoryPath}");
        //         }

        //         // Move the directory to the deleted folder
        //         Directory.Move(sourceDirectoryPath, destinationDirectoryPath);

        //         // Verify move worked
        //         if (!Directory.Exists(sourceDirectoryPath) && Directory.Exists(destinationDirectoryPath))
        //         {
        //             Debug.Log($"Successfully moved directory from {sourceDirectoryPath} to {destinationDirectoryPath}");
        //         }
        //         else
        //         {
        //             Debug.LogWarning($"Directory move may have failed. Source exists: {Directory.Exists(sourceDirectoryPath)}, Destination exists: {Directory.Exists(destinationDirectoryPath)}");
        //         }

        //         // Hide the premade canvas
        //         premadeCanvas.SetActive(false);

        //         // Refresh the button list to remove the deleted item
        //         LoadPremadesUIController.RequestPremadeListRefresh();

        //         Debug.Log("Premade object moved to deleted folder and button list refreshed.");
        //     }
        //     else
        //     {
        //         Debug.LogWarning($"Source directory not found: {sourceDirectoryPath}");
        //     }
        // }
        // catch (UnauthorizedAccessException e)
        // {
        //     Debug.LogError($"Access denied when moving directory: {e.Message}");
        // }
        // catch (DirectoryNotFoundException e)
        // {
        //     Debug.LogError($"Directory not found: {e.Message}");
        // }
        // catch (IOException e)
        // {
        //     Debug.LogError($"IO error when moving directory: {e.Message}");
        // }
        // catch (System.Exception e)
        // {
        //     Debug.LogError($"Failed to move premade object: {e.Message}");
    }


    public void RestorePremade()
    {
        string premadeName = LoadPremadesModel.Instance.CurrentSelectedPremadeName;
        string currentOpenedPremadeDirectory = LoadPremadesModel.Instance.CurrentSelectedPremadeDirectory;

        Debug.Log($"Restoring premade object from deleted folder: {premadeName}");

        MovePremadeToNewDirectory(premadeName, "Recycle Bin", "All Premades");

        LoadPremadesPresenter.RequestRefreshPremadeList();

        Debug.Log("Premade object restored from deleted folder and button list refreshed.");
    }

    public void LoadPremade()
    {
        string glbFilePath = LoadPremadesModel.Instance.CurrentSelectedPremadeGLB;

        Debug.Log($"Loading premade from glb file: {glbFilePath}");

        // Load the GLTF/GLB model from file
        StartCoroutine(LoadGLTFModelCoroutine(glbFilePath));
    }

    private IEnumerator LoadGLTFModelCoroutine(string filePath)
    {
        // Create a new game object to hold the imported model
        GameObject modelParent = new GameObject("ImportedPremadeModel");
        currentLoadedModel = modelParent;

        // Create a new GLTFast importer instance
        var gltf = new GltfImport();

        // Load the model from file
        bool success = false;

        var loadTask = gltf.Load(filePath);
        while (!loadTask.IsCompleted)
        {
            yield return null;
        }
        success = loadTask.Result;

        if (success)
        {
            // Instantiate the model into our scene
            var instantiateTask = gltf.InstantiateMainSceneAsync(modelParent.transform);
            while (!instantiateTask.IsCompleted)
            {
                yield return null;
            }
            bool instantiationSuccess = instantiateTask.Result;

            if (instantiationSuccess)
            {

                // Get a random position near the NearBaseMenu
                Vector3 spawnPosition = GetRandomPositionNearTarget();

                // Set position and scale
                // modelParent.transform.position = modelLoadPosition;
                modelParent.transform.position = spawnPosition;
                modelParent.transform.localScale = Vector3.one * modelScale;

                // Add this position to occupied positions
                occupiedPositions.Add(spawnPosition);

                Debug.Log($"Model loaded into the scene at position: {spawnPosition}");
            }
            else
            {
                Debug.LogError("Model instantiation failed.");
                Destroy(modelParent);
                currentLoadedModel = null;
            }
        }
        else
        {
            Debug.LogError("Model loading failed.");
            Destroy(modelParent);
            currentLoadedModel = null;
        }
    }


    private Vector3 GetRandomPositionNearTarget()
    {
        Vector3 basePosition = nearBaseMenu != null ? nearBaseMenu.position : Vector3.zero;
        Vector3 candidatePosition;
        int maxAttempts = 20;
        int attempts = 0;

        do
        {
            // Generate random position in a circle around the base position
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * spawnRadius;
            candidatePosition = basePosition + new Vector3(randomCircle.x, 0, randomCircle.y);
            attempts++;
        }
        while (IsPositionTooClose(candidatePosition) && attempts < maxAttempts);

        // If we couldn't find a good position after max attempts, use the candidate anyway
        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Could not find ideal spawn position, using fallback position");
        }

        return candidatePosition;
    }

    private bool IsPositionTooClose(Vector3 candidatePosition)
    {
        foreach (Vector3 occupiedPos in occupiedPositions)
        {
            if (Vector3.Distance(candidatePosition, occupiedPos) < minDistanceBetweenModels)
            {
                return true;
            }
        }
        return false;
    }

    // Call this method when you want to clear all loaded models and reset positions
    public void ClearAllLoadedModels()
    {
        occupiedPositions.Clear();
        if (currentLoadedModel != null)
        {
            Destroy(currentLoadedModel);
            currentLoadedModel = null;
        }
    }
}
