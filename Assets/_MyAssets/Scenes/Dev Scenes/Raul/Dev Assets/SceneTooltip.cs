using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneTooltip : MonoBehaviour
{
    [TextArea]
    public string Text;
    public Color color = Color.white;
    [Range(1f, 20f)]
    public int FontSize = 5;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR

        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        style.fontSize = FontSize;

        Vector3 labelPosition = this.transform.position;
        Vector3 screenPosition = HandleUtility.WorldToGUIPoint(labelPosition);
        Matrix4x4 originalMatrix = GUI.matrix;

        Camera camera = Camera.current;
        if (camera == null) return;
        float distanceToCamera = Vector3.Distance(camera.transform.position, labelPosition);
        float scaledFontSize = distanceToCamera * FontSize;

        Handles.BeginGUI();
        GUIUtility.RotateAroundPivot(this.transform.localEulerAngles.z, screenPosition);
        Handles.Label(labelPosition, Text, style);
        Handles.EndGUI();

        GUI.matrix = originalMatrix;
#endif
    }
}
