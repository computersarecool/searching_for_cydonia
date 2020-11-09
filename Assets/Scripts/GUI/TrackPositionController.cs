using UnityEngine;
using UnityEngine.UI;

public class TrackPositionController : MonoBehaviour
{
    public Image Sprite;
    private const float unitsPerCycle = 7.0F;
    
    public void UpdatePos(int value)
    {
        this.Sprite.fillAmount = value / unitsPerCycle;
    }
}
