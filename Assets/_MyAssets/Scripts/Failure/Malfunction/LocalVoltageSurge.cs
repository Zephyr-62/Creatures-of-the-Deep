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
    bool selfTriggered = false;

    public override void Enter()
    {
        base.Enter();
        count++;
        if (!selfTriggered)
        {
            target = UnityEngine.Random.Range(dial.Min, dial.Max);
        }
        selfTriggered = false;
    }

    public override void Exit()
    {
        base.Exit();
        count--;
    }

    public override bool IsFixed()
    {
        return Mathf.Abs(dial.GetFloatValue() - target) <= tolerance;
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
            if (!Enabled && Mathf.Abs(dial.GetFloatValue() - target) > tolerance)
            {
                selfTriggered = true;
                system.Failure(this);
            }
        }
        
    }
}
