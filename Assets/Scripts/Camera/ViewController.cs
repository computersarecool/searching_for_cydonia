using extOSC;
using UnityEngine;

public class ViewController: MonoBehaviour
{
    private const int moveAmount = 45;

    public void Move(bool forward)
    {
        var vector = forward
            ? SettingsSingleton.Instance.AttachedEnvironment.transform.position + moveAmount * Time.deltaTime * SettingsSingleton.Instance.MainCamera.transform.forward
            : SettingsSingleton.Instance.AttachedEnvironment.transform.position - moveAmount * Time.deltaTime * SettingsSingleton.Instance.MainCamera.transform.forward;

        SendOSCUpdate($"{OSCCommunicationRouter.CameraPositionAddress}", vector);
    }

    public void RotateView(Vector2 rotationAmount)
    {
        var vector = new Vector3(-rotationAmount.y, rotationAmount.x, 0);
        SendOSCUpdate($"{OSCCommunicationRouter.CameraRotationAddress}", vector);
    }

    private void SendOSCUpdate(string toAddress, Vector3 vector)
    {
        var message = new OSCMessage(toAddress);
        message.AddValue(OSCValue.Float(vector.x));
        message.AddValue(OSCValue.Float(vector.y));
        message.AddValue(OSCValue.Float(vector.z));
        SettingsSingleton.Instance.UnityOSCTransmitter.Send(message);
    }
}
