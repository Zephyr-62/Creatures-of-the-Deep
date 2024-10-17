using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HydraulicFailure : Malfunction
{
    [HideInInspector] public PhysicalControlSurface affectedControl;
    [SerializeField] private float impactThresshold;
    [SerializeField] private float impactRandomness;

    public override void Enter()
    {
        base.Enter();
        affectedControl.Block();
    }

    public override void Exit()
    {
        base.Exit();
        affectedControl.Unblock();
    }

    public override bool IsFixed()
    {
        return false;
    }

    public override void Update()
    {
        
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
