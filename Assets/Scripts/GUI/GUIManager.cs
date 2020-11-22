using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public GameObject ConfigurationPanel;
    public GameObject MainPanelsContainer;

    private void Start()
    {
        this.ConfigurationPanel.SetActive(false);
        this.MainPanelsContainer.SetActive(true);

        foreach (var panel in SettingsSingleton.Instance.Panels)
        {
            panel.SetActive(false);
        }
        SettingsSingleton.Instance.Panels[0].SetActive(true);
    }

    public void TogglePanels(bool showConfig)
    {
        this.ConfigurationPanel.SetActive(showConfig);
        this.MainPanelsContainer.SetActive(!showConfig);
    }
}
