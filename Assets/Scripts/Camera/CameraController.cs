using extOSC;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool ForwardMovement;
    
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

    public void Update()
    {
        if (this.pressed)
        {
            Move(this.ForwardMovement);
        }
    }

    private static void Move(bool goForward)
    {
        var newPosition = goForward
            ? SettingsSingleton.Instance.AttachedEnvironment.transform.position + moveAmount * Time.deltaTime * SettingsSingleton.Instance.MainCamera.transform.forward
            : SettingsSingleton.Instance.AttachedEnvironment.transform.position - moveAmount * Time.deltaTime * SettingsSingleton.Instance.MainCamera.transform.forward;

        SettingsSingleton.Instance.AttachedEnvironment.transform.position = newPosition;

       
        var message = new OSCMessage($"{OSCCommunicationRouter.CameraMoveAddress}");
        message.AddValue(OSCValue.Float(newPosition.x));
        message.AddValue(OSCValue.Float(newPosition.y));
        message.AddValue(OSCValue.Float(newPosition.z));
        SettingsSingleton.Instance.UnityOSCTransmitter.Send(message);
    }

    public void RotateView(Vector2 rotationAmount)
    {
        var vector = new Vector3(rotationAmount.y, -rotationAmount.x, 0);
        SettingsSingleton.Instance.MainCamera.transform.eulerAngles = vector;

        var message = new OSCMessage($"{OSCCommunicationRouter.CameraRotateAddress}");
        message.AddValue(OSCValue.Float(vector.x));
        message.AddValue(OSCValue.Float(vector.y));
        message.AddValue(OSCValue.Float(vector.z));
        SettingsSingleton.Instance.UnityOSCTransmitter.Send(message);
    }
}
