using UnityEngine;

public class AudioEqualizer : MonoBehaviour
{
    public float Scale;
    public GameObject[] Cubes;

    public void UpdateBand(int bandIndex, float value)
    {
        if (bandIndex >= this.Cubes.Length) return;
        
        var selectedCube = this.Cubes[bandIndex];
        var localScale = selectedCube.transform.localScale;
        var newScale = value * this.Scale;
        selectedCube.transform.localScale = new Vector3(localScale.x, newScale, localScale.z);
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
}
