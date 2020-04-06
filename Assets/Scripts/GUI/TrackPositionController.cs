using UnityEngine;
using UnityEngine.UI;

public class TrackPositionController : MonoBehaviour
{
    public Image sprite;
    private const float UnitsPerCycle = 7.0F;
    
    public void UpdatePos(int value)
    {
        sprite.fillAmount = value / UnitsPerCycle;
    }
}
