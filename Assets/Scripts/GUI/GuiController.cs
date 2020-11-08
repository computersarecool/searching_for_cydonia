using UnityEngine;
using UnityEngine.UI;

public class GuiController : MonoBehaviour
{
    public Image objectToFill;
    public float maxValue;

    public void FillObject(float value)
    {
        objectToFill.fillAmount = value / maxValue;
    }
}
