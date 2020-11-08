using extOSC;
using UnityEngine;

public class AbletonController : MonoBehaviour
{
    public string canonicalPath;
    public string propertyOrFunction;
    
    public void CallFunction()
    {
        var message = new OSCMessage($"{canonicalPath}/call");
        message.AddValue(OSCValue.String(propertyOrFunction));
        SettingsSingleton.Instance.externalOSCTransmitter.Send(message);
    }
    
    public void GetProperty()
    {
        var message = new OSCMessage($"{canonicalPath}/get");
        message.AddValue(OSCValue.String(propertyOrFunction));
        SettingsSingleton.Instance.externalOSCTransmitter.Send(message);
    }
    
    // Dynamic float
    public void SetProperty(float val)
    {
        var message = new OSCMessage($"{canonicalPath}/set/{propertyOrFunction}");
        message.AddValue(OSCValue.Float(val));
        SettingsSingleton.Instance.externalOSCTransmitter.Send(message);
    }
    
    // Special case
    public void MovePlayingPosition(int val)
    {
        var message = new OSCMessage($"{canonicalPath}");
        message.AddValue(OSCValue.Int(val));
        SettingsSingleton.Instance.externalOSCTransmitter.Send(message);
    }
}
