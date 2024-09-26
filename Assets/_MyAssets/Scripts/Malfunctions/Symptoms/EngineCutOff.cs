using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineCutOff : Symptom
{
    protected override void Cure(SubmarineControlSwitchboard system)
    {
        system.StartEngine();
    }

    protected override void Fail(SubmarineControlSwitchboard system)
    {
        system.StopEngine();
    }
}
