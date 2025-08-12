using UnityEngine;
using MixedReality.Toolkit.UX;
using UnityEngine.UI;
using TMPro;

public class LoadPremadesView : MonoBehaviour
{
    public PressableButton[] PremadeFunctionButtons;

    [Header("State Buttons")]
    public PressableButton AllPremadesButton;
    public PressableButton FavoritesButton;
    public PressableButton RecycleBinButton;
    public PressableButton SearchButton;

    [Header("Premade Button Components")]
    public Transform PremadeButtonContainer;
    public GameObject PremadeButtonPrefab;

    [Header("Premade Image Canvas")]
    public GameObject PremadeImageCanvas;
    public RawImage PremadeRawImageComponent;
    public TextMeshProUGUI PremadeNameText;

    public void ForceUntoggleAllPremadesButton()
    {
        AllPremadesButton.ForceSetToggled(false);
    }
    public void ForceUntoggleFavoritesButton()
    {
        FavoritesButton.ForceSetToggled(false); 
    }
    public void ForceUntoggleRecycleBinButton()
    {
        RecycleBinButton.ForceSetToggled(false); 
    }

    public void ShowButtonsForFavorite()
    {
        PremadeFunctionButtons[2].gameObject.SetActive(false); // Hide Favorite button
        PremadeFunctionButtons[3].gameObject.SetActive(true); // Show Unfavorite button
    }

    public void ShowButtonsForNonFavorite()
    {
        PremadeFunctionButtons[2].gameObject.SetActive(true); // Show Favorite button
        PremadeFunctionButtons[3].gameObject.SetActive(false); // Hide Unfavorite button
    }

    public void ShowButtonsForRecycle()
    {
        PremadeFunctionButtons[2].gameObject.SetActive(false); // Hide Favorite button
        PremadeFunctionButtons[4].gameObject.SetActive(false); // Hide Recycle button
        PremadeFunctionButtons[5].gameObject.SetActive(true); // Show Restore button
        PremadeFunctionButtons[6].gameObject.SetActive(true); // Show Delete button
    }

    public void ShowButtonsForNonRecycle()
    {
        PremadeFunctionButtons[4].gameObject.SetActive(true); // Show Recycle button
        PremadeFunctionButtons[5].gameObject.SetActive(false); // Hide Restore button
        PremadeFunctionButtons[6].gameObject.SetActive(false); // Hide Delete button
    }

}
