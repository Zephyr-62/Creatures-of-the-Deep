using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Symptom
{
    private int count;

    public void Do(MalfunctionSystem system)
    {
        count++;
        if(count == 1)
        {
            Fail(system);
        }
    }

    public void Undo(MalfunctionSystem system)
    {
        count--;
        count = Mathf.Max(count, 0);
        if(count == 0)
        {
            Cure(system);
        }
    }

    protected abstract void Fail(MalfunctionSystem system);
    protected abstract void Cure(MalfunctionSystem system);

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
