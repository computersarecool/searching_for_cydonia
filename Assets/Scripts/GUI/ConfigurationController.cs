using UnityEngine;

public class ConfigurationController : MonoBehaviour
{
    public GameObject configurationPanel;
    public GameObject mainPanel;
    
    public void ShowConfig(bool showConfig)
    {
        configurationPanel.SetActive(showConfig);
        mainPanel.SetActive(!showConfig);
    }
}
