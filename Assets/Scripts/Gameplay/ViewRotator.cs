using extOSC;
using UnityEngine;

public class ViewRotator: MonoBehaviour
{
    public void RotateView(Vector2 rotationAmount)
    {
        var vector = new Vector3(rotationAmount.y, -rotationAmount.x, 0);
        SettingsSingleton.Instance.mainCamera.transform.eulerAngles = vector;
        
        var message = new OSCMessage($"{Constants.CameraRotateAddress}");
        message.AddValue(OSCValue.Float(vector.x));
        message.AddValue(OSCValue.Float(vector.y));
        message.AddValue(OSCValue.Float(vector.z));

        SettingsSingleton.Instance.unityBroadcastTransmitter.Send(message);
    }
}

