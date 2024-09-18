using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Pulley : PhysicalControlSurface
{
    private const float SMOOTHING_FACTOR = 0.1f;

    [Header("Values")]
    [FormerlySerializedAs("value"), SerializeField] private float _value;
    [SerializeField] private float max;
    [Header("Moving parts")]
    [SerializeField] private Transform handle;
    [SerializeField] private float maxLength;
    [Header("Extra events")]
    [SerializeField] private UnityEvent<float> onReleasedValue;
    [SerializeField] private UnityEvent onValueChangedToMax;
    [SerializeField] private UnityEvent onReset;

    private Vector3 point;
    private float targetLength;
    private float clampedLength;
    [SerializeField] private float velocity;
    private float last;

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
                    onValueChangedToMax.Invoke();
                }
            }
        }
    }

    internal override void Release()
    {
        base.Release();

        if(value != 0)
        {
            handle.DOLocalMove(Vector3.zero, 0.05f).SetEase(Ease.InSine).onComplete += () => onReset.Invoke();
            DOTween.To(() => value, (x) => value = x, 0, 0.05f);
            onReleasedValue.Invoke(value);
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

            AdjustToLength(targetLength);

            value = Mathf.Lerp(0, max, Mathf.InverseLerp(0, maxLength, clampedLength));
        }
    }

    private void AdjustToLength(float length)
    {
        clampedLength = Mathf.Clamp(length, 0, maxLength);
        
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
        Gizmos.DrawSphere(point, SMOOTHING_FACTOR);
    }

    private void OnValidate()
    {
        AdjustToValue(value);
    }

    private void Update()
    {
        velocity = SMOOTHING_FACTOR * (value - last) / Time.deltaTime + (1 - SMOOTHING_FACTOR) * velocity;
        if (velocity < 0.01f) velocity = 0;
        last = value;
    }
}
