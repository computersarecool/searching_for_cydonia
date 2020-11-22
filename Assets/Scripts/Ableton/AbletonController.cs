using extOSC;
using UnityEngine;

public class AbletonController : MonoBehaviour
{
    public string CanonicalPath;
    public string PropertyOrFunction;

    public static void CallFunction(string canonicalPath, string function)
    {
        var message = new OSCMessage(canonicalPath);
        message.AddValue(OSCValue.String("call"));
        message.AddValue(OSCValue.String(function));
        SettingsSingleton.Instance.ExternalOSCTransmitter.Send(message);
    }

    public static void CallFunction(string canonicalPath, string function, float parameter)
    {
        var message = new OSCMessage(canonicalPath);
        message.AddValue(OSCValue.String("call"));
        message.AddValue(OSCValue.String(function));
        message.AddValue(OSCValue.Float(parameter));
        SettingsSingleton.Instance.ExternalOSCTransmitter.Send(message);
    }

    public static void GetProperty(string canonicalPath, string property)
    {
        var message = new OSCMessage(canonicalPath);
        message.AddValue(OSCValue.String("get"));
        message.AddValue(OSCValue.String(property));
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
