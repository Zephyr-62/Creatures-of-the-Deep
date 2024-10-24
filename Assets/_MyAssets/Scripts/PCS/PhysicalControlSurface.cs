using AdvancedEditorTools.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public abstract class PhysicalControlSurface : MonoBehaviour
{
    [Header("Core PCS events")]
    [SerializeField] public UnityEvent onValueChanged;
    [SerializeField] public UnityEvent onGrabbed;
    [SerializeField] public UnityEvent onReleased;
    [SerializeField] public UnityEvent onBlocked;
    [SerializeField] public UnityEvent onUnblocked;

    private FirstPersonCamera firstPersonCamera;
    public bool grabbed => firstPersonCamera != null;
    
    protected FirstPersonCamera FirstPersonCamera => firstPersonCamera;

    protected bool blocked;
    protected Vector3 grabPoint;

    internal virtual void Grab(FirstPersonCamera firstPersonCamera, Vector3 grabPoint)
    {
        this.firstPersonCamera = firstPersonCamera;
        this.grabPoint = grabPoint;
        onGrabbed.Invoke();
    }

    internal virtual void Release()
    {
        this.firstPersonCamera = null;
        onReleased.Invoke();
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
}
