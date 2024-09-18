using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ClickySwitch : PhysicalControlSurface
{
    [Header("Values")]
    [FormerlySerializedAs("value"), SerializeField] private bool _value;
    [Header("Moving parts")]
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float minAngle, maxAngle;
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private Ease animationEase = Ease.Linear;

    private Vector3 point;
    private Vector3 dir;
    private float targetAngle;
    private float clampedAngle;
    private bool old;

    public bool value
    {
        get
        {
            return _value;
        }
        private set
        {
            if(_value != value)
            {
                old = _value;
                _value = value;
                if (old != _value)
                {
                    onValueChanged.Invoke();
                }
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

            this.value = clampedAngle >= 0;

            AdjustToAngle(Vector3.SignedAngle(Vector3.up, transform.InverseTransformDirection(dir), Vector3.right));
        }
    }

    private void AdjustToAngle(float angle, bool skipAnimation = false)
    {
        targetAngle = angle;
        clampedAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

        if(clampedAngle > 0 && value != old)
        {
            Rotate(maxAngle, skipAnimation);
        }
        else if (clampedAngle < 0 && value != old)
        {
            Rotate(minAngle, skipAnimation);
        }
    }

    private void Rotate(float angle, bool skipAnimation = false)
    {
        if (skipAnimation)
        {
            rotatePoint.localRotation = Quaternion.AngleAxis(angle, Vector3.right);
        }
        else
        {
            rotatePoint.DOKill();
            rotatePoint.DOLocalRotate(new Vector3(angle, 0, 0), animationDuration).SetEase(animationEase);
        }
    }

    private void AdjustToValue(bool value, bool skipAnimation = false)
    {
        Debug.Log(value);
        this.value = value;
        AdjustToAngle(this.value ? maxAngle : minAngle, skipAnimation);
    }

    public override float GetFloatValue()
    {
        return value ? 1f : 0f;
    }

    public override bool GetBoolValue()
    {
        return value;
    }

    public override int GetIntValue()
    {
        return value ? 1 : 0;
    }

    public override void SetFloatValue(float value)
    {
        AdjustToValue(value != 0);
    }

    public override void SetBoolValue(bool value)
    {
        AdjustToValue(value);
    }

    public override void SetIntValue(int value)
    {
        AdjustToValue(value != 0);
    }

    private void OnValidate()
    {
        old = !value;
        AdjustToValue(value, true);
    }

    private void Awake()
    {
        old = !value;
        AdjustToValue(value, true);
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
