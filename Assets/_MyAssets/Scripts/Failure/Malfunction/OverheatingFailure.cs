using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class OverheatingFailure : Malfunction
{
    [SerializeField] private float recoveryHeatLevel;

    public override void Enter()
    {
        base.Enter();
        system.physicsSystem.TurnOff();
        system.engine.Overheat(true);
    }

    public override void Exit()
    {
        base.Exit();
        system.Failure(system.engineFailure);
        system.engine.Overheat(false);
    }

    public override bool IsFixed()
    {
        return system.engine.heat <= recoveryHeatLevel;
    }

    public override void Update()
    {
        if (!Enabled && system.engine.heat > system.engine.heatCapacity)
        {
            system.Failure(this);
        }
    }
}
