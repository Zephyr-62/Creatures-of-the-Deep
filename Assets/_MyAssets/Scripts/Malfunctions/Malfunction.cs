using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MalfunctionSymptom;

[CreateAssetMenu]
public class Malfunction : ScriptableObject
{
    [SerializeField] private ErrorMask errorCode;
    [SerializeField] private SymptomMask symptoms;

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
}
