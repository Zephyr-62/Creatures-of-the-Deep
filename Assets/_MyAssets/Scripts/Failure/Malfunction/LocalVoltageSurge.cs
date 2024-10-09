using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalVoltageSurge : Malfunction
{
    static int count = 0;
    public Lever dial;
    public ElectricalDevice device;
    public float tolerance = 0.1f;
    float target;

    public override void Enter()
    {
        count++;
        target = UnityEngine.Random.Range(dial.Min, dial.Max);
    }

    public override void Exit()
    {
        device.SetSurge(0);
        count--;
    }

    public override bool IsFixed()
    {
        return Mathf.Abs(dial.GetFloatValue() - target) < tolerance;
    }

    public override void Update()
    {
        device.SetSurge(Mathf.Abs(dial.GetFloatValue() - target));
    }
}
