using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MixedReality.Toolkit.UX;

public class GenerationUIController : MonoBehaviour
{
    // Reference to the UI elements
    [SerializeField] private GameObject objectGenerationUI;

    [SerializeField] private PressableButton _generateCustomsBtn;
    [SerializeField] private GameObject _generateCustomsUI;
    [SerializeField] private TextMeshProUGUI generateCustomsBtnIcon;
    [SerializeField] private TextMeshProUGUI generateCustomsBtnText;


    [SerializeField] private PressableButton _loadPremadesBtn;
    [SerializeField] private GameObject _loadPremadesUI;
    [SerializeField] private TextMeshProUGUI _loadPremadesBtnIcon;
    [SerializeField] private TextMeshProUGUI _loadPremadesBtnText;

    [SerializeField] private PressableButton _generationSettingsBtn;
    [SerializeField] private GameObject _generationSettingsUI;
    [SerializeField] private TextMeshProUGUI _generationSettingsBtnIcon;
    [SerializeField] private TextMeshProUGUI _generationSettingsBtnText;

    // Define enum for UI types
    public enum UIType
    {
        None,
        GenerateCustoms,
        GenerationSettings,
        LoadPremades
    }

    void Start()
    {
        // Initialize the UI elements if needed
        // if (objectGenerationUI != null)
        // {
        //     objectGenerationUI.SetActive(false); // Hide by default
        // }
        if (_generateCustomsUI != null)
        {
            _generateCustomsUI.SetActive(false); // Hide by default
        }

        GenerationUIManager.Instance.CurrentActiveUIButton = null;
    }

    // Add methods to manage the UI for generation here
    public void ShowObjectGenerationUI()
    {
        Debug.Log("Showing Object Generation UI");

        // Logic to show the generation UI
        objectGenerationUI.SetActive(true);
    }

    public void ShowSelectedUI(int uiType)
    {
        // Show the selected UI
        switch (uiType)
        {
            case 1:
                Debug.Log("Showing Object Generation UI");
                _generateCustomsUI.SetActive(true);

                HidePreviousUI(_generateCustomsBtn);

                IndicateActiveUIState(Color.yellow, generateCustomsBtnText, generateCustomsBtnIcon);
                break;

            case 2:
                Debug.Log("Showing Generation Settings UI");
                _generationSettingsUI.SetActive(true);

                HidePreviousUI(_generationSettingsBtn);

                IndicateActiveUIState(Color.yellow, _generationSettingsBtnText, _generationSettingsBtnIcon);
                break;

            case 3:
                Debug.Log("Showing Load Premades UI");
                _loadPremadesUI.SetActive(true);

                HidePreviousUI(_loadPremadesBtn);

                IndicateActiveUIState(Color.yellow, _loadPremadesBtnText, _loadPremadesBtnIcon);
                break;
        }
    }


    public void HidePreviousUI(PressableButton newButton)
    {
        PressableButton previousButton = GenerationUIManager.Instance.CurrentActiveUIButton;

        // Hide previous open UI by forcing its button to untoggle if not null
        if (previousButton != null && previousButton != newButton)
        {
            previousButton.ForceSetToggled(false);
        }

        // Set the new active button
        GenerationUIManager.Instance.CurrentActiveUIButton = newButton;
    }

    public void HideSelectedUI(int uiType)
    {
        switch (uiType)
        {
            case 1:
                _generateCustomsUI.SetActive(false);
                IndicateActiveUIState(Color.white, generateCustomsBtnText, generateCustomsBtnIcon);
                break;

            case 2:
                _generationSettingsUI.SetActive(false);
                IndicateActiveUIState(Color.white, _generationSettingsBtnText, _generationSettingsBtnIcon);
                break;
            case 3:
                _loadPremadesUI.SetActive(false);
                IndicateActiveUIState(Color.white, _loadPremadesBtnText, _loadPremadesBtnIcon);
                break;
        }
    }

    public void HideObjectGenerationUI()
    {
        // Logic to hide the generation UI
        objectGenerationUI.SetActive(false);
    }

    private void IndicateActiveUIState(Color color, TextMeshProUGUI buttonIcon, TextMeshProUGUI buttonText)
    {
        if (buttonText != null && buttonIcon != null)
        {
            Debug.Log("Changing button text color to indicate active UI state");
            buttonIcon.enableVertexGradient = true;
            buttonIcon.colorGradient = new VertexGradient(color);

            buttonText.enableVertexGradient = true;
            buttonText.colorGradient = new VertexGradient(color);
        }
    }

}
