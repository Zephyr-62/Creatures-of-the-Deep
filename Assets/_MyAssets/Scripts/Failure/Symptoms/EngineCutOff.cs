using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineCutOff : Symptom
{
    protected override void Cure(MalfunctionSystem system)
    {
        system.controls.StartEngine();
    }

    protected override void Fail(MalfunctionSystem system)
    {
        system.controls.StopEngine();
    }
}
