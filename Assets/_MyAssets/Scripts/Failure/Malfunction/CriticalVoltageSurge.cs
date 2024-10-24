using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CriticalVoltageSurge : Malfunction
{
    public override void Enter(MalfunctionTrigger trigger = null)
    {
        base.Enter();

        ElectricalDevice.SurgeAll(1f);
    }

    public override void Exit()
    {
        base.Exit();

        ElectricalDevice.SurgeAll(0f);
    }

    public override bool IsFixed()
    {
        return false;
    }

    public override void Update()
    {
        base.Update();
        if(!Enabled && system.breaker.IsOverloaded())
        {
            system.Failure(this);
        }
    }
}
