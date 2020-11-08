using System;
using UnityEngine;

public class InterfaceSelector : MonoBehaviour
{
    public void SelectInterface(int index)
    {
        SettingsSingleton.Instance.InterfaceIndex = index;
    }
}
