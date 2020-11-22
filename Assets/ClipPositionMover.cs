using UnityEngine;
using UnityEngine.EventSystems;

public class ClipPositionMover : MonoBehaviour, IPointerDownHandler
{
    private int trackIndex;
    public int beatsToMove;

    private void Start()
    {
        this.trackIndex = GetComponentInParent<TrackContainer>().TrackIndex;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var clipIndex = SettingsSingleton.Instance.LiveSet.tracks[this.trackIndex].PlayingSlotIndex;
        var canonicalPath = $"/live_set/tracks/{this.trackIndex}/clip_slots/{clipIndex}/clip";
        AbletonController.CallFunction(canonicalPath, "move_playing_pos", this.beatsToMove);
    }
}
