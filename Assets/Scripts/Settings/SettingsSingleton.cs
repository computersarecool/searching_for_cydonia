using extOSC;
using MusicControl;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSingleton : MonoBehaviour
{
    public static SettingsSingleton Instance { get; private set; }

    [Header("Game Objects")]
    public Camera MainCamera;
    public AudioEqualizer AudioEqualizer;
    public GameObject AttachedEnvironment;

    [Header("Communication")]
    public OSCTransmitter ExternalOSCTransmitter;
    public OSCTransmitter UnityOSCTransmitter;

    [Header("GUI")]
    public GameObject Canvas;
    public GameObject[] Panels;
    public Text TempoIndicator;
    public Image[] CycleOf8Indicators;
    public Image[] PositionIndicators;
    public Text[] FireButtonTexts;

    private int cameraIndex;
    private int CameraIndex
    {
        set
        {
            this.MainCamera.GetComponent<GridCamera>().CamIndex = value;
            this.cameraIndex = value;
        }
    }

    public Set LiveSet;
    [HideInInspector] public float Clock;

    private const int camRotationControlIndex = 2;
    private const int camMovementControlIndex = 6;
    private Image[] uiImages;
    private Text[] uiTexts;

    #region MonoBehaviours
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        this.LiveSet = new Set();
    }
    #endregion

    public void ChangeLiveTempo(bool increase)
    {
        this.LiveSet.ChangeLiveTempo(increase);
    }

    #region Game State
    public void UpdateCameraRotation(Vector3 vector)
    {
        if (this.cameraIndex != camRotationControlIndex)
            this.MainCamera.transform.eulerAngles = vector;
    }

    public void UpdateGUIHue(float newHue)
    {
        if (newHue < .01 || newHue > 1.0) return;

        this.AudioEqualizer.UpdateHue(newHue);

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
    }

    public void MoveMainCameraAndEnvironment(Vector3 vector)
    {
        if (this.cameraIndex != camMovementControlIndex)
            this.AttachedEnvironment.transform.position = vector;
    }

    public void SelectInterface(int index)
    {
        for (var i = 0; i < this.Panels.Length; i++)
        {
            this.Panels[i].SetActive(i == index);
        }
    }

    public void SelectCamera(int index)
    {
        Instance.CameraIndex = index;
    }
    #endregion
}
