using extOSC;
using UnityEngine;

public class AbletonController : MonoBehaviour
{
    public string CanonicalPath;
    public string PropertyOrFunction;
    
    public void CallFunction()
    {
        var message = new OSCMessage($"{this.CanonicalPath}");
        message.AddValue(OSCValue.String("call"));
        message.AddValue(OSCValue.String(this.PropertyOrFunction));
        SettingsSingleton.Instance.ExternalOSCTransmitter.Send(message);
    }
    
    public void GetProperty()
    {
        var message = new OSCMessage($"{this.CanonicalPath}");
        message.AddValue(OSCValue.String("get"));
        message.AddValue(OSCValue.String(this.PropertyOrFunction));
        SettingsSingleton.Instance.ExternalOSCTransmitter.Send(message);
    }

    public void SetProperty(float val)
    {
        var message = new OSCMessage($"{this.CanonicalPath}");
        message.AddValue(OSCValue.String("set"));
        message.AddValue(OSCValue.String(this.PropertyOrFunction));
        message.AddValue(OSCValue.Float(val));
        SettingsSingleton.Instance.ExternalOSCTransmitter.Send(message);
    }
}
