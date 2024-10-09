using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EngineFailure : Malfunction
{
    public override void Enter(MalfunctionSystem system)
    {
        system.utilities.ignition.SetBoolValue(false);
    }

    public override void Exit(MalfunctionSystem system)
    {
        system.utilities.ignition.SetBoolValue(false);
    }

    public override bool IsFixed(MalfunctionSystem system)
    {
        return system.utilities.power.GetBoolValue()
            && system.utilities.ignition.GetBoolValue()
            && system.utilities.starter.GetBoolValue();
    }
}
