using UnityEngine;
using System;

public class ObjectGenerationUIModel : MonoBehaviour
{
    public static ObjectGenerationUIModel Instance { get; private set; }

    private byte[] spar3dResult;
    public byte[] SPAR3DResult
    {
        get => spar3dResult;
        set
        {
            spar3dResult = value;
            // Notify listeners about the change if needed
        }
    }

    public static event Action OnObjectGenerationStarted;
    public static event Action OnObjectGenerationCompleted;

    private bool isGeneratingObject;
    public bool IsGeneratingObject
    {
        get => isGeneratingObject;
        set
        {
            isGeneratingObject = value;
            // Notify listeners about the change if needed
            if (isGeneratingObject)
            {

                Debug.Log("Invoking object generation started event");
                OnObjectGenerationStarted?.Invoke();
            }
            else
            {
                Debug.Log("Invoking object generation completed event");
                OnObjectGenerationCompleted?.Invoke();
            }
        }
    }

    private void Awake()
    {
        // Singleton pattern implementation
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
