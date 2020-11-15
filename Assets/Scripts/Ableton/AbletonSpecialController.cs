using extOSC;
using UnityEngine;

public class AbletonSpecialController : MonoBehaviour
{
    private AbletonController liveController;
    private const int sceneIncrementAmount = 8;
    private readonly string[] bankClipPropertiesToGet = { "name", "color" };

    public void ScrollScenes(bool increase)
    {
        // Get next scene
        var currentSceneIndex = SettingsSingleton.Instance.ViewingIndex;
        var nextSceneIndex = increase ? currentSceneIndex + sceneIncrementAmount : currentSceneIndex - sceneIncrementAmount;
        if (nextSceneIndex < 0)
            nextSceneIndex = 0;
        
        SettingsSingleton.Instance.ViewingIndex = nextSceneIndex;

        // Get data for next set of clips
        for (var index = nextSceneIndex; index < nextSceneIndex + sceneIncrementAmount; index++)
        {
            // It is expected that all tracks have the same clips so this gets the clips and track 0
            this.liveController.CanonicalPath = $"/live_set/tracks/0/clip_slots/{index}/clip";

            foreach (var property in this.bankClipPropertiesToGet)
            {
                this.liveController.PropertyOrFunction = property;
                this.liveController.GetProperty();
            }
        }
    }

    public void FireClip(int trackIndex)
    {
        this.liveController.CanonicalPath = $"/live_set/tracks/{trackIndex}/clip_slots/{SettingsSingleton.Instance.NextSceneToPlay}/clip";
        this.liveController.PropertyOrFunction = "fire";
        this.liveController.CallFunction();
    }

    private void Start()
    {
        this.liveController = GetComponent<AbletonController>();
    }
}
