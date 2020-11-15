using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool MoveForward;
    private bool pressed;
    private ViewController viewController;

    private void Start()
    {
        this.viewController = FindObjectOfType<ViewController>();
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
            this.viewController.Move(this.MoveForward);
        }
    }
}
