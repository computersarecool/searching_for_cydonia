using extOSC;
using MusicControl;
using System.Collections.Generic;
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
    private const string liveSetClipSlotsAddress = "/live_set/tracks/*/clip_slots/*";
    private const string liveSetClipAddress = "/live_set/tracks/*/clip_slots/*/clip";

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
        this.Receiver.Bind(liveSetClipSlotsAddress, UpdateClipSlots);
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
            case "tracks":
                List<Track> tracks = new List<Track>();
                for (var i = 1; i < message.Values.Count; i +=2)
                {
                    var id = $"{message.Values[i].StringValue} {message.Values[i + 1].IntValue}";
                    var canonicalPath = $"/live_set/tracks/{i - 1}";
                    tracks.Add(new Track(id, canonicalPath));
                }

                SettingsSingleton.Instance.Tracks = tracks;
                break;
        }
    }

    private static void UpdateTrack(OSCMessage message)
    {
        const int trackAddressIndex = 3;
        var addressArray = message.Address.Split('/');
        var trackIndex = int.Parse(addressArray[trackAddressIndex]);
        var property = message.Values[0].StringValue;

        switch (property)
        {
            case "clip_slots":
                List<ClipSlot> clipSlots = new List<ClipSlot>();
                for (var i = 1; i < message.Values.Count; i += 2)
                {
                    var id = $"{message.Values[i].StringValue} {message.Values[i + 1].IntValue}";
                    var canonicalPath = $"/live_set/tracks/{trackIndex}/clip_slots/{(i - 1) / 2}";
                    clipSlots.Add(new ClipSlot(id, canonicalPath));
                }
                SettingsSingleton.Instance.Tracks[trackIndex].ClipSlots = clipSlots;
                break;
        }
    }

    private static void UpdateClipSlots(OSCMessage message)
    {
        const int trackAddressIndex = 3;
        const int clipSlotAddressIndex = 5;
        var addressArray = message.Address.Split('/');
        var trackIndex = int.Parse(addressArray[trackAddressIndex]);
        var clipSlotIndex = int.Parse(addressArray[clipSlotAddressIndex]);
        var property = message.Values[0].StringValue;

        switch (property)
        {
            case "clip":
                var id = $"{message.Values[1].StringValue} {message.Values[2].IntValue}";
                var canonicalPath = $"/live_set/tracks/{trackIndex}/clip_slots/{clipSlotIndex}/clip";
                SettingsSingleton.Instance.Tracks[trackIndex].ClipSlots[clipSlotIndex].Clip = new Clip(id, canonicalPath);
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
                SettingsSingleton.Instance.Tracks[trackIndex].ClipSlots[clipIndex].Clip.Length = message.Values[1].FloatValue;
                break;
            case "name":
                SettingsSingleton.Instance.Tracks[trackIndex].ClipSlots[clipIndex].Clip.Name = message.Values[1].StringValue;
                break;
            case "color":
                SettingsSingleton.Instance.Tracks[trackIndex].ClipSlots[clipIndex].Clip.Color = message.Values[1].IntValue;
                break;
            case "pitch_coarse":
                SettingsSingleton.Instance.Tracks[trackIndex].ClipSlots[clipIndex].Clip.PitchCoarse = message.Values[1].IntValue;
                break;
        }
    }
}
