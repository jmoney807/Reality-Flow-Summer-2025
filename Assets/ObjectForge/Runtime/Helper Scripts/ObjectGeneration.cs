using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using GLTFast;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ObjectGeneration : MonoBehaviour
{
    public static event Action OnGenerateObjectRequested;
    public static void RequestObjectGeneration() => OnGenerateObjectRequested?.Invoke();

    private bool SPAR3DCompleted = false;
    private byte[] generatedModelData;

    [Header("Object Generation API Clients")]
    [SerializeField] private SPAR3DClient SPAR3DClient;
    [SerializeField] private TrellisClient TrellisClient;


    [Header("Model Spawning Settings")]
    [Tooltip("Reference to the NearBaseMenu object")]
    [SerializeField] private Transform nearBaseMenu;
    
    [Tooltip("Distance in front of NearBaseMenu to spawn models")]
    [SerializeField] private float spawnDistanceInFront = 1.0f;
    
    [Tooltip("Scale for loaded models")]
    [SerializeField] private float modelScale = 0.5f;
    
    [Tooltip("Minimum distance between models")]
    [SerializeField] private float minDistanceBetweenModels = 0.3f;
    
    [Tooltip("Maximum spread distance left/right from center")]
    [SerializeField] private float maxHorizontalSpread = 0.5f;

    private List<Vector3> occupiedPositions = new List<Vector3>();


    private void OnEnable()
    {
        OnGenerateObjectRequested += GenerateObject;
        SPAR3DClient.OnSPAR3DGenerationComplete += HandleSPAR3DGenerationComplete;
        SPAR3DClient.OnSPAR3DGenerationFailed += HandleSPAR3DGenerationFailed;

        // For now use HandleSPAR3DGenerationComplete for TRELLIS Prototyping
        // TrellisClient.OnTrellisGenerationComplete += HandleSPAR3DGenerationComplete;
        // TrellisClient.OnTrellisGenerationFailed += HandleSPAR3DGenerationFailed;
    }
    private void OnDisable()
    {
        OnGenerateObjectRequested -= GenerateObject;
        SPAR3DClient.OnSPAR3DGenerationComplete -= HandleSPAR3DGenerationComplete;
        SPAR3DClient.OnSPAR3DGenerationFailed -= HandleSPAR3DGenerationFailed;

        // TrellisClient.OnTrellisGenerationComplete -= HandleSPAR3DGenerationComplete;
        // TrellisClient.OnTrellisGenerationFailed -= HandleSPAR3DGenerationFailed;
    }

    private void HandleSPAR3DGenerationComplete(byte[] modelData)
    {
        generatedModelData = modelData;
        ObjectGenerationUIModel.Instance.SPAR3DResult = modelData;
        SPAR3DCompleted = true;
    }

    private void HandleSPAR3DGenerationFailed(string errorMessage)
    {
        Debug.LogError($"SPAR3D generation failed: {errorMessage}");
        SPAR3DCompleted = true; // Set to true to break the waiting loop
    }

    public void GenerateObject()
    {
        Debug.Log("Generating object...");
        StartCoroutine(GenerateObjectProcess());
    }

    private IEnumerator GenerateObjectProcess()
    {

        ObjectGenerationUIModel.Instance.IsGeneratingObject = true;

        // Set the body parameters for the SPAR3DClient
        SetSPAR3DClientParameters();

        // Get the processed image data from the UI model
        byte[] rembgResult = ImageGenerationUIModel.Instance.RembgResult;

        // Generate 3D object from the processed image
        SPAR3DClient.GenerateSPAR3DObject(rembgResult);
        // TrellisClient.GenerateTrellisObject(rembgResult);

        yield return StartCoroutine(WaitForCompletion(() => SPAR3DCompleted, "Object generation timed out"));
        // Reset the completion flag
        SPAR3DCompleted = false;

        // Check if generation was successful
        if (generatedModelData == null)
        {
            Debug.LogError("3D object generation failed");
            yield break;
        }

        Debug.Log("Object generated successfully, loading into scene...");

        ObjectGenerationUIModel.Instance.IsGeneratingObject = false;

        // Set the previous 
        SPAR3DSettingsModel.Instance.PreviousForegroundRatio = SPAR3DSettingsModel.Instance.ForegroundRatio;
        SPAR3DSettingsModel.Instance.PreviousGuidanceScaleValue = SPAR3DSettingsModel.Instance.GuidanceScaleValue;
        SPAR3DSettingsModel.Instance.PreviousSeedValue = SPAR3DSettingsModel.Instance.SeedValue;
        SPAR3DSettingsModel.Instance.PreviousTextureResolution = SPAR3DSettingsModel.Instance.TextureResolution;

        LoadGLBModelAsync();

        CreatePremade.RequestCreatePremade();

        // Clear the data
        generatedModelData = null;
    }

    private IEnumerator WaitForCompletion(Func<bool> isCompleted, string timeoutMessage, float timeoutDuration = 60f, float pollingInterval = 0.5f)
    {
        float elapsedTime = 0f;

        while (!isCompleted())
        {
            // Check for timeout
            if (elapsedTime > timeoutDuration)
            {
                Debug.LogError(timeoutMessage);
                yield break;
            }

            yield return new WaitForSeconds(pollingInterval);
            elapsedTime += pollingInterval;
        }
    }

    private void SetSPAR3DClientParameters()
    {
        SPAR3DClient.foregroundRatio = SPAR3DSettingsModel.Instance.ForegroundRatio;
        SPAR3DClient.guidanceScale = SPAR3DSettingsModel.Instance.GuidanceScaleValue;
        SPAR3DClient.seed = SPAR3DSettingsModel.Instance.SeedValue;
        SPAR3DClient.textureResolution = SPAR3DSettingsModel.Instance.TextureResolution;
    }

    // async void LoadGLBModelAsync()
    // {
    //     // Create a new game object to hold the imported model
    //     GameObject modelParent = new GameObject("ImportedModel");

    //     // Create a new GLTFast importer instance
    //     var gltf = new GltfImport();

    //     // Load the model from parameter
    //     bool success = await gltf.Load(ObjectGenerationUIModel.Instance.SPAR3DResult);

    //     if (success)
    //     {
    //         // Instantiate the model into our scene
    //         bool instantiationSuccess = await gltf.InstantiateMainSceneAsync(modelParent.transform);

    //         if (instantiationSuccess)
    //         {
    //             // Set position and scale
    //             modelParent.transform.position = new Vector3(0.246f, 1.388f, 0.576f);
    //             modelParent.transform.localScale = Vector3.one * 0.5f; // Adjusts scale

    //             Debug.Log("Model loaded into the scene!");
    //         }
    //         else
    //         {
    //             Debug.LogError("Model instantiation failed.");
    //             Destroy(modelParent);
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("Model loading failed.");
    //         Destroy(modelParent);
    //     }
    // }

    async void LoadGLBModelAsync()
    {
        // Create a new game object to hold the imported model
        GameObject modelParent = new GameObject("ImportedModel");

        // Create a new GLTFast importer instance
        var gltf = new GltfImport();

        // Load the model from parameter
        bool success = await gltf.Load(ObjectGenerationUIModel.Instance.SPAR3DResult);

        if (success)
        {
            // Instantiate the model into our scene
            bool instantiationSuccess = await gltf.InstantiateMainSceneAsync(modelParent.transform);

            if (instantiationSuccess)
            {
                // Get a position in front of the NearBaseMenu
                Vector3 spawnPosition = GetPositionInFrontOfNearBaseMenu();

                // Set position and scale
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
            }
        }
        else
        {
            Debug.LogError("Model loading failed.");
            Destroy(modelParent);
        }
    }

    private Vector3 GetPositionInFrontOfNearBaseMenu()
    {
        Vector3 basePosition = nearBaseMenu != null ? nearBaseMenu.position : Vector3.zero;
        Vector3 forwardDirection = nearBaseMenu != null ? nearBaseMenu.forward : Vector3.forward;
        
        Vector3 candidatePosition;
        int maxAttempts = 20;
        int attempts = 0;

        do
        {
            // Start with base position in front of the menu
            Vector3 frontPosition = basePosition + forwardDirection * spawnDistanceInFront;
            
            // Add some random horizontal offset to avoid exact overlap
            float horizontalOffset = UnityEngine.Random.Range(-maxHorizontalSpread, maxHorizontalSpread);
            Vector3 rightDirection = nearBaseMenu != null ? nearBaseMenu.right : Vector3.right;
            
            candidatePosition = frontPosition + rightDirection * horizontalOffset;
            
            // Ensure the Y position matches the NearBaseMenu's Y position
            candidatePosition.y = basePosition.y;
            
            attempts++;
        }
        while (IsPositionTooClose(candidatePosition) && attempts < maxAttempts);

        // If we couldn't find a good position after max attempts, use the candidate anyway
        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Could not find ideal spawn position in front of NearBaseMenu, using fallback position");
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
}