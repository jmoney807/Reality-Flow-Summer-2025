using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectGenerationUIView : MonoBehaviour
{

    [SerializeField] public GameObject ObjectGenerationUI;

    public TextMeshProUGUI HeaderText;
    public GameObject HorizontalButtonBar;

    public Image LoadingSpinnerImageComponent;

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

    public void ShowLoadingSpinner()
    {
        // turn off the Image component
        LoadingSpinnerImageComponent.enabled = true;
        // LoadingSpinnerImageComponent.gameObject.SetActive(true);
    }

    public void HideLoadingSpinner()
    {
        LoadingSpinnerImageComponent.enabled = false;
        // LoadingSpinnerImageComponent.gameObject.SetActive(false);
    }

    public void ShowHorizontalButtonBar()
    {
        HorizontalButtonBar.SetActive(true);
    }

    public void HideHorizontalButtonBar()
    {
        HorizontalButtonBar.SetActive(false);
    }

    public void IndicateObjectGenerationStarted()
    {
        Debug.Log("Object generation started.");
        HeaderText.text = "Generating 3D Model";
        ShowObjectGenerationUI();
        ShowLoadingSpinner();
        HideHorizontalButtonBar();
    }

    public void IndicateObjectGenerationCompleted()
    {
        Debug.Log("Object generation completed.");
        HeaderText.text = "Object Generation Complete";
        HideLoadingSpinner();
        ShowHorizontalButtonBar();
    }
   
}