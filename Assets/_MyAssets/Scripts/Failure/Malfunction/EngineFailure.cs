using AdvancedEditorTools.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EngineFailure : Malfunction
{
    public override void Enter()
    {
        base.Enter();
        system.engine.TurnOff();
        system.utilities.ignition.SetBoolValue(false);
    }

    public override void Exit()
    {
        base.Exit();
        system.engine.TurnOn();
        system.utilities.ignition.SetBoolValue(false);
    }

    public override bool IsFixed()
    {
        return system.utilities.power.GetBoolValue()
            && system.utilities.ignition.GetBoolValue()
            && system.utilities.starter.GetBoolValue();
    }

    public override void Update()
    {
        
    }
}
