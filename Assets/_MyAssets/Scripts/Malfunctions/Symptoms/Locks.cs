using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockThrottle : MalfunctionSymptom
{
    public LockThrottle(SymptomMask id) : base(id)
    {

    }

    protected override void Cure(SubmarineControlSwitchboard system)
    {
        system.BlockThrottle();
    }

    protected override void Fail(SubmarineControlSwitchboard system)
    {
        system.UnblockThrottle();
    }
}

public class LockSteering : MalfunctionSymptom
{
    public LockSteering(SymptomMask id) : base(id)
    {

    }

    protected override void Cure(SubmarineControlSwitchboard system)
    {
        system.BlockSteering();
    }

    protected override void Fail(SubmarineControlSwitchboard system)
    {
        system.BlockSteering();
    }
}

public class LockPitch : MalfunctionSymptom
{
    public LockPitch(SymptomMask id) : base(id)
    {

    }

    protected override void Cure(SubmarineControlSwitchboard system)
    {
        system.BlockSteering();
    }

    protected override void Fail(SubmarineControlSwitchboard system)
    {
        system.BlockSteering();
    }
}

public class LockElevation : MalfunctionSymptom
{
    public LockElevation(SymptomMask id) : base(id)
    {

    }

    protected override void Cure(SubmarineControlSwitchboard system)
    {
        system.BlockSteering();
    }

    protected override void Fail(SubmarineControlSwitchboard system)
    {
        system.BlockSteering();
    }
}