using UnityEngine;

public class HideUI : MonoBehaviour
{
    [SerializeField] private GameObject generateCustomsUI;
    [SerializeField] private GameObject loadPremadesUI;
    [SerializeField] private GameObject generationSettingsUI;

    public void HideGenerateCustomsUI()
    {
        generateCustomsUI.SetActive(false);
    }
    public void HideLoadPremadesUI()
    {
        loadPremadesUI.SetActive(false);
    }
    public void HideGenerationSettingsUI()
    {
        generationSettingsUI.SetActive(false);
    }

    public void ShowGenerateCustomsUI()
    {
        generateCustomsUI.SetActive(true);
    }
    public void ShowLoadPremadesUI()
    {
        loadPremadesUI.SetActive(true);
    }
    public void ShowGenerationSettingsUI()
    {
        generationSettingsUI.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
