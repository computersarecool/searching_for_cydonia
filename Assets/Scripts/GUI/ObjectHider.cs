using UnityEngine;

public class ObjectHider : MonoBehaviour
{
    public GameObject ObjectToShow;
    public GameObject[] ObjectsToHide;

    public void ShowAndHide()
    {
        foreach (var obj in this.ObjectsToHide)
        {
            obj.SetActive(false);
        }

        this.ObjectToShow.SetActive(true);
    }
}
