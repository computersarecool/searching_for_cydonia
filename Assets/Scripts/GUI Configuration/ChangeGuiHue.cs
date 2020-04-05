using extOSC;
using UnityEngine;

public class ChangeGuiHue : MonoBehaviour
{
    public void ChangeHue(float val)
    {
        var message = new OSCMessage($"{Constants.GuiHueAddress}");
        message.AddValue(OSCValue.Float(val));
        SettingsSingleton.Instance.unityBroadcastTransmitter.Send(message);
    }
}
