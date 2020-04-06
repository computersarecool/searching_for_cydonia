using System;
using UnityEngine;

public class PickCamera : MonoBehaviour
{
    public void PickACamera()
    {
        var objectName = gameObject.name;
        var index = objectName.Split(' ')[1];
        var numericalIndex = Convert.ToInt32(index);
        
        SettingsSingleton.Instance.cameraIndex = numericalIndex;
    }
}
