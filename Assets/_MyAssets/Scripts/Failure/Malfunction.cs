using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Symptom;

public abstract class Malfunction
{
    [SerializeField] protected ErrorMask errorCode;
    [SerializeField] protected SymptomMask symptoms;

    public ErrorMask ErrorCode => errorCode;
    public SymptomMask Symptoms => symptoms;

    [Flags]
    public enum ErrorMask
    {
        None = 0,
        A1 = 1,
        A2 = 2,
        A3 = 4,
        A4 = 8,
        B1 = 16,
        B2 = 32,
        B3 = 64,
        B4 = 128,
        C1 = 256,
        C2 = 512,
        C3 = 1024,
        C4 = 2048,
        D1 = 4096,
        D2 = 8192,
        D3 = 16384,
        D4 = 32768,
    }

    public abstract void Enter(MalfunctionSystem system);
    public abstract void Exit(MalfunctionSystem system);
    public abstract bool IsFixed(MalfunctionSystem system);

    private bool enabled;
    public bool Enabled => enabled;

    public void Enable(bool enabled)
    {
        this.enabled = enabled;
    }
}
