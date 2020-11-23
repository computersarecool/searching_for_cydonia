using UnityEngine;
using DAW;

public class DAWManager : MonoBehaviour
{
    public Set LiveSet;

    private void Start()
    {
        this.LiveSet = new Set();
    }
}
