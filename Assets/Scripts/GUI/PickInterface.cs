using System;
using UnityEngine;

public class PickInterface : MonoBehaviour
{
    public void PickAnInterface()
    {
        var objectName = gameObject.name;
        var index = objectName.Split(' ')[1];
        var numericalIndex = Convert.ToInt32(index);
        
        SettingsSingleton.Instance.InterfaceIndex = numericalIndex;
    }
}
