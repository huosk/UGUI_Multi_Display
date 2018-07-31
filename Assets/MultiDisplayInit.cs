using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDisplayInit : MonoBehaviour
{
    public int displayCount;

    // Use this for initialization
    void Start()
    {
        int count = Mathf.Min(displayCount, Display.displays.Length);
        int mainWidth = Display.main.systemWidth;
        int mainHeight = Display.main.systemHeight;

        for (int i = 0; i < count; i++)
        {
            if (Display.displays[i].active)
                continue;
            Display.displays[i].Activate(mainWidth, mainHeight, 60);
        }
    }
}
