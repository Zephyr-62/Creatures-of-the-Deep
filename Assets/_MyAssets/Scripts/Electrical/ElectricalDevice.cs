using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ElectricalDevice : Measureable 
{
    private static List<ElectricalDevice> all = new List<ElectricalDevice>();
    private static float globalSurge;
    
    private float localSurge;
    private bool hasPower;

    public bool HasPower => hasPower;

    public float surge => localSurge + globalSurge;

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
        hasPower = power;
        if(hasPower)
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
        globalSurge = intensity;
        foreach (var device in all)
        {
            device.OnSurge();
        }
    }

    public void SetSurge(float intensity)
    {
        localSurge = intensity;
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
