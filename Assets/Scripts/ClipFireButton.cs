using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClipFireButton : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector]
    public string CanonicalPath;

    public string Name
    {
        set
        {
            var textElement = GetComponentInChildren<Text>();
            textElement.text = value;
        }
    }

    public int Color
    {
        set
        {
            var colorR = (value >> 16) & 255;
            var colorG = (value >> 8) & 255;
            var colorB = value & 255;
            var colorRf = colorR / 255.0f;
            var colorGf = colorG / 255.0f;
            var colorBf = colorB / 255.0f;
            GetComponent<Image>().color = new Color(colorRf, colorGf, colorBf);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AbletonController.CallFunction(this.CanonicalPath, "fire");
    }
}
