using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Malfunction;

public abstract class MalfunctionSymptom
{
    private SymptomMask id;
    private int count;

    public SymptomMask Id => id;

    protected MalfunctionSymptom(SymptomMask id)
    {
        this.id = id;
    }

    public void Do(SubmarineControlSwitchboard system)
    {
        count++;
        if(count == 1)
        {
            Fail(system);
        }
    }

    public void Undo(SubmarineControlSwitchboard system)
    {
        count--;
        count = Mathf.Max(count, 0);
        if(count == 0)
        {
            Cure(system);
        }
    }

    protected abstract void Fail(SubmarineControlSwitchboard system);
    protected abstract void Cure(SubmarineControlSwitchboard system);

    [Flags]
    public enum SymptomMask
    {
        None = 0,
        LockThrottle = 1,
        LockSteering = 2,
        LockPitch = 4,
        LockElevation = 8,
        EngineCutOff = 16,
        PowerFailure = 32,
    }
}
