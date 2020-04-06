using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class LayoutCellSizer : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        SizeCells();
    }

    private void OnEnable()
    {
        SizeCells();        
    }

    private void SizeCells()
    {
        RectTransform parentCanvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Vector2 canvasWidthHeight = new Vector2(parentCanvas.rect.width, parentCanvas.rect.height);
        GridLayoutGroup layoutGroup = GetComponentInParent<Canvas>().GetComponent<GridLayoutGroup>();
        layoutGroup.cellSize = new Vector2(canvasWidthHeight.x / 4, canvasWidthHeight.y / 3);
    }
    
}

