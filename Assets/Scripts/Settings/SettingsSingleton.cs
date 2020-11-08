using extOSC;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSingleton : MonoBehaviour
{
    public static SettingsSingleton Instance { get; private set; }

    public int cameraIndex;
    public Camera mainCamera;
    public AudioEqualizer AudioEqualizer;
    public GameObject fixedEnvironment;

    [Header("Communication")]
    public OSCTransmitter externalOSCTransmitter;
    public OSCTransmitter unityOSCTransmitter;

    [Header("GUI")]
    public GameObject canvas;
    public Text tempoIndicator;
    public GameObject[] panels;
    public TrackPositionController[] cycleOf8Indicators;
    public Image[] fullPositionControllers;
    public Text[] fireButtonTexts;
    public GameObject[] clipBanks;

    [HideInInspector] public float clock;
    [HideInInspector] public int viewingIndex;
    [HideInInspector] public int nextTrackToPlayIndex;

    public float LiveSetTempo
    {
        get => _liveSetTempo;
        set
        {
            _liveSetTempo = value;
            tempoIndicator.text = value.ToString("F0");
        }
    }

    public int InterfaceIndex
    {
        get => _interfaceIndex;
        set
        {
            _interfaceIndex = value;
            for (var i = 0; i < panels.Length; i++)
            {
                panels[i].SetActive(i == _interfaceIndex);
            }
        }
    }

    private readonly float[] _playingClipsLength = {1.0f, 1.0f, 1.0f, 1.0f};
    private const int CamRotationControlIndex = 2;
    private const int CamMovementControlIndex = 6;
    private int _interfaceIndex;
    private float _liveSetTempo;
    private Image[] _uiImages;
    private Text[] _uiTexts;

    public void UpdateClipName(int clipIndex, string clipName)
    {
        // It is expected that the song name looks like: 12A-126-Haddaway-Thing Called Love
        const int expectedNameElements = 4;
        const int bpmIndex = 0;
        const int keyIndex = 1;
        const int artistIndex = 2;
        const int titleIndex = 3;

        var clipNameArray = clipName.Split('-');
        var buttonIndex = clipIndex - viewingIndex;
        foreach (var clipBank in clipBanks)
        {
            var clipTextObjects = clipBank.transform.GetChild(buttonIndex).gameObject.GetComponentsInChildren<Text>();

            if (clipNameArray.Length < expectedNameElements)
            {
                clipTextObjects[0].text = clipName;
            }
            else
            {
                clipTextObjects[0].text = $"{clipNameArray[titleIndex]}";
                clipTextObjects[1].text = $"{clipNameArray[keyIndex]} {clipNameArray[bpmIndex]}";
                clipTextObjects[2].text = $"{clipNameArray[artistIndex]}";
            }
        }
    }

    public void UpdateClipColor(int clipIndex, int clipColor)
    {
        var colorR = (clipColor >> 16) & 255;
        var colorG = (clipColor >> 8) & 255;
        var colorB = clipColor & 255;
        var colorRf = colorR / 255.0f;
        var colorGf = colorG / 255.0f;
        var colorBf = colorB / 255.0f;

        var buttonIndex = clipIndex - viewingIndex;
        foreach (var clipBank in clipBanks)
        {
            var buttons = clipBank.GetComponentsInChildren<Button>();
            var clipButton = buttons[buttonIndex];
            var buttonColor = clipButton.colors;
            buttonColor.normalColor = new Color(colorRf, colorGf, colorBf);
            clipButton.colors = buttonColor;
        }
    }

    public void UpdatePlayingClipPosition(int trackNumber, float position)
    {
        // Update full track length percent in the GUI
        fullPositionControllers[trackNumber].fillAmount = position / _playingClipsLength[trackNumber];

        // Update cycle of 8
        var cycleOf8 = (int) position % 8;
        cycleOf8Indicators[trackNumber].UpdatePos(cycleOf8);
    }

    public void UpdateClipLength(int trackIndex, float length)
    {
        _playingClipsLength[trackIndex] = length;
    }

    public void UpdatePlayingClipName(int trackIndex, string clipName)
    {
        fireButtonTexts[trackIndex].text = clipName;
    }

    public void UpdateCameraRotation(Vector3 vector)
    {
        if (cameraIndex != CamRotationControlIndex)
            mainCamera.transform.eulerAngles = vector;
    }

    public void MoveMainCameraAndEnvironment(Vector3 vector)
    {
        if (cameraIndex != CamMovementControlIndex)
            fixedEnvironment.transform.position = vector;
    }

    public void UpdateGUIHue(float newHue)
    {
        if (!(newHue > .01) || !(newHue < 1.01)) return;

        AudioEqualizer.UpdateHue(newHue);

        foreach (var uiElement in _uiImages)
        {
            var originalColor = uiElement.color;
            var originalAlpha = originalColor.a;
            Color.RGBToHSV(originalColor, out _, out var startSat, out var startVal);
            var rgbColor = Color.HSVToRGB(newHue, startSat, startVal);
            uiElement.color = new Color(rgbColor.r, rgbColor.g, rgbColor.b, originalAlpha);
        }

        foreach (var uiElement in _uiTexts)
        {
            var originalColor = uiElement.color;
            var originalAlpha = originalColor.a;
            Color.RGBToHSV(originalColor, out _, out var startSat, out var startVal);
            var rgbColor = Color.HSVToRGB(newHue, startSat, startVal);
            uiElement.color = new Color(rgbColor.r, rgbColor.g, rgbColor.b, originalAlpha);
        }
    }

    private void GetSetState()
    {
        // Get data from live on start
        var message = new OSCMessage("live_set/get");
        message.AddValue(OSCValue.String("tempo"));
        externalOSCTransmitter.Send(message);

        message = new OSCMessage("live_set/view/get");
        message.AddValue(OSCValue.String("selected_scene_index"));
        externalOSCTransmitter.Send(message);
    }

    private void Awake()
    {
        // Delay to get the network ready
        const int delayForNetwork = 4;
        Invoke(nameof(GetSetState), delayForNetwork);

        // Collect GUI objects for hue change
        _uiImages = canvas.GetComponentsInChildren<Image>(true);
        _uiTexts = canvas.GetComponentsInChildren<Text>(true);

        // Singleton code
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
