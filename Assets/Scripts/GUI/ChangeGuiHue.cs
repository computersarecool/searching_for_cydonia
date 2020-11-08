using extOSC;
using UnityEngine;

public class ChangeGuiHue : MonoBehaviour
{
    public void ChangeHue(float val)
    {
        var message = new OSCMessage($"{OSCCommunicationRouter.GuiHueAddress}");
        message.AddValue(OSCValue.Float(val));
        SettingsSingleton.Instance.unityOSCTransmitter.Send(message);
    }
}
