using extOSC;
using UnityEngine;
using UnityEngine.EventSystems;

public class ViewMover: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool forwardMovement;
    
    private bool _pressed;
    private const int MoveAmount = 5;

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
    }

    public void Update()
    {
        if (_pressed)
        {
            Move(forwardMovement);
        }
    }

    private static void Move(bool goForward)
    {
        Vector3 newPosition;
        if (goForward)
        {
            newPosition = SettingsSingleton.Instance.fixedEnvironment.transform.position + MoveAmount * Time.deltaTime * SettingsSingleton.Instance.mainCamera.transform.forward;
        }
        else
        {
            newPosition = SettingsSingleton.Instance.fixedEnvironment.transform.position - MoveAmount * Time.deltaTime * SettingsSingleton.Instance.mainCamera.transform.forward;
        }
        
        SettingsSingleton.Instance.fixedEnvironment.transform.position = newPosition;

       
        var message = new OSCMessage($"{OSCCommunicationRouter.CameraMoveAddress}");
        message.AddValue(OSCValue.Float(newPosition.x));
        message.AddValue(OSCValue.Float(newPosition.y));
        message.AddValue(OSCValue.Float(newPosition.z));
        SettingsSingleton.Instance.unityOSCTransmitter.Send(message);
    }

}

