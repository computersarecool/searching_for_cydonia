using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public GameObject ConfigurationPanel;
    public GameObject MainPanelsContainer;

    public void TogglePanels(bool showConfig)
    {
        this.ConfigurationPanel.SetActive(showConfig);
        this.MainPanelsContainer.SetActive(!showConfig);
    }
}
