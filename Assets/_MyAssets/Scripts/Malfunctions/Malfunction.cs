using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Malfunction : ScriptableObject
{
    [SerializeField] private ErrorIndicator errorIndicator;
    [SerializeField] private List<MalfunctionSymptom> symptoms;

    [SerializeField] private GameObject test;

    [Flags]
    public enum ErrorIndicator
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
    }
}
