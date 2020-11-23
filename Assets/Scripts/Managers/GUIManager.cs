using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    private const string configurationPanelTag = "Configuration Panel";
    private const string mainCanvasTag = "Main Canvas";
    private const string tempoIndicatorTag = "Tempo Indicator";
    private const string cycleIndicatorTag = "Cycle Indicator";
    private const string clipPositionIndicatorTag = "Clip Position Indicator";
    private const string fireButtonTag = "Fire Button";

    private CommunicationManager communicator;
    private GameObject configurationPanel;
    private GameObject mainCanvas;
    private AudioEqualizer audioEqualizer;
    private List<MaskableGraphic> hueObjects;
    private List<GameObject> tempoIndicators;
    private List<GameObject> cycleIndicators;
    private List<GameObject> positionIndicators;
    private List<GameObject> fireButtonTexts;

    private int activePanel;
    private readonly List<GameObject> mainPanels = new List<GameObject>();

    private float hue;
    public float Hue
    {
        set
        {
            if (value < .01 || value > 1.0 || Math.Abs(value - this.hue) < .0001) return;

            this.hue = value;

            // TODO: Can make more efficient by getting by tag?
            foreach (var element in this.hueObjects)
            {
                var originalColor = element.color;
                var originalAlpha = originalColor.a;
                Color.RGBToHSV(originalColor, out _, out var startSat, out var startVal);
                var rgbColor = Color.HSVToRGB(value, startSat, startVal);

                element.color = new Color(rgbColor.r, rgbColor.g, rgbColor.b, originalAlpha);
            }

            this.audioEqualizer.UpdateHue(value);
        }
    }

    private void Start()
    {
        // TODO: Find if inactive
        this.communicator = (CommunicationManager)FindObjectOfType(typeof(CommunicationManager));
        this.configurationPanel = GameObject.FindWithTag(configurationPanelTag);
        this.mainCanvas = GameObject.FindWithTag(mainCanvasTag);
        this.audioEqualizer = (AudioEqualizer)FindObjectOfType(typeof(AudioEqualizer));
        var hueImages = new List<MaskableGraphic>(this.mainCanvas.GetComponentsInChildren<Image>(true));
        var hueTexts = new List<MaskableGraphic>(this.mainCanvas.GetComponentsInChildren<Text>(true));
        this.hueObjects = hueImages.Union(hueTexts).ToList();
        this.tempoIndicators = new List<GameObject>(GameObject.FindGameObjectsWithTag(tempoIndicatorTag));
        this.cycleIndicators = new List<GameObject>(GameObject.FindGameObjectsWithTag(cycleIndicatorTag));
        this.positionIndicators = new List<GameObject>(GameObject.FindGameObjectsWithTag(clipPositionIndicatorTag));
        this.fireButtonTexts = new List<GameObject>(GameObject.FindGameObjectsWithTag(fireButtonTag));

        foreach (Transform childTransform in mainCanvas.transform)
        {
            var child = childTransform.gameObject;
            if (child == this.configurationPanel) continue;

            child.SetActive(false);
            this.mainPanels.Add(child);
        }

        this.ToggleConfigurationPanel(false);
    }

    public void ToggleConfigurationPanel(bool show)
    {
        this.mainPanels[activePanel].SetActive(!show);
        this.configurationPanel.SetActive(show);
    }

    public void SelectInterface(int index)
    {
        this.activePanel = index;
        this.ToggleConfigurationPanel(false);
    }

    public void SetGUIHue(float value)
    {
        communicator.SetAppProperty(OSCListener.GUIAddress, OSCListener.GUIHueProperty, value);
    }
}
