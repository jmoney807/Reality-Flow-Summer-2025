using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;

public class ChangeMicrophonePresenter : MonoBehaviour
{

    private void Start()
    {
        // Ensure the microphone list is populated when the script starts
        PopulateMicrophoneList();
    }

    public void ChangeMicrophoneToggled()
    {
        // Show the microphone list panel
        ChangeMicrophoneModel.Instance.MicrophoneListPanel.SetActive(true);
    }

    public void ChangeMicrophoneUntoggled()
    {
        // Hide the microphone list panel
        ChangeMicrophoneModel.Instance.MicrophoneListPanel.SetActive(false);
    }

    public void PopulateMicrophoneList()
    {
        // Clear existing microphone options
        foreach (Transform child in ChangeMicrophoneModel.Instance.MicrophoneListContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Get the list of available microphones
        string[] microphones = Microphone.devices;

        // Create a button for each microphone
        foreach (string mic in microphones)
        {
            CreateButton(mic);
        }

        // Select the first microphone by default
        if (microphones.Length > 0 && string.IsNullOrEmpty(ChangeMicrophoneModel.Instance.SelectedMicrophone))
        {
            ChangeMicrophoneModel.Instance.SelectedMicrophone = microphones[0];
            Debug.Log("Default microphone selected: " + microphones[0]);
        }
        {
            GameObject firstButton = ChangeMicrophoneModel.Instance.MicrophoneListContainer.transform.GetChild(1).gameObject; // CHANGE BACK TO INDEX 0 WHEN YOU GET RID OF RECORD ACTION BTN PLACEHOLDER
            SelectMicrophone(microphones[0], firstButton);
        }
    }

    public void CleanMicrophoneList()
    {
        // Clear existing microphone options
        foreach (Transform child in ChangeMicrophoneModel.Instance.MicrophoneListContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateButton(string micDeviceName)
    {
        GameObject buttonPrefab = ChangeMicrophoneModel.Instance.MicrophoneOptionButtonPrefab;
        Transform buttonContainer = ChangeMicrophoneModel.Instance.MicrophoneListContainer.transform;

        if (buttonPrefab == null || buttonContainer == null)
        {
            Debug.LogError("Button prefab or container not assigned!");
            return;
        }

        // Instantiate new button
        GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
        PressableButton button = buttonObj.GetComponent<PressableButton>();

        if (button == null)
        {
            Debug.LogError("Button prefab doesn't contain a Button component!");
            return;
        }

        // Adjust button size
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Debug.Log("Setting button size");
            rectTransform.sizeDelta = new Vector2(128f, 32f); // Width: 200, Height: 50
        }

        // Set button text
        TextMeshProUGUI buttonText = buttonObj.transform.Find("Frontplate/AnimatedContent/Text")?.GetComponent<TextMeshProUGUI>();
        if (buttonText != null)
        {
            // Activate the button text if it's not already active
            buttonText.gameObject.SetActive(true);

            Debug.Log("Setting button text");
            buttonText.text = micDeviceName; // Set the microphone name as the button text
        }

        // Set button onClick action
        button.OnClicked.AddListener(() => SelectMicrophone(micDeviceName, buttonObj));

        // Get rid of Icon for all buttons
        GameObject iconObj = buttonObj.transform.Find("Frontplate/AnimatedContent/Icon/UIButtonFontIcon")?.gameObject;
        iconObj.SetActive(false);
    }

    public void SelectMicrophone(string micDeviceName, GameObject buttonObj)
    {
        // Clear old selection
        if (ChangeMicrophoneModel.Instance.CurrentSelectedButtonPrefab != null)
        {
            DeselectMicrophone(ChangeMicrophoneModel.Instance.CurrentSelectedButtonPrefab);
        }

        ChangeMicrophoneModel.Instance.SelectedMicrophone = micDeviceName;
        ChangeMicrophoneModel.Instance.CurrentSelectedButtonPrefab = buttonObj;

        Debug.Log("Selected microphone: " + micDeviceName);

        // Update the UI to reflect the selected microphone
        GameObject iconObj = buttonObj.transform.Find("Frontplate/AnimatedContent/Icon/UIButtonFontIcon")?.gameObject;
        iconObj.SetActive(true);

        FontIconSelector selectedButtonIcon = buttonObj.transform.Find("Frontplate/AnimatedContent/Icon/UIButtonFontIcon")?.GetComponent<FontIconSelector>();
        selectedButtonIcon.CurrentIconName = "Icon 20"; // Set the icon to the selected state
    }    
    
    public void DeselectMicrophone(GameObject buttonObj)
    {
        // Clear old selection
        ChangeMicrophoneModel.Instance.SelectedMicrophone = null;

        Debug.Log("Deselected microphone");

        // Update the UI to reflect the deselected state
        GameObject iconObj = buttonObj.transform.Find("Frontplate/AnimatedContent/Icon/UIButtonFontIcon")?.gameObject;
        iconObj.SetActive(false);

        FontIconSelector selectedButtonIcon = buttonObj.transform.Find("Frontplate/AnimatedContent/Icon/UIButtonFontIcon")?.GetComponent<FontIconSelector>();
        selectedButtonIcon.CurrentIconName = "Icon 135"; // Set the icon to the deselected state
    }
}
