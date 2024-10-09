using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MissfireFailure : Malfunction
{
    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override bool IsFixed()
    {
        return false;
    }

    public override void Update()
    {
        system.engine.SetMaxPower(Mathf.Round(Mathf.PerlinNoise1D(Time.time * 10f)));
    }
}
