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
        Alert = 1,
        Shake = 2,
        C = 4,
        D = 8,
        E = 16,
        F = 32,
    }
}
