using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Lever : PhysicalControlSurface
{
    [Header("Values")]
    [FormerlySerializedAs("value"), SerializeField] private float _value;
    [SerializeField] private float min, max;
    [Header("Moving parts")]
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float minAngle, maxAngle;
    [Header("Extra events")]
    [SerializeField] public UnityEvent onValueChangedToMax;
    [SerializeField] public UnityEvent onValueChangedToMin;

    private Vector3 point;
    private Vector3 dir;
    private float targetAngle;
    private float clampedAngle;

    public float value
    {
        get
        {
            return _value;
        }
        private set
        {
            var old = _value;
            _value = Mathf.Clamp(value, min, max);
            if (old != _value)
            {
                if (_value == max)
                {
                    onValueChangedToMax.Invoke();
                }
                if (_value == min)
                {
                    onValueChangedToMin.Invoke();
                }
                onValueChanged.Invoke();
            }
        }
    }

    public override void HandleInput()
    {
        var plane = new Plane(transform.right, transform.position);
        var ray = FirstPersonCamera.GetRay();
        
        if (plane.Raycast(ray, out var e))
        {
            point = ray.GetPoint(e);
            dir = point - rotatePoint.position;

            AdjustToAngle(Vector3.SignedAngle(Vector3.up, transform.InverseTransformDirection(dir), Vector3.right));

            value = Mathf.Lerp(min, max, Mathf.InverseLerp(minAngle, maxAngle, clampedAngle));
        }
    }

    private void AdjustToAngle(float angle)
    {
        targetAngle = angle;
        clampedAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);
        rotatePoint.localRotation = Quaternion.AngleAxis(clampedAngle, Vector3.right);
    }

    private void AdjustToValue(float value)
    {
        this.value = value;
        AdjustToAngle(Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(min, max, this.value)));
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

    public override void SetFloatValue(float value)
    {
        AdjustToValue(value);
    }

    public override void SetBoolValue(bool value)
    {
        AdjustToValue(value ? max : min);
    }

    public override void SetIntValue(int value)
    {
        AdjustToValue(value);
    }

    private void OnValidate()
    {
        AdjustToValue(value);
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
