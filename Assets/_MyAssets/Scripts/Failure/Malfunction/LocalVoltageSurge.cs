using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalVoltageSurge : Malfunction
{
    static int count = 0;
    public Lever dial;
    public List<ElectricalDevice> devices;
    public float tolerance = 0.2f;
    float target;

    public override void Enter()
    {
        base.Enter();
        count++;
        target = UnityEngine.Random.Range(dial.Min, dial.Max);
    }

    public override void Exit()
    {
        base.Exit();
        count--;
    }

    public override bool IsFixed()
    {
        return Mathf.Abs(dial.GetFloatValue() - target) < tolerance;
    }

    public override void Update()
    {
        base.Update();
        if (dial)
        {
            foreach (var device in devices)
            {
                device.SetSurge(Mathf.Clamp01(Mathf.Abs(dial.GetFloatValue() - target) - tolerance));
            }
        }
    }
}
