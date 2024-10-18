using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Pulley : PhysicalControlSurface
{
    private const float SMOOTHING_FACTOR = 10f;

    [Header("Values")]
    [FormerlySerializedAs("value"), SerializeField] private float _value;
    [SerializeField] private float max;
    [Header("Moving parts")]
    [SerializeField] private Transform handle;
    [SerializeField] private float maxLength;
    [SerializeField] private float blockedLength;
    [SerializeField] private float range = 1f;
    [SerializeField] private float animationDuration = 0.05f;
    [SerializeField] private Ease animationEase = Ease.InSine;
    [Header("Extra events")]
    [SerializeField] public UnityEvent onPulledToMax;
    [SerializeField] public UnityEvent onReset;

    private Vector3 point;
    private float targetLength;
    private float clampedLength;
    private float velocity;
    private float last;

    public float Velocity => velocity;

    public float value
    {
        get
        {
            return _value;
        }
        private set
        {
            var old = _value;
            _value = Mathf.Clamp(value, 0, max);
            
            if (old != _value)
            {
                onValueChanged.Invoke();
                if(_value == max)
                {
                    onPulledToMax.Invoke();
                }
            }
        }
    }

    internal override void Release(bool fireEvent = true)
    {
        base.Release();

        if(value != 0)
        {
            handle.DOLocalMove(Vector3.zero, animationDuration).SetEase(animationEase).onComplete += () => onReset.Invoke();
            DOTween.To(() => value, (x) => value = x, 0, animationDuration).SetEase(animationEase);
        }
    }

    public override void HandleInput()
    {
        var plane = new Plane(transform.forward, transform.position);
        var ray = FirstPersonCamera.GetRay();

        if (plane.Raycast(ray, out var e))
        {
            point = ray.GetPoint(e);

            targetLength = transform.InverseTransformPoint(point).y;

            if (Vector3.Distance(handle.transform.position, point) > range)
            {
                FirstPersonCamera.ForceRelease();
                return;
            }

            AdjustToLength(targetLength);

            value = Mathf.Lerp(0, max, Mathf.InverseLerp(0, maxLength, clampedLength));
        }
    }

    private void AdjustToLength(float length)
    {
        clampedLength = Mathf.Clamp(length, 0, blocked ? blockedLength : maxLength);
        
        handle.localPosition = Vector3.up * clampedLength;
    }

    private void AdjustToValue(float value)
    {
        this.value = value;
        AdjustToLength(Mathf.Lerp(0, maxLength, Mathf.InverseLerp(0, max, this.value)));
    }

    public override bool GetBoolValue()
    {
        return value == max;
    }

    public override float GetFloatValue()
    {
        return value;
    }

    public override int GetIntValue()
    {
        return Mathf.RoundToInt(value);
    }

    public override void SetBoolValue(bool value)
    {
        this.value = value ? 1 : 0;
    }

    public override void SetFloatValue(float value)
    {
        this.value = value;
    }

    public override void SetIntValue(int value)
    {
        this.value = value;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(point, 0.1f);
    }

    private void OnValidate()
    {
        AdjustToValue(value);
    }

    private void Update()
    {
        velocity = (value - last) / Time.deltaTime;
        last = value;
    }

    private void OnDrawGizmos()
    {
        if (!grabbed) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(point, 0.05f);
#if UNITY_EDITOR
        Handles.color = Color.blue;
        Handles.DrawWireDisc(handle.position, transform.forward, range);
                Handles.Label(transform.position, value.ToString());
#endif
    }
}
