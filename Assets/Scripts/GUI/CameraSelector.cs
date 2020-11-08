using System;
using UnityEngine;

public class CameraSelector : MonoBehaviour
{
    public void SelectCamera(int index)
    {
        SettingsSingleton.Instance.cameraIndex = index;
    }
}
