using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioEqualizer : MonoBehaviour
{
    public float Scale;
    public GameObject[] Cubes;

    private const float eqExponent = 0.5f;
    private const int numCubes = 9;
    private const int numSamples = 10;
    readonly List<List<float>> samplesArrays = new List<List<float>>();

    private void Start()
    {
        for (var i = 0; i < numCubes; i++)
        {
            var sublist = new List<float>();
            for (var j = 0; j < numSamples; j++)
            {
                sublist.Add(j);
            }
            this.samplesArrays.Add(sublist);
        }
    }

    public void UpdateBand(int bandIndex, float value)
    {
        if (bandIndex >= this.Cubes.Length) return;

        // The cross~ object from M4L outputs positive / negative amplitude values.
        // Take the abs of these which range from very small values [.001] to larger [0.4]
        // Those values can be smoothed and scaled
        var selectedCube = this.Cubes[bandIndex];
        var localScale = selectedCube.transform.localScale;
        var localPosition = selectedCube.transform.localPosition;

        var smoothValue = this.GetEQAverage(bandIndex, value);
        var newScale = smoothValue * this.Scale;
        selectedCube.transform.localScale = new Vector3(localScale.x, newScale, localScale.z);
        selectedCube.transform.localPosition = new Vector3(localPosition.x, newScale / 2.0f, localPosition.z);
    }

    public void UpdateHue(float newHue)
    {
        foreach (var cube in this.Cubes)
        {
            var mat = cube.GetComponent<Renderer>().material;
            var originalColor = mat.color;
            var originalAlpha = originalColor.a;
            Color.RGBToHSV(originalColor, out _, out var startSat, out var startVal);
            var rgbColor = Color.HSVToRGB(newHue, startSat, startVal);
            mat.color = new Color(rgbColor.r, rgbColor.g, rgbColor.b, originalAlpha);
        }
    }

    private float GetEQAverage(int bandIndex, float val)
    {
        var sampleAmplitude = (float)Math.Pow(Math.Abs(val), eqExponent);
        this.samplesArrays[bandIndex].Add(sampleAmplitude);
        this.samplesArrays[bandIndex].RemoveAt(0);
        return this.samplesArrays[bandIndex].Average();
    }
}
