using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool MoveForward;
    private bool pressed;
    //private AttachedObjectController attachedObjectController;

    private void Start()
    {
        //this.attachedObjectController = FindObjectOfType<AttachedObjectController>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.pressed = false;
    }

    private void Update()
    {
        if (this.pressed)
        {
            //this.attachedObjectController.Move(this.MoveForward);
        }
    }
}
