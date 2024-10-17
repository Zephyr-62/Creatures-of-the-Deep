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
public class Lever : PhysicalControlSurface
{
    [Header("Values")]
    [FormerlySerializedAs("value"), SerializeField] private float _value;
    [SerializeField] private float min, max;
    [Header("Moving parts")]
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float minAngle, maxAngle;
    [SerializeField] private float blockedRange;
    [SerializeField] private float range = 1f;
    [SerializeField] private float speed = 360f;
    [Header("Extra events")]
    [SerializeField] public UnityEvent onValueChangedToMax;
    [SerializeField] public UnityEvent onValueChangedToMin;
    [Header("Sounds")]
    [SerializeField] private FMODUnity.EventReference rotate;
    [SerializeField] private string parameter = "lever_speed";

    private Vector3 point;
    private Vector3 dir;
    private float targetAngle;
    private float clampedAngle;
    private float currentMinAngle, currentMaxAngle;
    private FMOD.Studio.EventInstance instance;

    public float Min => min;
    public float Max => max;

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

    private void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(rotate);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform);
    }

    internal override void Grab(FirstPersonCamera firstPersonCamera, Vector3 grabPoint)
    {
        base.Grab(firstPersonCamera, grabPoint);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform);
        instance.start();
    }

    internal override void Release()
    {
        base.Release();
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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

            value = Mathf.Lerp(min, max, Mathf.InverseLerp(minAngle, maxAngle, clampedAngle));
        }
    }

    private void AdjustToAngle(float angle)
    {
        angle = Mathf.Clamp(angle, currentMinAngle, currentMaxAngle);

        var delta = Mathf.DeltaAngle(targetAngle, angle);

        var v = Mathf.Clamp(delta, -speed * Time.deltaTime, speed * Time.deltaTime);

        instance.setParameterByName(parameter, Mathf.Abs((v / Time.deltaTime) / speed));

        targetAngle = targetAngle + v;

        clampedAngle = Mathf.Clamp(targetAngle, currentMinAngle, currentMaxAngle);

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
