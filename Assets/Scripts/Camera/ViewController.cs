using extOSC;
using UnityEngine;
using UnityEngine.EventSystems;

public class ViewController: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pressed;
    private const int moveAmount = 5;

    public void OnPointerDown(PointerEventData eventData)
    {
        this.pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.pressed = false;
    }

    private void Update()
    {
        if (this.pressed)
        {
            MoveForward();
        }
    }

    public void MoveForward()
    {
        var vector = SettingsSingleton.Instance.AttachedEnvironment.transform.position + moveAmount * Time.deltaTime * SettingsSingleton.Instance.MainCamera.transform.forward;
        // TODO: Update from sync
        SettingsSingleton.Instance.AttachedEnvironment.transform.position = vector;
        SendOSCUpdate($"{OSCCommunicationRouter.CameraMoveAddress}", vector);
    }

    public void MoveBackward()
    {
        var vector = SettingsSingleton.Instance.AttachedEnvironment.transform.position - moveAmount * Time.deltaTime * SettingsSingleton.Instance.MainCamera.transform.forward;
        SettingsSingleton.Instance.AttachedEnvironment.transform.position = vector;
        SendOSCUpdate($"{OSCCommunicationRouter.CameraMoveAddress}", vector);
    }

    public void RotateView(Vector2 rotationAmount)
    {
        var vector = new Vector3(rotationAmount.y, -rotationAmount.x, 0);
        SettingsSingleton.Instance.MainCamera.transform.eulerAngles = vector;
        SendOSCUpdate($"{OSCCommunicationRouter.CameraRotateAddress}", vector);
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
