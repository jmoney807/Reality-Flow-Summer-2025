using UnityEngine;

public class ObjectGenerationUIView : MonoBehaviour
{

    [SerializeField] public GameObject ObjectGenerationUI;

    public void Start()
    {
        ObjectGenerationUI.SetActive(false);
    }


    public void ShowObjectGenerationUI()
    {
        ObjectGenerationUI.SetActive(true);
    }

    public void HideObjectGenerationUI()
    {
        ObjectGenerationUI.SetActive(false);
    }
   
}