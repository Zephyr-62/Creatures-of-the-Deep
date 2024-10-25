using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ElevationFailure : Malfunction
{
    public Terrain terrain;
    public float elevationLimit = 100;

    private float elevation;
    private MalfunctionTrigger trigger;

    public override void Enter(MalfunctionTrigger trigger = null)
    {
        base.Enter();
        system.physicsSystem.TurnOff();
        this.trigger = trigger;
    }

    public override void Exit()
    {
        base.Exit();
        system.Failure(system.engineFailure);
    }

    public override bool IsFixed()
    {
        return elevation < elevationLimit && (!trigger || !trigger.triggered);
    }

    public override void Update()
    {
        base.Update();

        if (terrain == null) return;
        var t = terrain.SampleHeight(system.physicsSystem.transform.position);

        elevation = system.physicsSystem.transform.position.y - t;

        if (!Enabled && elevation > elevationLimit)
        {
            system.Failure(this);
        }
    }
}
