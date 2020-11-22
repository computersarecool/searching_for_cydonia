using extOSC;
using MusicControl;
using System.Collections.Generic;
using UnityEngine;

public class OSCCommunicationRouter : MonoBehaviour
{
    public OSCReceiver Receiver;

    #region Network Addresses
    // From this app
    public const string CameraRotationAddress = "/camera_rotate";
    public const string CameraPositionAddress = "/camera_move";
    public const string GUIHueAddress = "/gui_hue";

    // From others
    // TODO: Make come from this app
    private const string timeAddress = "/central/time";

    // From Live
    private const string liveEQBandAddress = "/eq/band/*";
    private const string liveSetAddress = "/live_set";
    private const string liveSetTracksAddress = "/live_set/tracks/*";
    private const string liveSetClipSlotsAddress = "/live_set/tracks/*/clip_slots/*";
    private const string liveSetClipAddress = "/live_set/tracks/*/clip_slots/*/clip";

    #endregion

    #region OSC Address Indices
    const int eqBandAddressIndex = 3;
    const int trackAddressIndex = 3;
    const int clipSlotAddressIndex = 5;
    #endregion

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

    #region From this app
    // TODO: Change to set properties directly
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
    #endregion

    #region From Others
    private static void UpdateTime(OSCMessage message)
    {
        if (!message.ToFloat(out var value)) return;

        SettingsSingleton.Instance.Clock = value;
    }

    #endregion

    #region From Live
    private static void UpdateEQBand(OSCMessage message)
    {
        if (!message.ToFloat(out var val)) return;

        var addressArray = message.Address.Split('/');
        var bandIndex = int.Parse(addressArray[eqBandAddressIndex]);

        SettingsSingleton.Instance.AudioEqualizer.UpdateBand(bandIndex, val);
    }

    private static void UpdateSet(OSCMessage message)
    {
        var property = message.Values[0].StringValue;

        switch (property)
        {
            case "tempo":
                SettingsSingleton.Instance.LiveSet.Tempo = message.Values[1].FloatValue;
                break;
            case "tracks":
                List<Track> tracks = new List<Track>();
                for (var i = 1; i < message.Values.Count; i += 2)
                {
                    var id = $"{message.Values[i].StringValue} {message.Values[i + 1].IntValue}";
                    var canonicalPath = $"/live_set/tracks/{tracks.Count}";
                    tracks.Add(new Track(id, canonicalPath, tracks.Count));
                }

                SettingsSingleton.Instance.LiveSet.tracks = tracks;
                break;
        }
    }

    private static void UpdateTrack(OSCMessage message)
    {
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
                    clipSlots.Add(new ClipSlot(id, canonicalPath, trackIndex));
                }
                SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots = clipSlots;
                break;
            case "playing_slot_index":
                SettingsSingleton.Instance.LiveSet.tracks[trackIndex].PlayingSlotIndex = message.Values[1].IntValue;
                break;
        }
    }

    private static void UpdateClipSlots(OSCMessage message)
    {
        // TODO: Extract to function
        var addressArray = message.Address.Split('/');
        var trackIndex = int.Parse(addressArray[trackAddressIndex]);
        var clipSlotIndex = int.Parse(addressArray[clipSlotAddressIndex]);
        var property = message.Values[0].StringValue;

        switch (property)
        {
            case "clip":
                var id = $"{message.Values[1].StringValue} {message.Values[2].IntValue}";
                var canonicalPath = $"/live_set/tracks/{trackIndex}/clip_slots/{clipSlotIndex}/clip";
                SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip = new Clip(id, canonicalPath, trackIndex);
                break;
        }
    }

    private static void UpdateClip(OSCMessage message)
    {
        // TODO: Extract to function
        var addressArray = message.Address.Split('/');
        var trackIndex = int.Parse(addressArray[trackAddressIndex]);
        var clipSlotIndex = int.Parse(addressArray[clipSlotAddressIndex]);
        var property = message.Values[0].StringValue;

        switch (property)
        {
            case "playing_position":
                SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip.PlayingPosition = message.Values[1].FloatValue;
                break;
            case "length":
                SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip.Length = message.Values[1].FloatValue;
                break;
            case "name":
                SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip.Name = message.Values[1].StringValue;
                break;
            case "color":
                SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip.Color = message.Values[1].IntValue;
                break;
            case "pitch_coarse":
                SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip.PitchCoarse = message.Values[1].IntValue;
                break;
        }
    }
    #endregion
}

