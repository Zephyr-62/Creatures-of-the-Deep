using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ElectricalDevice : MonoBehaviour
{
    private static List<ElectricalDevice> all = new List<ElectricalDevice>();
    private static float globalSurge;
    
    private float localSurge;
    protected float surge => localSurge + globalSurge;

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
        if(power)
        {
            all.ForEach(d => d.OnPowerOn());
        } else
        {
            all.ForEach(d => d.OnPowerOff());
        }
    }

    public void Power(bool power)
    {
        if(power)
        {
            OnPowerOn();
        }
        else
        {
            OnPowerOff();
        }
    }

    public static void SurgeAll(float intensity)
    {
        globalSurge = intensity;
    }

    public void SetSurge(float intensity)
    {
        localSurge = intensity;
        OnSurge(localSurge);
    }

    protected abstract void OnPowerOn();
    protected abstract void OnPowerOff();
    protected abstract void OnSurge(float intensity);
}
