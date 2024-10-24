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

    private bool _fixed;

    public override void Enter(MalfunctionTrigger trigger = null)
    {
        base.Enter();
        affectedControl.Block();
        _fixed = false;
    }

    public override void Exit()
    {
        base.Exit();
        affectedControl.Unblock();
        _fixed = false;
    }

    public override bool IsFixed()
    {
        return _fixed;
    }

    public override void Update()
    {
        
    }

    public override void OnCollision(Collision collision, float force)
    {
        base.OnCollision(collision, force);

        if (!Enabled && force > UnityEngine.Random.Range(impactThresshold, impactThresshold + impactRandomness))
        {
            system.Failure(this);
        }
    }

    public override void AttachSystem(MalfunctionSystem system)
    {
        base.AttachSystem(system);
        system.pump.OnVent.AddListener(OnVent);
        system.pump.OnDecompress.AddListener(OnDecompress);

    }

    private void OnVent(PhysicalControlSurface pcs)
    {
        if (pcs == affectedControl && Enabled)
        {
            _fixed = true;
        }
    }

    private void OnDecompress(PhysicalControlSurface pcs)
    {
        if(pcs == affectedControl && !Enabled)
        {
            system.Failure(this);
        }
    }
}
