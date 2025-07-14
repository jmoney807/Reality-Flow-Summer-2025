using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MixedReality.Toolkit.UX;
using System.Collections.Generic;
using System.Linq;

public class NearMenuBaseController : MonoBehaviour
{
    [System.Serializable]
    public class UIPanel
    {
        [Header("UI References")]
        public PressableButton button;
        public GameObject panelUI;
        public TextMeshProUGUI buttonText;
        public TextMeshProUGUI buttonIcon;
        
        [Header("Visual States")]
        public Color activeColor = Color.yellow;
        public Color inactiveColor = Color.white;
        
        [Header("Identification")]
        public string panelName;
    }

    [SerializeField] private GameObject NearMenuBaseUI;
    [SerializeField] private List<UIPanel> uiPanels = new List<UIPanel>();
    
    [Header("Active State Tracking")]
    private UIPanel currentActivePanel;
    private Dictionary<PressableButton, UIPanel> buttonToPanelMap;

    void Start()
    {
        InitializePanels();
        SetupButtonEvents();
        HideAllPanels();
    }

    private void InitializePanels()
    {
        // Create lookup dictionary for quick access
        buttonToPanelMap = uiPanels.ToDictionary(panel => panel.button, panel => panel);
        
        // Initialize all panels as inactive
        foreach (var panel in uiPanels)
        {
            if (panel.panelUI != null)
                panel.panelUI.SetActive(false);
                
            SetPanelVisualState(panel, false);
        }
        
        GenerationUIManager.Instance.CurrentActiveUIButton = null;
    }

    private void SetupButtonEvents()
    {
        foreach (var panel in uiPanels)
        {
            if (panel.button != null)
            {
                // Use closure to capture the panel reference
                var capturedPanel = panel;
                panel.button.OnClicked.AddListener(() => TogglePanel(capturedPanel));
            }
        }
    }

    public void TogglePanel(UIPanel targetPanel)
    {
        if (targetPanel == null) return;

        if (currentActivePanel == targetPanel)
        {
            // Hide current panel if clicking the same button
            HidePanel(targetPanel);
        }
        else
        {
            // Show new panel and hide previous
            ShowPanel(targetPanel);
        }
    }

    public void ShowPanel(UIPanel panel)
    {
        if (panel == null) return;

        // Hide previous panel
        if (currentActivePanel != null)
        {
            HidePanel(currentActivePanel);
        }

        // Show new panel
        panel.panelUI?.SetActive(true);
        SetPanelVisualState(panel, true);
        
        currentActivePanel = panel;
        GenerationUIManager.Instance.CurrentActiveUIButton = panel.button;
        
        Debug.Log($"Showing {panel.panelName} UI");
    }

    public void HidePanel(UIPanel panel)
    {
        if (panel == null) return;

        panel.panelUI?.SetActive(false);
        SetPanelVisualState(panel, false);
        panel.button?.ForceSetToggled(false);

        if (currentActivePanel == panel)
        {
            currentActivePanel = null;
            GenerationUIManager.Instance.CurrentActiveUIButton = null;
        }
    }

    private void HideAllPanels()
    {
        foreach (var panel in uiPanels)
        {
            HidePanel(panel);
        }
    }

    private void SetPanelVisualState(UIPanel panel, bool isActive)
    {
        if (panel.buttonText == null || panel.buttonIcon == null) return;

        Color targetColor = isActive ? panel.activeColor : panel.inactiveColor;
        
        panel.buttonIcon.enableVertexGradient = true;
        panel.buttonIcon.colorGradient = new VertexGradient(targetColor);
        
        panel.buttonText.enableVertexGradient = true;
        panel.buttonText.colorGradient = new VertexGradient(targetColor);
    }

    // Legacy methods for backward compatibility (if needed)
    public void ShowSelectedUI(int uiType)
    {
        if (uiType > 0 && uiType <= uiPanels.Count)
        {
            ShowPanel(uiPanels[uiType - 1]);
        }
    }

    public void HideSelectedUI(int uiType)
    {
        if (uiType > 0 && uiType <= uiPanels.Count)
        {
            HidePanel(uiPanels[uiType - 1]);
        }
    }

    // Public methods for external access
    public void ShowPanelByName(string panelName)
    {
        var panel = uiPanels.FirstOrDefault(p => p.panelName == panelName);
        if (panel != null)
        {
            ShowPanel(panel);
        }
    }

    public void ShowObjectGenerationUI()
    {
        Debug.Log("Showing Object Generation UI");
        NearMenuBaseUI?.SetActive(true);
    }

    public void HideObjectGenerationUI()
    {
        NearMenuBaseUI?.SetActive(false);
    }
}