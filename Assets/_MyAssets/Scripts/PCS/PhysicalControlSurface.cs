using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public abstract class PhysicalControlSurface : MonoBehaviour
{
    [Header("Core PCS events")]
    [SerializeField] protected UnityEvent onValueChanged;
    [SerializeField] private UnityEvent onGrabbed;
    [SerializeField] private UnityEvent onReleased;

    public bool grabbed => firstPersonCamera != null;

    private FirstPersonCamera firstPersonCamera;
    protected FirstPersonCamera FirstPersonCamera => firstPersonCamera;

    internal virtual void Grab(FirstPersonCamera firstPersonCamera)
    {
        this.firstPersonCamera = firstPersonCamera;
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

}
