using System;
using UnityEngine;

public class StoreNextTrackIndex : MonoBehaviour
{
    public void StoreTrackIndex()
    {
        var objectName = gameObject.name;
        var index = objectName.Split(' ')[1];
        var numericalIndex = Convert.ToInt32(index);

        SettingsSingleton.Instance.nextTrackToPlayIndex = numericalIndex + SettingsSingleton.Instance.viewingIndex;
    }
}
