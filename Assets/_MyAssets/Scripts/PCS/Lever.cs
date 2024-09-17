using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Lever : PhysicalControlSurface
{
    [Header("Values")]
    [SerializeField] private float value;
    [SerializeField] private float min, max;
    [Header("Moving parts")]
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float step;
    [SerializeField] private float minAngle, maxAngle;
    [Header("Extra events")]
    [SerializeField] private UnityEvent onValueChangedToMax;
    [SerializeField] private UnityEvent onValueChangedToMin;

    private Vector3 point;
    private Vector3 dir;
    private float targetAngle;
    private float clampedAngle;

    public override bool HandleInput()
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

            var old = value;
            value = Mathf.Lerp(min, max, Mathf.InverseLerp(minAngle, maxAngle, clampedAngle));

            if(old != value)
            {
                if(value == max)
                {
                    onValueChangedToMax.Invoke();
                }
                if(value == min)
                {
                    onValueChangedToMin.Invoke();
                }
                return true;
            }
        }

        return false;
    }

    public override float GetFloatValue()
    {
        return value;
    }

    public override bool GetBoolValue()
    {
        return Mathf.RoundToInt(Mathf.InverseLerp(min, max, value)) == 1;
    }

    public override int GetIntValue()
    {
        return Mathf.RoundToInt(value);
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
}
