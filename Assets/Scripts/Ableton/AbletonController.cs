using extOSC;
using UnityEngine;

public class AbletonController : MonoBehaviour
{
    public string canonicalPath;
    public string propertyOrFunction;
    
    public void CallFunction()
    {
        var message = new OSCMessage($"{canonicalPath}");
        message.AddValue(OSCValue.String("call"));
        message.AddValue(OSCValue.String(propertyOrFunction));
        SettingsSingleton.Instance.externalOSCTransmitter.Send(message);
    }
    
    public void GetProperty()
    {
        var message = new OSCMessage($"{canonicalPath}");
        message.AddValue(OSCValue.String("get"));
        message.AddValue(OSCValue.String(propertyOrFunction));
        SettingsSingleton.Instance.externalOSCTransmitter.Send(message);
    }

    public void SetProperty(float val)
    {
        var message = new OSCMessage($"{canonicalPath}");
        message.AddValue(OSCValue.String("set"));
        message.AddValue(OSCValue.String(propertyOrFunction));
        message.AddValue(OSCValue.Float(val));
        SettingsSingleton.Instance.externalOSCTransmitter.Send(message);
    }
}
