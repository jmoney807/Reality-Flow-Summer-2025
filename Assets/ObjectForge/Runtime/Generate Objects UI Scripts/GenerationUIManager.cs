using UnityEngine;
using MixedReality.Toolkit.UX; // Assuming you are using Mixed Reality Toolkit for UI components

public class GenerationUIManager : MonoBehaviour
{
    public static GenerationUIManager Instance { get; private set; }

    public PressableButton CurrentActiveUIButton { get; set; }

   
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

    void Start()
    {
        
    }

    void Update()
    {
    }
}
