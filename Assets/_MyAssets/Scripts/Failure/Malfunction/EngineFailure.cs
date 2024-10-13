using AdvancedEditorTools.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EngineFailure : Malfunction
{
    private bool isFixed;

    public override void Enter()
    {
        base.Enter();
        isFixed = false;
        system.physicsSystem.TurnOff();
        system.engine.onSuccessfullStart.AddListener(OnSuccessfullStart);
        system.engine.ignition.SetBoolValue(false);
    }

    public override void Exit()
    {
        base.Exit();
        system.physicsSystem.TurnOn();
        system.engine.ignition.SetBoolValue(false);
    }

    public override bool IsFixed()
    {
        return isFixed;
    }

    private void OnSuccessfullStart()
    {
        isFixed = true;
    }

    public override void Update()
    {
        base.Update();
        
        if(!Enabled && !system.engine.power.GetBoolValue()) 
        {
            system.Failure(this);
        }
    }
}
