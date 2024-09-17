using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public abstract class PhysicalControlSurface : MonoBehaviour
{
    [SerializeField] protected UnityEvent onValueChanged;
    [SerializeField] private UnityEvent onGrabbed;
    [SerializeField] private UnityEvent onReleased;

    private FirstPersonCamera firstPersonCamera;

    protected FirstPersonCamera FirstPersonCamera => firstPersonCamera;

    internal void Grab(FirstPersonCamera firstPersonCamera)
    {
        this.firstPersonCamera = firstPersonCamera;
        onGrabbed.Invoke();
    }

    internal void Release()
    {
        this.firstPersonCamera = null;
        onReleased.Invoke();
    }

    internal Vector3 UpdateSurface(Vector3 input)
    {
        if (HandleInput())
        {
            onValueChanged.Invoke();
        }
        return input;
    }

    public abstract bool HandleInput();
    public abstract float GetFloatValue();
    public abstract bool GetBoolValue();
    public abstract int GetIntValue();
}
