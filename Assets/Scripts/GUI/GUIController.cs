using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public Image objectToFill;
    public float _maxValue;

    public void FillObject(float value)
    {
        objectToFill.fillAmount = value / _maxValue;
    }
}
