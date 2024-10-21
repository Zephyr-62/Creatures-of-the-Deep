using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Valve;

public class HydraulicPump : Measureable
{
    [SerializeField] private SubmarinePhysicsSystem _system;

    [SerializeField] private Valve root;
    [SerializeField] private RouterIndicator indicator;


    public UnityEvent<PhysicalControlSurface> OnVent;
    public UnityEvent<PhysicalControlSurface> OnDecompress;

    public void Vent()
    {
        CheckValve(root);
    }

    private void Update()
    {
        indicator.Set(true);
    }

    private void CheckValve(Valve valve)
    {
        if (valve == null) return;
        if(valve.crank.Get01FloatValue() > 0.2f)
        {
            CheckValve(valve.rightValve);
            Vent(valve.rightTarget);
        } else if(valve.crank.Get01FloatValue() < 0.8f)
        {
            CheckValve(valve.leftValve);
            Vent(valve.leftTarget);
        }
    }

    private void Vent(Target target)
    {
        switch (target)
        {
            case Target.None:
                break;
            case Target.Thrust:
                Vent(_system.throttleControl);
                break;
            case Target.Steering:
                Vent(_system.steeringControl);
                break;
            case Target.Pitch:
                Vent(_system.pitchControl);
                break;
            case Target.Elevation:
                Vent(_system.elevationControl);
                break;
            case Target.Engine:
                break;
            case Target.Exhaust:
                break;
            default:
                break;
        }
    }


    private void Vent(PhysicalControlSurface pcs)
    {
        if (!pcs) return;
        Debug.Log("Vented: " + pcs.name);
        if (pcs.isBlocked)
        {
            OnVent.Invoke(pcs);
        }
        else
        {
            OnDecompress.Invoke(pcs);
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
