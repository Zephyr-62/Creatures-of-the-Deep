using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class HandCrank : PhysicalControlSurface
{
    [Header("Values")]
    [FormerlySerializedAs("value"), SerializeField] private float _value;
    [SerializeField] private float min, max;
    [Header("Moving parts")]
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private Transform handle;
    [SerializeField] private float minAngle, maxAngle;
    [SerializeField] private float blockedRange;
    [SerializeField] private float range = 1f;
    [SerializeField] private float speed = 360f;
    [Header("Extra events")]
    [SerializeField] public UnityEvent onValueChangedToMax;
    [SerializeField] public UnityEvent onValueChangedToMin;

    private Vector3 point;
    private Vector3 dir;
    private float targetAngle;
    private float clampedAngle;
    private float currentMinAngle, currentMaxAngle;

    private void Awake()
    {
        currentMinAngle = minAngle;
        currentMaxAngle = maxAngle;
    }

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
        var plane = new Plane(rotatePoint.up, rotatePoint.position);
        var ray = FirstPersonCamera.GetRay();

        if (plane.Raycast(ray, out var e))
        {
            point = ray.GetPoint(e);
            dir = point - rotatePoint.position;

            AdjustToAngle(Vector3.SignedAngle(Vector3.forward, transform.InverseTransformDirection(dir), Vector3.up));

            value = Mathf.Lerp(min, max, Mathf.InverseLerp(minAngle, maxAngle, clampedAngle));
        }
    }

    private void AdjustToAngle(float angle)
    {
        var delta = Mathf.DeltaAngle(targetAngle, angle);
   
        targetAngle = targetAngle + Mathf.Clamp(delta, -speed * Time.deltaTime, speed * Time.deltaTime);

        clampedAngle = Mathf.Clamp(targetAngle, currentMinAngle, currentMaxAngle);
        rotatePoint.localRotation = Quaternion.AngleAxis(clampedAngle, Vector3.up);
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

    private void OnDrawGizmos()
    {
        if (!grabbed) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(point, 0.05f);
        Gizmos.DrawRay(rotatePoint.position, dir);
#if UNITY_EDITOR
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, transform.up, 1f);
        Handles.color = Color.red;
        Handles.DrawWireDisc(handle.position, FirstPersonCamera.transform.position - handle.position, range);
        Handles.Label(transform.position, value.ToString());
#endif
    }

    public override void Block()
    {
        base.Block();
        currentMaxAngle = Mathf.Min(clampedAngle + blockedRange, maxAngle);
        currentMinAngle = Mathf.Max(clampedAngle - blockedRange, minAngle);
    }

    public override void Unblock()
    {
        base.Unblock();
        currentMinAngle = minAngle;
        currentMaxAngle = maxAngle;
    }
}
