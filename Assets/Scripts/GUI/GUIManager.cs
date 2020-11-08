using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public GameObject configurationPanel;
    public GameObject mainPanelsContainer;

    public void ShowConfig(bool showConfig)
    {
        configurationPanel.SetActive(showConfig);
        mainPanelsContainer.SetActive(!showConfig);
    }

    public void FillObject(float value, float vv)
    {
        Debug.Log(value);
    }
}
