// Create a grid of camera offsets
// A 60 degree FOV creates an aspect ratio of 2.1333
// Camera layout is:
// 0 1 2  3
// 4 5 6  7
// 8 9 10 11

using System;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class GridCamera : MonoBehaviour
{
    public int camIndex;
    public float horizontalOffset;
    public float verticalOffset;
    
    private Camera _cam;

    // Camera grid layout
    private const int NumRows = 3;
    private const int CamsPerRow = 4;

    // Camera settings
    private const float TotalFOV = 60.0F;
    private const float TotalFovRad = Mathf.Deg2Rad * TotalFOV;
    private const float IndividualAspectRatio = 1.6F;

    private void OnValidate()
    {
        Mathf.Clamp(camIndex, 0, NumRows * CamsPerRow - 1);
    }

    private void Start()
    {
        _cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        // This could be called once on start for optimization
        _cam.projectionMatrix = GetIndividualProjectionMatrix();
    }

    private Matrix4x4 GetIndividualProjectionMatrix()
    {
        const int middleRow = NumRows / 2;
        const int middleColumn = CamsPerRow / 2;
        
        // Avoid repeated accessing of built-ins
        var nearClip = _cam.nearClipPlane;
        var farClip = _cam.farClipPlane;
        
        // Camera position in grid
        var column = camIndex % CamsPerRow;
        var row = camIndex / CamsPerRow;

        // Get near plane width
        var nearPlaneWidth = 2 * nearClip * (float)Math.Tan(TotalFovRad / 2);
        var individualNearPlaneWidth = nearPlaneWidth / CamsPerRow;
        
        // Calculate column offset from center
        var offsetColumn = middleColumn - column;
        if (column >= middleColumn)
        {
            offsetColumn = Math.Abs(offsetColumn) + 1;
        }

        // Calculate near plane values going right from center
        var left = (offsetColumn - 1) * individualNearPlaneWidth;
        var right = offsetColumn * individualNearPlaneWidth;

        // Calculate vertical plane going up from center
        var offsetRow = Math.Abs(middleRow - row);
        var nearPlaneHeight = individualNearPlaneWidth / IndividualAspectRatio;
        var top = nearPlaneHeight / 2 + offsetRow * nearPlaneHeight;
        var bottom = top - nearPlaneHeight;

        // Add horizontal offsets
        var totalHorizontalOffset = offsetColumn * horizontalOffset;
        left += totalHorizontalOffset;
        right += totalHorizontalOffset;
        
        // Add vertical offsets
        var totalVerticalOffset = offsetRow * verticalOffset;
        if (row != middleRow)
        {
            top += totalVerticalOffset;
            bottom += totalVerticalOffset;
        }

        // Negate and swap horizontal values if camera is to the left of center
        if (column < CamsPerRow / 2)
        {
            var newRight = -left;
            left = -right;
            right = newRight;
        }

        // Negate and swap vertical values if camera is below center
        if (row > middleRow)
        {
            var newBottom = -top;
            top = -bottom;
            bottom = newBottom;
        }

        return PerspectiveOffCenter(left, right, bottom, top, nearClip, farClip);
    }
    
    // Copied from Unity's website
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