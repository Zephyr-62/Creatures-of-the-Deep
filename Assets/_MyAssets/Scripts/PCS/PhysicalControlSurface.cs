using AdvancedEditorTools.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public abstract class PhysicalControlSurface : Measureable
{
    [Header("Core PCS events")]
    [SerializeField] public UnityEvent onValueChanged;
    [SerializeField] public UnityEvent onGrabbed;
    [SerializeField] public UnityEvent onReleased;
    [SerializeField] public UnityEvent onBlocked;
    [SerializeField] public UnityEvent onUnblocked;

    private FirstPersonCamera firstPersonCamera;
    public bool grabbed => firstPersonCamera != null;
    public bool isBlocked => blocked;

    protected FirstPersonCamera FirstPersonCamera => firstPersonCamera;

    [SerializeField] [ReadOnly] protected bool blocked = false;
    protected Vector3 grabPoint => transform.TransformPoint(_grabPoint);
    private Vector3 _grabPoint;

    internal virtual void Grab(FirstPersonCamera firstPersonCamera, Vector3 grabPoint, bool fireEvent = true)
    {
        this.firstPersonCamera = firstPersonCamera;
        this._grabPoint = grabPoint;
        if(fireEvent) onGrabbed.Invoke();
    }

    internal virtual void Release(bool fireEvent = true)
    {
        this.firstPersonCamera = null;
        if (fireEvent) onReleased.Invoke();
    }

    internal Vector3 UpdateSurface(Vector3 input)
    {
        HandleInput();
        return input;
    }

    public abstract void HandleInput();
    public abstract float GetFloatValue();
    public abstract bool GetBoolValue();
    public abstract int GetIntValue();
    public abstract void SetFloatValue(float value);
    public abstract void SetBoolValue(bool value);
    public abstract void SetIntValue(int value);
    public abstract float Get01FloatValue();
    public abstract void Set01FloatValue(float value);

    public virtual void Block()
    {
        blocked = true;
        onBlocked.Invoke();
    }

    public virtual void Unblock()
    {
        blocked = false;
        onUnblocked.Invoke();
    }

    public override float Measure()
    {
        return Get01FloatValue();
    }

    public override Vector2 GetRange()
    {
        return new Vector2(0, 1f);
    }
}
