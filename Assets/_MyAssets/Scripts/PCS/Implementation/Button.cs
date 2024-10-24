using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

public class Button : PhysicalControlSurface
{    
    [Header("Moving parts")]
    [SerializeField] private Transform movePoint;
    [SerializeField] private float depth;
    [SerializeField] private float range = 1f;
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private Ease animationEase = Ease.Linear;

    private Vector3 point;
    private bool _value;
    
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

    internal override void Grab(FirstPersonCamera firstPersonCamera, Vector3 grabPoint)
    {
        base.Grab(firstPersonCamera, grabPoint);
        AdjustToValue(true);
    }

    internal override void Release()
    {
        base.Release();
        AdjustToValue(false);
    }

    public override void HandleInput()
    {
        var plane = new Plane(FirstPersonCamera.transform.position - movePoint.position, movePoint.position);
        var ray = FirstPersonCamera.GetRay();

        if (plane.Raycast(ray, out var e))
        {
            point = ray.GetPoint(e);

            if(Vector3.Distance(point, movePoint.position) > range)
            {
                FirstPersonCamera.ForceRelease();
                return;
            }
        }
    }

    private void AdjustToValue(bool value, bool skipAnimation = false)
    {
        if(blocked && value) return;

        this.value = value;

        var delta = Vector3.down * (value ? depth : 0);

        if (skipAnimation)
        {
            movePoint.localPosition = delta;
        } else
        {
            movePoint.DOKill();
            movePoint.DOLocalMove(delta, animationDuration).SetEase(animationEase);
            onValueChanged.Invoke();
        }
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
        AdjustToValue(value, true);
    }

    private void Awake()
    {
        AdjustToValue(value, true);
    }

    private void OnDrawGizmos()
    {
        if (!grabbed) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(point, 0.05f);
        Gizmos.DrawRay(movePoint.position, point - movePoint.position);
#if UNITY_EDITOR
        Handles.color = Color.red;
        Handles.DrawWireDisc(movePoint.position, FirstPersonCamera.transform.position - movePoint.position, range);
        Handles.Label(transform.position, value.ToString());
#endif
    }
}
