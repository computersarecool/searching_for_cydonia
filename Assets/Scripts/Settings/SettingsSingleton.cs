using extOSC;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSingleton : MonoBehaviour
{
    public VisualizerController visualizerController;
    public Camera mainCamera;
    public GameObject cameraEnvContainer;
    public OSCTransmitter channelTransmitter;
    public OSCTransmitter unityBroadcastTransmitter;
    public GameObject guiCanvas;
    public GameObject[] guiPanels;
    public Text tempoMonitor;
    public TrackPositionController[] cycleOf8Indicators;
    public Image[] fullPositionControllers;
    public Text[] fireButtonTexts;
    public GameObject[] clipBanks;
    
    [HideInInspector]public int cameraIndex;
    [HideInInspector]public float clock;
    [HideInInspector]public int viewingIndex;
    [HideInInspector]public int nextTrackToPlayIndex;
    public float LiveSetTempo
    {
        get => _liveSetTempo;
        set
        {
            _liveSetTempo = value;
            tempoMonitor.text = value.ToString("F0");
        }
    }
    public int InterfaceIndex
    {
        get => _interfaceIndex;
        set
        {
            _interfaceIndex = value;
            for (var i = 0; i < guiPanels.Length; i++)
            {
                guiPanels[i].SetActive(i == _interfaceIndex);
            }
        }
    }

    public static SettingsSingleton Instance { get; private set; }
    
    private readonly float[] _playingClipsLength = { 1.0f, 1.0f, 1.0f, 1.0f };
    private const int CamRotationControlIndex = 2;
    private const int CamMovementControlIndex = 6;
    private int _interfaceIndex;
    private float _liveSetTempo;
    private Image[] _uiImages;
    private Text[] _uiTexts;
    
    public void UpdateClipName(int clipIndex, string clipName)
    {
        // It is expected that the song name looks like 12A-126-Haddaway-Thing Called Love
        const int expectedNameElements = 4;
        const int bpmIndex = 0;
        const int keyIndex = 1;
        const int artistIndex = 2;
        const int titleIndex = 3;
        
        var clipNameArray = clipName.Split('-');
        var buttonIndex = clipIndex - viewingIndex;
        foreach (var clipBank in clipBanks)
        {
            var clipTextObjects =  clipBank.transform.GetChild(buttonIndex).gameObject.GetComponentsInChildren<Text>();

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
        var colorB = clipColor  & 255;
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
            cameraEnvContainer.transform.position = vector;
    }
    
    public void UpdateGuiHue(float newHue)
    {
        if (!(newHue > .01) || !(newHue < 1.01)) return;
        
        visualizerController.UpdateColor(newHue);
        
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
        // This gets data on start
        var message = new OSCMessage("live_set/get");
        message.AddValue(OSCValue.String("tempo"));
        channelTransmitter.Send(message);
        
        message = new OSCMessage("live_set/view/get");
        message.AddValue(OSCValue.String("selected_scene_index"));
        channelTransmitter.Send(message);
    }
    
    private void Awake()
    {
        // This delays to get the network ready
        const int delayForNetwork = 4;
        Invoke(nameof(GetSetState), delayForNetwork);
        
        // Collect GUI objects for GUI hue change
        _uiImages = guiCanvas.GetComponentsInChildren<Image>(true);
        _uiTexts = guiCanvas.GetComponentsInChildren<Text>(true);
        
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
