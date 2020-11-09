using UnityEngine;

public class AbletonSpecialController : MonoBehaviour
{
    private AbletonController _liveController;
    private const int SceneIncrementAmount = 8;
    private readonly string[] _bankClipPropertiesToGet = { "name", "color" };
    
    public void IncrementalTempoChange(bool increase)
    {
        var currentTempo = SettingsSingleton.Instance.LiveSetTempo;
        var nextTempo = increase ? currentTempo + 1 : currentTempo - 1;
        _liveController.SetProperty(nextTempo);
    }

    public void AdvanceScene(bool increase)
    {
        // Get next scene
        var currentSceneIndex = SettingsSingleton.Instance.viewingIndex;
        var nextSceneIndex = increase ? currentSceneIndex + SceneIncrementAmount : currentSceneIndex - SceneIncrementAmount;
        if (nextSceneIndex < 0)
            nextSceneIndex = 0;
        
        SettingsSingleton.Instance.viewingIndex = nextSceneIndex;

        // Get data for next set of clips
        for (var index = nextSceneIndex; index < nextSceneIndex + SceneIncrementAmount; index++)
        {
            // It is expected that all tracks have the same clips so this gets the clips and track 0
            _liveController.canonicalPath = $"/live_set/tracks/0/clip_slots/{index}/clip";

            foreach (var property in _bankClipPropertiesToGet)
            {
                _liveController._propertyOrFunction = property;
                _liveController.GetProperty();
            }
        }
    }

    public void FireClip(int trackIndex)
    {
        _liveController.canonicalPath = $"/live_set/tracks/{trackIndex}/clip_slots/{SettingsSingleton.Instance.nextTrackToPlayIndex}/clip";
        _liveController._propertyOrFunction = "fire";
        _liveController.CallFunction();
    }

    private void Start()
    {
        _liveController = GetComponent<AbletonController>();
    }
}
