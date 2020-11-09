using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public GameObject _configurationPanel;
    public GameObject mainPanelsContainer;

    public void TogglePanels(bool showConfig)
    {
        _configurationPanel.SetActive(showConfig);
        mainPanelsContainer.SetActive(!showConfig);
    }
}
