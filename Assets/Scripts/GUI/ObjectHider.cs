using UnityEngine;

public class ObjectHider : MonoBehaviour
{
    public GameObject objectToShow;
    public GameObject[] objectsToHide;

    public void ShowAndHide()
    {
        foreach (var obj in objectsToHide)
        {
            obj.SetActive(false);
        }

        objectToShow.SetActive(true);
    }
}
