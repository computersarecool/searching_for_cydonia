using UnityEngine;
using UnityEngine.UI;

public class GUIFiller : MonoBehaviour
{
    public Image ObjectToFill;
    public float MaxValue;

    public void FillObject(float value)
    {
        this.ObjectToFill.fillAmount = value / this.MaxValue;
    }
}
