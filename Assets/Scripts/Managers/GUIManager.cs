using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public GameObject ConfigurationPanel;
    public GameObject MainPanelsContainer;

    public GameObject Canvas;
    public GameObject[] Panels;
    public Text TempoIndicator;
    public Image[] CycleOf8Indicators;
    public Image[] PositionIndicators;
    public Text[] FireButtonTexts;

    private Image[] uiImages;
    private Text[] uiTexts;

    private void Start()
    {
        this.ConfigurationPanel.SetActive(false);
        this.MainPanelsContainer.SetActive(true);

        foreach (var panel in this.Panels)
        {
            panel.SetActive(false);
        }
        this.Panels[0].SetActive(true);
    }

    public void TogglePanels(bool showConfig)
    {
        this.ConfigurationPanel.SetActive(showConfig);
        this.MainPanelsContainer.SetActive(!showConfig);
    }

    public void UpdateGUIHue(float newHue)
    {
        if (newHue < .01 || newHue > 1.0) return;

        //this.AudioEqualizer.UpdateHue(newHue);

        this.uiImages = this.Canvas.GetComponentsInChildren<Image>(true);
        this.uiTexts = this.Canvas.GetComponentsInChildren<Text>(true);

        foreach (var uiElement in this.uiImages)
        {
            var originalColor = uiElement.color;
            var originalAlpha = originalColor.a;
            Color.RGBToHSV(originalColor, out _, out var startSat, out var startVal);
            var rgbColor = Color.HSVToRGB(newHue, startSat, startVal);
            uiElement.color = new Color(rgbColor.r, rgbColor.g, rgbColor.b, originalAlpha);
        }

        foreach (var uiElement in this.uiTexts)
        {
            var originalColor = uiElement.color;
            var originalAlpha = originalColor.a;
            Color.RGBToHSV(originalColor, out _, out var startSat, out var startVal);
            var rgbColor = Color.HSVToRGB(newHue, startSat, startVal);
            uiElement.color = new Color(rgbColor.r, rgbColor.g, rgbColor.b, originalAlpha);
        }

        // var message = new OSCMessage($"{OSCCommunicationRouter.GUIHueAddress}");
        // message.AddValue(OSCValue.Float(val));
        // GetComponent<CommunicationManager>().primaryTransmitter.Send(message);
    }

    public void SelectInterface(int index)
    {
        for (var i = 0; i < this.Panels.Length; i++)
        {
            this.Panels[i].SetActive(i == index);
        }
    }

}
