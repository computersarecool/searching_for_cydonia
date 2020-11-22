using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackContainer : MonoBehaviour
{
    private const int maxButtons = 6;
    public GameObject ClipButtonLayout;
    public GameObject ClipFireButton;
    public int TrackIndex;

    private int clipIndexOffset;

    public void UpdateOffset(bool increase)
    {
        if (!increase)
        {
            this.clipIndexOffset = Math.Max(0, this.clipIndexOffset - maxButtons);
        }
        else if (this.clipIndexOffset + maxButtons > SettingsSingleton.Instance.LiveSet.tracks[this.TrackIndex].ClipSlots.Count)
        {
            return;
        }
        else
        {
            this.clipIndexOffset += maxButtons;
        }

        this.UpdateButtons();
    }

    private void OnEnable()
    {
        this.UpdateButtons();
    }

    private void UpdateButtons()
    {
        // Destroy old buttons
        var clipFireButtons = this.ClipButtonLayout.GetComponentsInChildren<ClipFireButton>();
        foreach (var button in clipFireButtons)
        {
            Destroy(button.gameObject);
        }

        if (SettingsSingleton.Instance == null)
        {
            return;
        }

        // Create new buttons
        var buttonList = new List<GameObject>();
        for (var i = clipIndexOffset; i < clipIndexOffset + maxButtons; i++)
        {
            if (i >= SettingsSingleton.Instance.LiveSet.tracks[this.TrackIndex].ClipSlots.Count) break;

            buttonList.Add(Instantiate(ClipFireButton, this.ClipButtonLayout.transform));
        }

        // Set button values
        for (var i = 0; i < buttonList.Count; i++)
        {
            var clipFireButtonComp = buttonList[i].GetComponentInChildren<ClipFireButton>();
            var trueIndex = this.clipIndexOffset + i;
            clipFireButtonComp.CanonicalPath = $"/live_set/tracks/{this.TrackIndex}/clip_slots/{trueIndex}/clip";
            clipFireButtonComp.Name = SettingsSingleton.Instance.LiveSet.tracks[this.TrackIndex].ClipSlots[trueIndex].Clip.Name;
            clipFireButtonComp.Color = SettingsSingleton.Instance.LiveSet.tracks[this.TrackIndex].ClipSlots[trueIndex].Clip.Color;
        }
    }
}
