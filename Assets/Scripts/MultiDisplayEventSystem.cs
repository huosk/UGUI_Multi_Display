using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MultiDisplayEventSystem : EventSystem
{
    [SerializeField]
    DisplayIndex displayIndex;

    protected override void Update()
    {
        if (MultiDisplayUtil.GetCurrentDisplay() == (int)displayIndex)
        {
            if (current != this) current = this;
        }
        else
            return;

        base.Update();
    }
}
