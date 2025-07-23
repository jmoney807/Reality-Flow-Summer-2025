using UnityEngine;

public class ObjectGenerationUIPresenter : MonoBehaviour
{
    [SerializeField] private ObjectGenerationUIView objectGenerationUIView;

    public void OnEnable()
    {
        ObjectGenerationUIModel.OnObjectGenerationStarted += IndicateObjectGenerationStarted;
        ObjectGenerationUIModel.OnObjectGenerationCompleted += IndicateObjectGenerationCompleted;
    }

    public void OnDisable()
    {
        ObjectGenerationUIModel.OnObjectGenerationStarted -= IndicateObjectGenerationStarted;
        ObjectGenerationUIModel.OnObjectGenerationCompleted -= IndicateObjectGenerationCompleted;
    }

    public void IndicateObjectGenerationStarted()
    {
        Debug.Log("Object generation started.");
        objectGenerationUIView.ShowObjectGenerationUI();
        
    }
    public void IndicateObjectGenerationCompleted()
    {
        Debug.Log("Object generation completed.");
        objectGenerationUIView.HideObjectGenerationUI();
    }


}
