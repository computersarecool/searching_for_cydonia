using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FOVCalculator : MonoBehaviour {
    private Camera cam;
    private float horizontalFov;
    private float loggedFov;

    public void OnDrawGizmos()
    {
        this.cam = GetComponent<Camera>();
        if (this.cam == null) return;
        
        this.horizontalFov = GetHorizontalAngle(this.cam);
        if (this.horizontalFov == this.loggedFov) return;
        
        this.loggedFov = this.horizontalFov;
        LogFov(this.horizontalFov);
    }

    public void OnEnable()
    {
        if (this.loggedFov != 0)
        {
            LogFov(this.loggedFov);
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
        Debug.Log(this.name + "FOV: " + value);
    }
}
