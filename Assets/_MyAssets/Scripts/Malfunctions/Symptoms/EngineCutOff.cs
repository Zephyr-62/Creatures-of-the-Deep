using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineCutOff : MalfunctionSymptom
{
    public EngineCutOff(SymptomMask id) : base(id)
    {

    }

    protected override void Cure(SubmarineControlSwitchboard system)
    {
        system.StartEngine();
    }

    protected override void Fail(SubmarineControlSwitchboard system)
    {
        system.StopEngine();
    }
}
