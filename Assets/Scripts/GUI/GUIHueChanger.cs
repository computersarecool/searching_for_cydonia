using extOSC;
using UnityEngine;

public class GUIHueChanger : MonoBehaviour
{
    public void ChangeHue(float val)
    {
        var message = new OSCMessage($"{OSCCommunicationRouter.GuiHueAddress}");
        message.AddValue(OSCValue.Float(val));
        SettingsSingleton.Instance.UnityOSCTransmitter.Send(message);
    }
}
