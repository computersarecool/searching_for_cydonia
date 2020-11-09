using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public GameObject configurationPanel;
    public GameObject mainPanelsContainer;

    public void TogglePanels(bool showConfig)
    {
        configurationPanel.SetActive(showConfig);
        mainPanelsContainer.SetActive(!showConfig);
    }
}
