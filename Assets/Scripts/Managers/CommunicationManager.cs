using extOSC;
using UnityEngine;

public class CommunicationManager : MonoBehaviour
{
    private OSCListener oscListener;

    // TODO: Make programatically
    public OSCTransmitter primaryTransmitter;
    public OSCTransmitter secondaryTransmitter;
    public OSCReceiver receiver;

    public void Start()
    {
        this.oscListener = new OSCListener(receiver);
    }

    // TODO: Make variadic function
    public void CallDAWFunction(string canonicalPath, string function)
    {
        var message = new OSCMessage(canonicalPath);
        message.AddValue(OSCValue.String("call"));
        message.AddValue(OSCValue.String(function));
        this.primaryTransmitter.Send(message);
    }

    public void CallDAWFunction(string canonicalPath, string function, float parameter)
    {
        var message = new OSCMessage(canonicalPath);
        message.AddValue(OSCValue.String("call"));
        message.AddValue(OSCValue.String(function));
        message.AddValue(OSCValue.Float(parameter));
        this.primaryTransmitter.Send(message);
    }

    public void GetDAWProperty(string canonicalPath, string property)
    {
        var message = new OSCMessage(canonicalPath);
        message.AddValue(OSCValue.String("get"));
        message.AddValue(OSCValue.String(property));
        this.primaryTransmitter.Send(message);
    }

    public void SetDAWProperty(string canonicalPath, string property, float value)
    {
        var message = new OSCMessage(canonicalPath);
        message.AddValue(OSCValue.String("set"));
        message.AddValue(OSCValue.String(property));
        message.AddValue(OSCValue.Float(value));
        this.primaryTransmitter.Send(message);
    }

    public void SetAppProperty(string canonicalPath, string property, float value)
    {
        var message = new OSCMessage(canonicalPath);
        message.AddValue(OSCValue.String(property));
        message.AddValue(OSCValue.Float(value));
        this.primaryTransmitter.Send(message);
    }
}
