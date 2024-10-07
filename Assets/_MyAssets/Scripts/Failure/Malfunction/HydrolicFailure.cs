using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HydrolicFailure : Malfunction
{
    public override void Enter(MalfunctionSystem system)
    {

    }

    public override void Exit(MalfunctionSystem system)
    {

    }

    public override bool IsFixed(MalfunctionSystem system)
    {
        return false;
    }
}
