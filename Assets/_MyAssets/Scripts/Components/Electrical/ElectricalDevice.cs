using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ElectricalDevice : Measureable 
{
    private static List<ElectricalDevice> all = new List<ElectricalDevice>();
    private static float _globalSurge;
    public static float globalSurge => _globalSurge;

    private float _localSurge;
    private bool _isPowered;

    public bool isPowered => _isPowered;

    public float surge => _localSurge + _globalSurge;

    protected virtual void OnEnable()
    {
        all.Add(this);
    }

    protected virtual void OnDisable()
    {
        all.Remove(this);
    }

    public static void PowerAll(bool power)
    {
        all.ForEach(d => d.Power(power));
    }

    public void Power(bool power)
    {
        _isPowered = power;
        if(_isPowered)
        {
            OnPowerGained();
        }
        else
        {
            OnPowerLost();
        }
    }

    public static void SurgeAll(float intensity)
    {
        _globalSurge = intensity;
        foreach (var device in all)
        {
            device.OnSurge();
        }
    }

    public static float TotalSurge()
    {
        var x = _globalSurge;
        foreach (var device in all)
        {
            x += device._localSurge;
        }
        return x;
    }

    public void SetSurge(float intensity)
    {
        _localSurge = intensity;
        OnSurge();
    }

    protected abstract void OnPowerGained();
    protected abstract void OnPowerLost();
    protected abstract void OnSurge();

    public override float Measure()
    {
        return surge;
    }

    public override Vector2 GetRange()
    {
        return new Vector2(0f, 1f);
    }
}
