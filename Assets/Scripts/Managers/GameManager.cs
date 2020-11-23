using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int camRotationControlIndex = 2;
    private const int camMovementControlIndex = 6;
    private int cameraIndex;
    private int CameraIndex
    {
        set
        {
            this.cameraIndex = value;
            if (Camera.main is null) return;

            Camera.main.GetComponent<GridCamera>().CamIndex = value;
        }
    }
    [HideInInspector] public float Clock;

    public void UpdateCameraRotation(Vector3 vector)
    {
        if (this.cameraIndex == camRotationControlIndex || Camera.main is null) return;

        Camera.main.transform.eulerAngles = vector;
    }

    public void SelectCamera(int index)
    {
        this.CameraIndex = index;
    }

    public void MoveMainCameraAndEnvironment(Vector3 vector)
    {
        if (this.cameraIndex != camMovementControlIndex)
        {
            //FindObjectOfType<AttachedObjectController>().transform.position = vector;
        }
    }

    private const int moveAmount = 45;

    public void Move(bool forward)
    {
        // var vector = forward
        //     ? SettingsSingleton.Instance.AttachedEnvironment.transform.position + moveAmount * Time.deltaTime * SettingsSingleton.Instance.MainCamera.transform.forward
        //     : SettingsSingleton.Instance.AttachedEnvironment.transform.position - moveAmount * Time.deltaTime * SettingsSingleton.Instance.MainCamera.transform.forward;
        //
        // SendOSCUpdate($"{OSCCommunicationRouter.CameraPositionAddress}", vector);
    }

    public void RotateView(Vector2 rotationAmount)
    {
        var vector = new Vector3(-rotationAmount.y, rotationAmount.x, 0);
        SendOSCUpdate($"{OSCListener.CameraRotationAddress}", vector);
    }

    private void SendOSCUpdate(string toAddress, Vector3 vector)
    {
        // var message = new OSCMessage(toAddress);
        // message.AddValue(OSCValue.Float(vector.x));
        // message.AddValue(OSCValue.Float(vector.y));
        // message.AddValue(OSCValue.Float(vector.z));
        //SettingsSingleton.Instance.UnityOSCTransmitter.Send(message);
    }
}
