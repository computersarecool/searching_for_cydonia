using extOSC;
using UnityEngine;

public class OSCCommunicationRouter : MonoBehaviour
{
    public OSCReceiver Receiver;

    // From Unity
    public const string CameraRotateAddress = "/camera_rotate";
    public const string CameraMoveAddress = "/camera_move";
    public const string GuiHueAddress = "/gui_hue";

    // From others
    private const string timeAddress = "/central/time";

    // From Live
    private const string liveEqBandAddress = "/eq/band/*";
    private const string liveTempoAddress = "/live_set/tempo";
    private const string liveClipNameAddress = "/live_set/tracks/0/clip_slots/*/clip/name";
    private const string clipColorsAddress = "/live_set/tracks/0/clip_slots/*/clip/color";
    private const string livePlayingClipPositionAddress = "/live_set/tracks/*/playing_clip/playing_position";
    private const string playingClipLengthAddress = "/live_set/tracks/*/playing_clip/length";
    private const string playingSlotIndexAddress = "/live_set/tracks/*/playing_clip/name";

    #region MonoBehaviors

    public void Start()
    {
        // From Unity
        this.Receiver.Bind(CameraRotateAddress, RotateMainCamera);
        this.Receiver.Bind(CameraMoveAddress, MoveMainCamera);
        this.Receiver.Bind(GuiHueAddress, UpdateGuiHue);

        // From others
        this.Receiver.Bind(timeAddress, UpdateTime);

        // From Live
        this.Receiver.Bind(liveEqBandAddress, UpdateBand);
        this.Receiver.Bind(liveTempoAddress, UpdateTempo);
        this.Receiver.Bind(liveClipNameAddress, UpdateClipName);
        this.Receiver.Bind(clipColorsAddress, UpdateClipColor);
        this.Receiver.Bind(livePlayingClipPositionAddress, UpdateTrackPosition);
        this.Receiver.Bind(playingClipLengthAddress, UpdateClipLength);
        this.Receiver.Bind(playingSlotIndexAddress, UpdatePlayingSlotIndex);
    }

    #endregion

    // From Unity
    private static void RotateMainCamera(OSCMessage message)
    {
        if (!message.ToVector3(out var vector)) return;

        SettingsSingleton.Instance.UpdateCameraRotation(vector);
    }

    private static void MoveMainCamera(OSCMessage message)
    {
        if (!message.ToVector3(out var vector)) return;

        SettingsSingleton.Instance.MoveMainCameraAndEnvironment(vector);
    }

    private static void UpdateGuiHue(OSCMessage message)
    {
        if (!message.ToFloat(out var value)) return;

        SettingsSingleton.Instance.UpdateGUIHue(value);
    }

    // From others
    private static void UpdateTime(OSCMessage message)
    {
        if (!message.ToFloat(out var value)) return;

        SettingsSingleton.Instance.Clock = value;
    }

    // From Live
    private static void UpdateBand(OSCMessage message)
    {
        if (!message.ToFloat(out var val)) return;

        const int trackAddressIndex = 3;
        var addressArray = message.Address.Split('/');
        var bandIndex = int.Parse(addressArray[trackAddressIndex]);

        SettingsSingleton.Instance.AudioEqualizer.UpdateBand(bandIndex, val);
    }

    private static void UpdateTempo(OSCMessage message)
    {
        if (!message.ToFloat(out var val)) return;

        SettingsSingleton.Instance.LiveSetTempo = val;
    }

    private static void UpdateClipName(OSCMessage message)
    {
        if (!message.ToString(out var data)) return;

        const int clipAddressIndex = 5;
        var addressArray = message.Address.Split('/');
        var clipIndex = int.Parse(addressArray[clipAddressIndex]);

        SettingsSingleton.Instance.UpdateClipName(clipIndex, data);
    }

    private static void UpdateClipColor(OSCMessage message)
    {
        if (!message.ToInt(out var color)) return;

        const int clipAddressIndex = 5;
        var addressArray = message.Address.Split('/');
        var clipIndex = int.Parse(addressArray[clipAddressIndex]);

        SettingsSingleton.Instance.UpdateClipColor(clipIndex, color);
    }

    private static void UpdateTrackPosition(OSCMessage message)
    {
        if (!message.ToFloat(out var position)) return;

        const int trackAddressIndex = 3;
        var addressArray = message.Address.Split('/');
        var trackIndex = int.Parse(addressArray[trackAddressIndex]);

        SettingsSingleton.Instance.UpdatePlayingClipPosition(trackIndex, position);
    }

    private static void UpdateClipLength(OSCMessage message)
    {
        if (!message.ToFloat(out var length)) return;

        const int trackAddressIndex = 3;
        var addressArray = message.Address.Split('/');
        var trackIndex = int.Parse(addressArray[trackAddressIndex]);

        SettingsSingleton.Instance.UpdateClipLength(trackIndex, length);
    }

    private static void UpdatePlayingSlotIndex(OSCMessage message)
    {
        if (!message.ToString(out var name)) return;

        const int trackAddressIndex = 3;
        var addressArray = message.Address.Split('/');
        var trackIndex = int.Parse(addressArray[trackAddressIndex]);

        SettingsSingleton.Instance.UpdatePlayingClipName(trackIndex, name);
    }
}
