using AdvancedEditorTools.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EngineFailure : Malfunction
{
    [SerializeField] private float impactThresshold;
    [SerializeField] private float impactRandomness;

    private bool isFixed;

    public override void Enter(MalfunctionTrigger trigger = null)
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
        system.engine.onSuccessfullStart.RemoveListener(OnSuccessfullStart);
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
        if(!Enabled && (!system.engine.power.GetBoolValue() || !system.engine.isPowered)) 
        {
            system.Failure(this);
        }
    }

    public override void OnCollision(Collision collision, float f)
    {
        base.OnCollision(collision, f);

        if (!Enabled && f > UnityEngine.Random.Range(impactThresshold, impactThresshold + impactRandomness))
        {
            system.Failure(this);
        }
    }
}
