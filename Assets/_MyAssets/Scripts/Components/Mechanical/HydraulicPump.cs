using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraulicPump : Measureable
{
    [SerializeField] private SubmarinePhysicsSystem _system;
    [SerializeField] private HandCrank _crankThrust;
    [SerializeField] private HandCrank _crankSteering;
    [SerializeField] private HandCrank _crankPitch;
    [SerializeField] private HandCrank _crankElevation;

    public void Vent()
    {
        CheckVent(_system.throttleControl, _crankThrust);
    }

    private void CheckVent(PhysicalControlSurface pcs, HandCrank crank)
    {
        var v = crank.Get01FloatValue();
        if (pcs.isBlocked && v >= 0.8)
        {
            pcs.Unblock();
        }
        else if (v >= 0.2f)
        {
            pcs.Block();
        }
    }

    public override Vector2 GetRange()
    {
        return new Vector2(0, 1f);
    }

    public override float Measure()
    {
        return 0f;
    }
}
