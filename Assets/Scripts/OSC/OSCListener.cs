using DAW;
using extOSC;
using System.Collections.Generic;
using UnityEngine;

public class OSCListener
{
    // TODO: Separate into parser and listener

    // From this app
    public const string CameraRotationAddress = "/camera_rotate";
    public const string CameraPositionAddress = "/camera_move";
    public const string GUIAddress = "/gui";
    public const string GUIHueProperty = "hue";

    // From others
    // TODO: Make come from this app
    private const string timeAddress = "/central/time";

    // From Live
    private const string liveEQBandAddress = "/eq/band/*";
    private const string liveSetAddress = "/live_set";
    private const string liveSetTracksAddress = "/live_set/tracks/*";
    private const string liveSetClipSlotsAddress = "/live_set/tracks/*/clip_slots/*";
    private const string liveSetClipAddress = "/live_set/tracks/*/clip_slots/*/clip";

    private GUIManager guiManager;

    public OSCListener (OSCReceiver receiver)
    {
        this.guiManager = (GUIManager)Object.FindObjectOfType(typeof(GUIManager));

        // From this app
        receiver.Bind(CameraRotationAddress, UpdateCameraRotation);
        receiver.Bind(CameraPositionAddress, UpdateCameraPosition);
        receiver.Bind(GUIAddress, UpdateGUI);

        // From others
        receiver.Bind(timeAddress, UpdateTime);

        // From Live
        receiver.Bind(liveEQBandAddress, UpdateEQBand);
        receiver.Bind(liveSetAddress, UpdateSet);
        receiver.Bind(liveSetTracksAddress, UpdateTrack);
        receiver.Bind(liveSetClipAddress, UpdateClip);
        receiver.Bind(liveSetClipSlotsAddress, UpdateClipSlots);
    }

    #region OSC Address Indices

    const int eqBandAddressIndex = 3;
    const int trackAddressIndex = 3;
    const int clipSlotAddressIndex = 5;

    #endregion

    #region From This App

    private void UpdateGUI(OSCMessage message)
    {
        var property = message.Values[0].StringValue;
        var value = message.Values[1].FloatValue;

        switch (property)
        {
            case GUIHueProperty:
                guiManager.Hue = value;
                break;
        }
    }

    // TODO: Set properties directly
    private static void UpdateCameraRotation(OSCMessage message)
    {
        if (!message.ToVector3(out var vector)) return;

        //SettingsSingleton.Instance.UpdateCameraRotation(vector);
    }

    public void UpdateCameraPosition(OSCMessage message)
    {
        if (!message.ToVector3(out var vector)) return;

        //SettingsSingleton.Instance.MoveMainCameraAndEnvironment(vector);
    }

    #endregion

    #region From Others
    public void UpdateTime(OSCMessage message)
    {
        if (!message.ToFloat(out var value)) return;

        //SettingsSingleton.Instance.Clock = value;
    }

    #endregion

    #region From Live
    public void UpdateEQBand(OSCMessage message)
    {
        if (!message.ToFloat(out var val)) return;

        var addressArray = message.Address.Split('/');
        var bandIndex = int.Parse(addressArray[eqBandAddressIndex]);

        //SettingsSingleton.Instance.AudioEqualizer.UpdateBand(bandIndex, val);
    }

    public static void UpdateSet(OSCMessage message)
    {
        var property = message.Values[0].StringValue;

        switch (property)
        {
            case "tempo":
                break;
            //     SettingsSingleton.Instance.LiveSet.Tempo = message.Values[1].FloatValue;
            //     break;
            // case "tracks":
            //     List<Track> tracks = new List<Track>();
            //     for (var i = 1; i < message.Values.Count; i += 2)
            //     {
            //         var id = $"{message.Values[i].StringValue} {message.Values[i + 1].IntValue}";
            //         var canonicalPath = $"/live_set/tracks/{tracks.Count}";
            //         tracks.Add(new Track(id, canonicalPath, tracks.Count));
            //     }
            //
            //     SettingsSingleton.Instance.LiveSet.tracks = tracks;
            //     break;
        }
    }

    public static void UpdateTrack(OSCMessage message)
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
                //SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots = clipSlots;
                break;
            case "playing_slot_index":
                //SettingsSingleton.Instance.LiveSet.tracks[trackIndex].PlayingSlotIndex = message.Values[1].IntValue;
                break;
        }
    }

    public static void UpdateClipSlots(OSCMessage message)
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
                //SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip = new Clip(id, canonicalPath, trackIndex);
                break;
        }
    }

    public static void UpdateClip(OSCMessage message)
    {
        // TODO: Extract to function
        var addressArray = message.Address.Split('/');
        var trackIndex = int.Parse(addressArray[trackAddressIndex]);
        var clipSlotIndex = int.Parse(addressArray[clipSlotAddressIndex]);
        var property = message.Values[0].StringValue;

        switch (property)
        {
            case "playing_position":
                //SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip.PlayingPosition = message.Values[1].FloatValue;
                break;
            case "length":
                //SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip.Length = message.Values[1].FloatValue;
                break;
            case "name":
                //SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip.Name = message.Values[1].StringValue;
                break;
            case "color":
                //SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip.Color = message.Values[1].IntValue;
                break;
            case "pitch_coarse":
                //SettingsSingleton.Instance.LiveSet.tracks[trackIndex].ClipSlots[clipSlotIndex].Clip.PitchCoarse = message.Values[1].IntValue;
                break;
        }
    }
    #endregion
}

