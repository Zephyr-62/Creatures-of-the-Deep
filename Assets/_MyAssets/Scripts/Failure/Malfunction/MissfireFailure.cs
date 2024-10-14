using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MissfireFailure : Malfunction
{
    public override bool IsFixed()
    {
        return false;
    }

    public override void Update()
    {
        if (Enabled)
        {
            system.physicsSystem.SetMaxPower(Mathf.Round(Mathf.PerlinNoise1D(Time.time * 10f)));
        }
    }
}
