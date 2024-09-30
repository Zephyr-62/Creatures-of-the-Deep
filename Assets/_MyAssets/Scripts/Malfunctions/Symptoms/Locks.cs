using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockThrottle : Symptom
{
    protected override void Cure(SubmarineControlSwitchboard system)
    {
        system.UnblockThrottle();
    }

    protected override void Fail(SubmarineControlSwitchboard system)
    {
        system.BlockThrottle();
    }
}

public class LockSteering : Symptom
{
    protected override void Cure(SubmarineControlSwitchboard system)
    {
        system.UnblockSteering();
    }

    protected override void Fail(SubmarineControlSwitchboard system)
    {
        system.BlockSteering();
    }
}

public class LockPitch : Symptom
{
    protected override void Cure(SubmarineControlSwitchboard system)
    {
        system.UnblockSteering();
    }

    protected override void Fail(SubmarineControlSwitchboard system)
    {
        system.BlockSteering();
    }
}

public class LockElevation : Symptom
{
    protected override void Cure(SubmarineControlSwitchboard system)
    {
        system.UnblockSteering();
    }

    protected override void Fail(SubmarineControlSwitchboard system)
    {
        system.BlockSteering();
    }
}