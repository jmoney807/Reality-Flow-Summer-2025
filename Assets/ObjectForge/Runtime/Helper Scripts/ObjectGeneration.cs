using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using GLTFast;

public class ObjectGeneration : MonoBehaviour
{
    public static event Action OnGenerateObjectRequested;
    public static void RequestObjectGeneration() => OnGenerateObjectRequested?.Invoke(); 
    
    private bool SPAR3DCompleted = false;
    private byte[] generatedModelData;


    [SerializeField] private SPAR3DClient SPAR3DClient;

    private void OnEnable()
    {
        OnGenerateObjectRequested += GenerateObject;
        SPAR3DClient.OnSPAR3DGenerationComplete += HandleSPAR3DGenerationComplete;
        SPAR3DClient.OnSPAR3DGenerationFailed += HandleSPAR3DGenerationFailed;


    }
    private void OnDisable()
    {
        OnGenerateObjectRequested -= GenerateObject;
        SPAR3DClient.OnSPAR3DGenerationComplete -= HandleSPAR3DGenerationComplete;
        SPAR3DClient.OnSPAR3DGenerationFailed -= HandleSPAR3DGenerationFailed;

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

        // Get the processed image data from the UI model
        byte[] rembgResult = ImageGenerationUIModel.Instance.RembgResult;
        // Generate 3D object from the processed image
        SPAR3DClient.GenerateSPAR3DObject(rembgResult);

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
                // Set position and scale
                modelParent.transform.position = new Vector3(0.246f, 1.388f, 0.576f);
                modelParent.transform.localScale = Vector3.one * 0.5f; // Adjusts scale

                Debug.Log("Model loaded into the scene!");
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
}
