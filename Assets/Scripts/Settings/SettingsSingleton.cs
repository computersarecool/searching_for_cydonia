using extOSC;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSingleton : MonoBehaviour
{
    public static SettingsSingleton Instance { get; private set; }

    public Camera MainCamera;
    public AudioEqualizer AudioEqualizer;
    public GameObject AttachedEnvironment;

    [Header("Communication")]
    public OSCTransmitter ExternalOSCTransmitter;
    public OSCTransmitter UnityOSCTransmitter;

    [Header("GUI")]
    public GameObject Canvas;
    public Text TempoIndicator;
    public GameObject[] Panels;
    public Image[] CycleOf8Indicators;
    public Image[] PositionIndicators;
    public Text[] FireButtonTexts;
    public GameObject[] ClipBanks;

    [HideInInspector] public float Clock;
    [HideInInspector] public int ViewingIndex;
    [HideInInspector] public int NextSceneToPlay;
    

    private float liveSetTempo;
    public float LiveSetTempo
    {
        get => this.liveSetTempo;
        set
        {
            this.liveSetTempo = value;
            this.TempoIndicator.text = value.ToString("F0");
        }
    }

    private int interfaceIndex;
    public int InterfaceIndex
    {
        get => this.interfaceIndex;
        set
        {
            this.interfaceIndex = value;
            for (var i = 0; i < this.Panels.Length; i++)
            {
                this.Panels[i].SetActive(i == this.interfaceIndex);
            }
        }
    }

    private int cameraIndex;
    public int CameraIndex
    {
        set
        {
            this.MainCamera.GetComponent<GridCamera>().CamIndex = value;
            this.cameraIndex = value;
        }
    }

    private const int camRotationControlIndex = 2;
    private const int camMovementControlIndex = 6;
    private Image[] uiImages;
    private Text[] uiTexts;

    private readonly int[] playingSlotIndices = new int[8];
    private readonly float[] playingClipsLength = new float[8];
    private readonly string[] playingClipsName = new string[8];
    private readonly float[] playingClipsPosition = new float[8];
    private readonly string[] playingClipProperties = { "length", "name" };

    #region MonoBehaviours
    private void Awake()
    {
        const int delayForNetwork = 4;
        Invoke(nameof(GetSetState), delayForNetwork);

        // Collect GUI objects for hue change
        this.uiImages = this.Canvas.GetComponentsInChildren<Image>(true);
        this.uiTexts = this.Canvas.GetComponentsInChildren<Text>(true);

        // Singleton code
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    #region Ableton Functions
    private void GetSetState()
    {
        var message = new OSCMessage("/live_set");
        message.AddValue(OSCValue.String("get"));
        message.AddValue(OSCValue.String("tempo"));
        this.ExternalOSCTransmitter.Send(message);
    }
    #endregion

    public void UpdatePlayingSlotIndex(int track, int playingSlotIndex)
    {
        this.playingSlotIndices[track] = playingSlotIndex;
        Debug.Log(track);
        Debug.Log(playingSlotIndex);
        foreach (var prop in this.playingClipProperties)
        {
            var message = new OSCMessage($"/live_set/{track}/clip_slots/{playingSlotIndex}/clip");
            message.AddValue(OSCValue.String("get"));
            message.AddValue(OSCValue.String(prop));
            this.ExternalOSCTransmitter.Send(message);
        }
    }

    public void UpdatePlayingClipsLength(int track, int clip, float length)
    {
        if (this.playingSlotIndices[track] == clip)
        {
            this.playingClipsLength[track] = length;
        }
    }

    public void UpdatePlayingClipsName(int track, int clip, string name)
    {
        if (this.playingSlotIndices[track] == clip)
        {
            this.playingClipsName[track] = name;
            this.FireButtonTexts[track].text = name;
        }
    }

    public void UpdatePlayingClipsPosition(int track, int clip, float pos)
    {
        if (this.playingSlotIndices[track] == clip)
        {
            this.playingClipsPosition[track] = pos;
            this.PositionIndicators[track].fillAmount = pos / this.playingClipsLength[track];
            this.CycleOf8Indicators[track].fillAmount = (int)pos % 8 / 7.0F;
        }
    }

    public void UpdateClipName(int clipIndex, string clipName)
    {
        // It is expected that the song name looks like: 12A-126-Haddaway-Thing Called Love
        //const int expectedNameElements = 4;
        //const int bpmIndex = 0;
        //const int keyIndex = 1;
        //const int artistIndex = 2;
        //const int titleIndex = 3;

        //var clipNameArray = clipName.Split('-');
        //var buttonIndex = clipIndex - this.ViewingIndex;
        //foreach (var clipBank in this.ClipBanks)
        //{
        //    var clipTextObjects = clipBank.transform.GetChild(buttonIndex).gameObject.GetComponentsInChildren<Text>();

        //    if (clipNameArray.Length < expectedNameElements)
        //    {
        //        clipTextObjects[0].text = clipName;
        //    }
        //    else
        //    {
        //        clipTextObjects[0].text = $"{clipNameArray[titleIndex]}";
        //        clipTextObjects[1].text = $"{clipNameArray[keyIndex]} {clipNameArray[bpmIndex]}";
        //        clipTextObjects[2].text = $"{clipNameArray[artistIndex]}";
        //    }
        //}
    }

    public void UpdateClipColor(int clipIndex, int clipColor)
    {
        var colorR = (clipColor >> 16) & 255;
        var colorG = (clipColor >> 8) & 255;
        var colorB = clipColor & 255;
        var colorRf = colorR / 255.0f;
        var colorGf = colorG / 255.0f;
        var colorBf = colorB / 255.0f;

        var buttonIndex = clipIndex - this.ViewingIndex;
        foreach (var clipBank in this.ClipBanks)
        {
            var buttons = clipBank.GetComponentsInChildren<Button>();
            var clipButton = buttons[buttonIndex];
            var buttonColor = clipButton.colors;
            buttonColor.normalColor = new Color(colorRf, colorGf, colorBf);
            clipButton.colors = buttonColor;
        }
    }

    public void ChangeLiveTempo(bool increase)
    {
        var newTempo = increase ? this.liveSetTempo + 1 : this.liveSetTempo - 1;
        var message = new OSCMessage("/live_set");
        message.AddValue(OSCValue.String("set"));
        message.AddValue(OSCValue.String("tempo"));
        message.AddValue(OSCValue.Float(newTempo));
        this.ExternalOSCTransmitter.Send(message);
    }

    public void UpdatePlayingClipPosition(int trackNumber, float position)
    {
        // Update full track length percent in the GUI
        this.PositionIndicators[trackNumber].fillAmount = position / this.playingClipsLength[trackNumber];

        // Update cycle of 8
        var cycleOf8 = (int) position % 8;
        //this.CycleOf8Indicators[trackNumber].UpdatePos(cycleOf8);
    }

    public void UpdateClipLength(int trackIndex, float length)
    {
        this.playingClipsLength[trackIndex] = length;
    }

    public void UpdatePlayingClipName(int trackIndex, string clipName)
    {
        this.FireButtonTexts[trackIndex].text = clipName;
    }

    public void UpdateCameraRotation(Vector3 vector)
    {
        if (this.cameraIndex != camRotationControlIndex)
            this.MainCamera.transform.eulerAngles = vector;
    }

    public void UpdateGUIHue(float newHue)
    {
        if (newHue < .01 || newHue > 1.0) return;

        this.AudioEqualizer.UpdateHue(newHue);

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

    public void StoreSceneIndex(int index)
    {
        Instance.NextSceneToPlay = index + Instance.ViewingIndex;
    }

    public void SelectInterface(int index)
    {
        Instance.InterfaceIndex = index;
    }

    public void SelectCamera(int index)
    {
        Instance.CameraIndex = index;
    }
}
