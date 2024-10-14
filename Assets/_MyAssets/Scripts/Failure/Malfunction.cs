using AdvancedEditorTools.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Symptom;

public abstract class Malfunction
{
    [SerializeField] protected ErrorMask errorCode;
    [SerializeField] protected SymptomMask symptoms;

    protected MalfunctionSystem system;

    public ErrorMask ErrorCode => errorCode;
    public SymptomMask Symptoms => symptoms;

    [Flags]
    public enum ErrorMask
    {
        None = 0,
        ENG = 1, //ENGINE
        H_MFD = 2, //HYDRAULICS
        SONR = 4, //SONAR
        BRK = 8, //Breakers
        CAM = 16, //Camera
        PWR = 32, //Power
        PMP = 128, //Pump
        THR = 256, //Throttle
        FIN = 512, //Pitch -> fins
        RDR = 1024, //Rudder -> steering
        DCTRL = 2048, //Depth control -> elevation
        SPEC = 4096, //Spectronomer
        VOL = 8192, //Voltage
        HPE = 16384, //High Priority Emergency
        HULL_SYS = 32768, //Hull System
        BAR = 65536 // Pressure issue 
    }

    public virtual void Enter() { enabled = true; }
    public virtual void Exit() { enabled = false; }
    public abstract bool IsFixed();
    public virtual void Update() { }

    public virtual void OnCollision(Collision collision)
    {
        
    }

    private bool enabled;
    public bool Enabled => enabled;

    public void AttachSystem(MalfunctionSystem system)
    {
        this.system = system;
    }
}
