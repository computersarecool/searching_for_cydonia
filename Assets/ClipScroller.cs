using UnityEngine;
using UnityEngine.EventSystems;

public class ClipScroller : MonoBehaviour, IPointerDownHandler
{
    public bool increase;

    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponentInParent<TrackContainer>().UpdateOffset(increase);
    }
}
