#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class PatrolTarget : MonoBehaviour
{
    public float GizmoSize = 1;
    public Color Color = Color.magenta;
    public Color BeamColor = Color.magenta;
    public float BeamHeight = 50;
    public float BeamThickness = 0.5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color;
        Gizmos.DrawSphere(transform.position, GizmoSize);
        var p2 = transform.position;
        var p1 = transform.position + Vector3.up * BeamHeight;

        Handles.DrawBezier(p1, p2, p1, p2, BeamColor, null, BeamThickness);
    }
}

#endif