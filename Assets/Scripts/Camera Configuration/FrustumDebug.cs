// Set an off-center projection, where perspective's vanishing
// point is not necessarily in the center of the screen.

// Camera order is:
// 0 1 2  3
// 4 5 6  7  
// 8 9 10 11

using System;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class FrustumDebug : MonoBehaviour
{
    private Camera _cam;
    
    // Camera layout configuration
    private const int CamsPerRow = 4;
    private const int CamsToLeftOfCenter = 2;
    private const int CamLeftOffset = CamsPerRow - CamsToLeftOfCenter;
    private const int NumRows = 3;
    
    // Camera settings
    private const float AspectRatio = 1.6F;
    private const float Fov = 60;
    private const float FovInRadians = Mathf.Deg2Rad * Fov;
    private readonly float _tanHalfFov = (float)Math.Tan(FovInRadians / 2.0);
    
    public void Start()
    {
        _cam= GetComponent<Camera>();
    }

    public void LateUpdate()
    {
        // Avoid repeated accessing of built-ins
        var nearClip = _cam.nearClipPlane;
        var farClip = _cam.farClipPlane;


        // Multiply by two to get entire horizontal clip plane amount
        var horizontalClipPlaneAmount = nearClip * _tanHalfFov * 2;
        var farthestLeft = horizontalClipPlaneAmount * -CamLeftOffset;
        var verticalClipPlaneAmount = horizontalClipPlaneAmount / AspectRatio;
        
        // Projection matrix values
        var left = farthestLeft;
        var right = left + horizontalClipPlaneAmount * CamsPerRow;
        var bottom = 0;
        var top = bottom + verticalClipPlaneAmount * NumRows;
        
        var projectionMatrix = PerspectiveOffCenter(left, right, bottom, top, nearClip, farClip);
        _cam.projectionMatrix = projectionMatrix;
    }

    private static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        var x = 2.0F * near / (right - left);
        var y = 2.0F * near / (top - bottom);
        var a = (right + left) / (right - left);
        var b = (top + bottom) / (top - bottom);
        var c = -(far + near) / (far - near);
        var d = -(2.0F * far * near) / (far - near);
        const float e = -1.0F;
        var projectionMatrix = new Matrix4x4
        {
            [0, 0] = x,
            [0, 1] = 0,
            [0, 2] = a,
            [0, 3] = 0,
            [1, 0] = 0,
            [1, 1] = y,
            [1, 2] = b,
            [1, 3] = 0,
            [2, 0] = 0,
            [2, 1] = 0,
            [2, 2] = c,
            [2, 3] = d,
            [3, 0] = 0,
            [3, 1] = 0,
            [3, 2] = e,
            [3, 3] = 0
        };
        return projectionMatrix;
    }
}