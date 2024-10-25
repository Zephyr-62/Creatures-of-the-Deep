using AdvancedEditorTools.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    [Button("Screenshot")]
    private void Take()
    {
        ScreenCapture.CaptureScreenshot("Screenshot-" + Guid.NewGuid() + ".png", 4);
    }
}
