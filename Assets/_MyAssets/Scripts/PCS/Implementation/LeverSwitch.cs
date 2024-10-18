using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[SelectionBase]
public class LeverSwitch : PhysicalControlSurface
{
    [Header("Values")]
    [FormerlySerializedAs("value"), SerializeField] private bool _value;
    [Header("Moving parts")]
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private Transform handle;
    [SerializeField] private float minAngle, maxAngle;
    [SerializeField] private float attachMinAngle, attachMaxAngle;
    [SerializeField] private float blockedRange;
    [SerializeField] private float range = 1f;
    [SerializeField] private float speed = 360f;
    [Header("Physics")]
    [SerializeField] private float mass = 1f;
    [SerializeField] private float drag = 0.5f;
    [SerializeField] private float bounciness = 0.1f;

    private Vector3 point;
    private Vector3 dir;
    private float targetAngle;
    private float clampedAngle;
    private float currentMinAngle, currentMaxAngle;
    private float velocity;

    private void Awake()
    {
        currentMinAngle = minAngle;
        currentMaxAngle = maxAngle;
    }

    public bool value
    {
        get
        {
            return _value;
        }
        private set
        {
            var old = _value;
            _value = value;
            if (old != _value)
            {
                onValueChanged.Invoke();
            }
        }
    }

    internal override void Grab(FirstPersonCamera firstPersonCamera, Vector3 grabPoint, bool fireEvent = true)
    {
        base.Grab(firstPersonCamera, grabPoint);
        velocity = 0;
    }

    private void Update()
    {
        if (grabbed) return;

        if(clampedAngle >= attachMinAngle && clampedAngle <= attachMaxAngle)
        {
            velocity = 0;
            return;
        }

        velocity += rotatePoint.InverseTransformDirection(Vector3.ProjectOnPlane(Physics.gravity * mass, rotatePoint.up)).z;

        velocity -= velocity * velocity * drag * Time.deltaTime; //Drag

        AdjustToAngle(clampedAngle + velocity, false);

        value = clampedAngle >= attachMinAngle && clampedAngle <= attachMaxAngle;

        if(clampedAngle == maxAngle || clampedAngle == minAngle)
        {
            velocity = -velocity * bounciness;
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

            if(dir.magnitude > range)
            {
                FirstPersonCamera.ForceRelease();
                return;
            }

            AdjustToAngle(Vector3.SignedAngle(Vector3.up, transform.InverseTransformDirection(dir), Vector3.right));

            value = clampedAngle >= attachMinAngle && clampedAngle <= attachMaxAngle;
        }
    }

    private void AdjustToAngle(float angle, bool speedLimit = true)
    {
        var delta = Mathf.DeltaAngle(clampedAngle, angle);
        if (speedLimit)
        {
            velocity = Mathf.Clamp(delta, -speed * Time.deltaTime, speed * Time.deltaTime);
            targetAngle = clampedAngle + velocity;
        } else
        {
            targetAngle = angle;
        }

        clampedAngle = Mathf.Clamp(targetAngle, currentMinAngle, currentMaxAngle);
        rotatePoint.localRotation = Quaternion.AngleAxis(clampedAngle, Vector3.right);
    }

    private void AdjustToValue(bool value)
    {
        AdjustToAngle(this.value ? maxAngle : minAngle, false);
        this.value = value;
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
        currentMinAngle = minAngle;
        currentMaxAngle = maxAngle;
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
        Handles.DrawWireDisc(rotatePoint.position, transform.right, range);
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
