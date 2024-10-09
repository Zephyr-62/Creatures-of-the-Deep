using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockThrottle : Symptom
{
    protected override void Cure(MalfunctionSystem system)
    {
        system.controls.UnblockThrottle();
    }

    protected override void Fail(MalfunctionSystem system)
    {
        system.controls.BlockThrottle();
    }
}

public class LockSteering : Symptom
{
    protected override void Cure(MalfunctionSystem system)
    {
        system.controls.UnblockSteering();
    }

    protected override void Fail(MalfunctionSystem system)
    {
        system.controls.BlockSteering();
    }
}

public class LockPitch : Symptom
{
    protected override void Cure(MalfunctionSystem system)
    {
        system.controls.UnblockSteering();
    }

    protected override void Fail(MalfunctionSystem system)
    {
        system.controls.BlockSteering();
    }
}

public class LockElevation : Symptom
{
    protected override void Cure(MalfunctionSystem system)
    {
        system.controls.UnblockSteering();
    }

    protected override void Fail(MalfunctionSystem system)
    {
        system.controls.BlockSteering();
    }
}