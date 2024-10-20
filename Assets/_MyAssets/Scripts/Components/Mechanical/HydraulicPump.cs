using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HydraulicPump : Measureable
{
    [SerializeField] private SubmarinePhysicsSystem _system;
    [SerializeField] private HandCrank _crankA;
    [SerializeField] private HandCrank _crankB;
    [SerializeField] private HandCrank _crankC;
    [SerializeField] private HandCrank _crankD;
    [SerializeField] private HandCrank _crankE;

    [SerializeField] private Valve root;


    public UnityEvent<PhysicalControlSurface> OnVent;
    public UnityEvent<PhysicalControlSurface> OnDecompress;

    public void Vent()
    {
        CheckValve(root);
    }

    private void CheckValve(Valve valve)
    {
        if(valve.crank.Get01FloatValue() > 0.2f)
        {
            CheckValve(valve.rightValve);
            Vent(valve.rightPCS);
        } else if(valve.crank.Get01FloatValue() < 0.8f)
        {
            CheckValve(valve.leftValve);
            Vent(valve.leftPCS);
        }
    }

    private void Vent(PhysicalControlSurface pcs)
    {
        if (!pcs) return;
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
