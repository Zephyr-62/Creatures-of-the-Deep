using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HydraulicFailure : Malfunction
{
    [SerializeField] private PhysicalControlSurface affectedControl;
    
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
}
