using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DisplayIndex : int
{
    Display1 = 0,
    Display2,
    Display3,
    Display4,
    Display5,
    Display6,
    Display7,
    Display8,
}

public class MultiDisplayGraphicRaycaster : GraphicRaycaster
{
    public DisplayIndex DisplayIndex
    {
        get
        {
            return displayIndex;
        }
        set
        {
            displayIndex = value;
        }
    }

    [SerializeField]
    private DisplayIndex displayIndex;

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (MultiDisplayUtil.GetCurrentDisplay() != (int)displayIndex)
            return;

        base.Raycast(eventData, resultAppendList);
    }
}
