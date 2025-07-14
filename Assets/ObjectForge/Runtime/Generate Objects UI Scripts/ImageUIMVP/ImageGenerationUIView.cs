using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ImageGenerationUIView : MonoBehaviour
{

    [SerializeField] public GameObject ImageGenerationUI;

    [SerializeField] public TextMeshProUGUI HeaderText;

    [SerializeField] public GameObject LoadingSpinnerObject;
    [SerializeField] public GameObject ImageResultObject;

    [SerializeField] public GameObject HorizontalButtonBar;

    public void Start()
    {
        ImageGenerationUI.SetActive(false);
    }

    public void ShowImageGenerationUI()
    {
        ImageGenerationUI.SetActive(true);
    }

    public void HideImageGenerationUI()
    {
        ImageGenerationUI.SetActive(false);
    }

    public void ShowLoadingSpinner()
    {
        LoadingSpinnerObject.SetActive(true);
    }

    public void HideLoadingSpinner()
    {
        LoadingSpinnerObject.SetActive(false);
    }

    public void ShowImageResult(byte[] imageData)
    {
        try
        {
            // Create a new texture
            Texture2D texture = new Texture2D(2, 2);
            // Load the image data directly from the variable
            texture.LoadImage(imageData);

            ImageResultObject.GetComponent<RawImage>().texture = texture;
            ImageResultObject.SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to display image: {e.Message}");
        }

    }

    public void HideImageResult()
    {
        ImageResultObject.SetActive(false);
    }

    public void ShowHorizontalButtonBar()
    {
        HorizontalButtonBar.SetActive(true);
    }

    public void HideHorizontalButtonBar()
    {
        HorizontalButtonBar.SetActive(false);
    }

    public void SetHeaderText(string text)
    {
        HeaderText.text = text;
    }
}
