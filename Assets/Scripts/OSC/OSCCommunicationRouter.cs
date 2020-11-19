using extOSC;
using UnityEngine;

public class OSCCommunicationRouter : MonoBehaviour
{
    public OSCReceiver Receiver;

    // From this app
    public const string CameraRotationAddress = "/camera_rotate";
    public const string CameraPositionAddress = "/camera_move";
    public const string GUIHueAddress = "/gui_hue";

    // From others
    // TODO: Make come from this app
    private const string timeAddress = "/central/time";

    // From Live
    private const string liveEQBandAddress = "/eq/band/*";
    // TODO: Make more like a router with multiple wildcards
    private const string liveSetAddress = "/live_set";
    private const string liveSetTracksAddress = "/live_set/tracks/*";
    private const string liveSetClipAddress= "/live_set/tracks/*/clip_slots/*/clip";

    #region MonoBehaviors

    public void Start()
    {
        // From this app
        this.Receiver.Bind(CameraRotationAddress, UpdateCameraRotation);
        this.Receiver.Bind(CameraPositionAddress, UpdateCameraPosition);
        this.Receiver.Bind(GUIHueAddress, UpdateGUIHue);

        // From others
        this.Receiver.Bind(timeAddress, UpdateTime);

        // From Live
        this.Receiver.Bind(liveEQBandAddress, UpdateEQBand);
        this.Receiver.Bind(liveSetAddress, UpdateSet);
        this.Receiver.Bind(liveSetTracksAddress, UpdateTrack);
        this.Receiver.Bind(liveSetClipAddress, UpdateClip);
    }

    #endregion

    // From Unity
    private static void UpdateCameraRotation(OSCMessage message)
    {
        if (!message.ToVector3(out var vector)) return;

        SettingsSingleton.Instance.UpdateCameraRotation(vector);
    }

    private static void UpdateCameraPosition(OSCMessage message)
    {
        if (!message.ToVector3(out var vector)) return;

        SettingsSingleton.Instance.MoveMainCameraAndEnvironment(vector);
    }

    private static void UpdateGUIHue(OSCMessage message)
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
    private static void UpdateEQBand(OSCMessage message)
    {
        if (!message.ToFloat(out var val)) return;

        const int bandAddressIndex = 3;
        var addressArray = message.Address.Split('/');
        var bandIndex = int.Parse(addressArray[bandAddressIndex]);

        SettingsSingleton.Instance.AudioEqualizer.UpdateBand(bandIndex, val);
    }

    private static void UpdateSet(OSCMessage message)
    {
        var property = message.Values[0].StringValue;

        switch (property)
        {
            case "tempo":
                SettingsSingleton.Instance.LiveSetTempo = message.Values[1].FloatValue;
                break;
        }
    }

    private static void UpdateTrack(OSCMessage message)
    {
        const int trackAddressIndex = 3;
        var addressArray = message.Address.Split('/');
        if (addressArray.Length > 4) return; // This would be a child of track

        var trackIndex = int.Parse(addressArray[trackAddressIndex]);
        var property = message.Values[0].StringValue;

        switch (property)
        {
            case "playing_slot_index":
                SettingsSingleton.Instance.UpdatePlayingSlotIndex(trackIndex, message.Values[1].IntValue);
                break;
        }
    }

    private static void UpdateClip(OSCMessage message)
    {
        const int trackAddressIndex = 3;
        const int clipAddressIndex = 5;
        var addressArray = message.Address.Split('/');
        var trackIndex = int.Parse(addressArray[trackAddressIndex]);
        var clipIndex = int.Parse(addressArray[clipAddressIndex]);
        var property = message.Values[0].StringValue;

        switch (property)
        {
            case "length":
                SettingsSingleton.Instance.UpdatePlayingClipsLength(trackIndex, clipIndex, message.Values[1].FloatValue);
                break;
            case "name":
                SettingsSingleton.Instance.UpdatePlayingClipsName(trackIndex, clipIndex, message.Values[1].StringValue);
                break;
            case "playing_position":
                SettingsSingleton.Instance.UpdatePlayingClipsPosition(trackIndex, clipIndex, message.Values[1].FloatValue);
                break;
        }
    }

    // TODO: Remove these
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
