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

    public override void OnCollision(Collision collision)
    {
        base.OnCollision(collision);

        Rigidbody rb1 = system.GetComponentInParent<Rigidbody>();
        Rigidbody rb2 = collision.rigidbody;

        float mass1 = rb1.mass;
        float mass2 = rb2 != null ? rb2.mass : 1000f;

        Vector3 relativeVelocity = collision.relativeVelocity;

        float reducedMass = (2 * mass1 * mass2) / (mass1 + mass2);
        float collisionForce = reducedMass * relativeVelocity.magnitude;

        if (!Enabled && collisionForce > UnityEngine.Random.Range(impactThresshold, impactThresshold + impactRandomness))
        {
            system.Failure(this);
        }
    }
}
