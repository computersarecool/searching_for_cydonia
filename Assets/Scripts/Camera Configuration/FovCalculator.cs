using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FovCalculator : MonoBehaviour {
    private Camera _cam;
    private float _horizontalFov;
    private float _loggedFov;

    public void OnDrawGizmos()
    {
        _cam = gameObject.GetComponent<Camera>();
        if (_cam == null) return;
        
        _horizontalFov = GetHorizontalAngle(_cam);
        if (_horizontalFov == _loggedFov) return;
        
        _loggedFov = _horizontalFov;
        LogFov(_horizontalFov);
    }

    public void OnEnable()
    {
        if (_loggedFov != 0)
        {
            LogFov(_loggedFov);
        }
    }
    
    private static float GetHorizontalAngle(Camera cam)
    {
        var vFovRad = cam.fieldOfView * Mathf.Deg2Rad;
        var cameraHeightAt1 = Mathf.Tan(vFovRad * .5f);
        return Mathf.Atan(cameraHeightAt1 * cam.aspect) * 2f * Mathf.Rad2Deg;
    }
    
    private void LogFov(float value)
    {
        Debug.Log(name + "FOV: " + value);
    }
}