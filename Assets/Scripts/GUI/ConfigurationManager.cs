using UnityEngine;

public class ConfigurationManager : MonoBehaviour
{
    public GameObject configurationPanel;
    public GameObject mainPanelsContainer;

    public void ShowConfig(bool showConfig)
    {
        configurationPanel.SetActive(showConfig);
        mainPanelsContainer.SetActive(!showConfig);
    }
}
