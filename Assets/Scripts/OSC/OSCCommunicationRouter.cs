using extOSC;
using UnityEngine;

public class OSCCommunicationRouter : MonoBehaviour
{
    public OSCReceiver receiver;

    // From Unity
    public const string CameraRotateAddress = "/camera_rotate";
    public const string CameraMoveAddress = "/camera_move";
    public const string GuiHueAddress = "/gui_hue";

    // From others
    private const string TimeAddress = "/central/time";

    // From Live
    private const string LiveEqBandAddress = "/eq/band/*";
    private const string LiveTempoAddress = "/live_set/tempo";
    private const string LiveClipNameAddress = "/live_set/tracks/0/clip_slots/*/clip/name";
    private const string ClipColorsAddress = "/live_set/tracks/0/clip_slots/*/clip/color";
    private const string LivePlayingClipPositionAddress = "/live_set/tracks/*/playing_clip/playing_position";
    private const string PlayingClipLengthAddress = "/live_set/tracks/*/playing_clip/length";
    private const string PlayingSlotIndexAddress = "/live_set/tracks/*/playing_clip/name";

    public void Start()
    {
        // From Unity
        receiver.Bind(CameraRotateAddress, RotateMainCamera);
        receiver.Bind(CameraMoveAddress, MoveMainCamera);
        receiver.Bind(GuiHueAddress, UpdateGuiHue);

        // From others
        receiver.Bind(TimeAddress, UpdateTime);

        // From Live
        receiver.Bind(LiveEqBandAddress, UpdateBand);
        receiver.Bind(LiveTempoAddress, UpdateTempo);
        receiver.Bind(LiveClipNameAddress, UpdateClipName);
        receiver.Bind(ClipColorsAddress, UpdateClipColor);
        receiver.Bind(LivePlayingClipPositionAddress, UpdateTrackPosition);
        receiver.Bind(PlayingClipLengthAddress, UpdateClipLength);
        receiver.Bind(PlayingSlotIndexAddress, UpdatePlayingSlotIndex);
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

        SettingsSingleton.Instance.clock = value;
    }
}
