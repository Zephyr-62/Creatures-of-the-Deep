using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Lever : FloatPCS
{
    [Header("Values")]
    [SerializeField] private float value;
    [SerializeField] private float min, max;
    [Header("Moving parts")]
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float step;
    [SerializeField] private float minAngle, maxAngle;

    private Vector3 point;
    private Vector3 dir;
    private float targetAngle;
    private float clampedAngle;

    public override void HandleInput()
    {
        var plane = new Plane(transform.right, transform.position);
        var ray = FirstPersonCamera.GetRay();
        if (plane.Raycast(ray, out var e))
        {
            point = ray.GetPoint(e);
            dir = point - rotatePoint.position;

            targetAngle = Vector3.SignedAngle(Vector3.up, transform.InverseTransformDirection(dir), Vector3.right);

            clampedAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

            rotatePoint.localRotation = Quaternion.AngleAxis(clampedAngle, Vector3.right);

            value = Mathf.Lerp(min, max, Mathf.InverseLerp(minAngle, maxAngle, clampedAngle));

            onValueChanged.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(point, 0.1f);
        Gizmos.DrawRay(rotatePoint.position, dir);
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, $"[{targetAngle} : {clampedAngle} : {value}]");
    }

    public override float GetValue()
    {
        return value;
    }
}
