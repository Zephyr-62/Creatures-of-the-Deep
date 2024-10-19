using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour
{
    [SerializeField] private Transform anchor;

    LineRenderer lineRenderer;
    Vector3[] points;

    private void Update()
    {
        SetLine();
    }

    private void SetLine()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        points ??= new Vector3[2];

        points[0] = transform.position;
        points[1] = anchor.position;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(points);
    }

    private void OnValidate()
    {
        SetLine();
    }
}
