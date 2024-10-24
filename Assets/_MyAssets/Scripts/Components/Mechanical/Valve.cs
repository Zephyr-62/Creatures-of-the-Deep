using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : MonoBehaviour
{
    public HandCrank crank;
    public Valve leftValve;
    public Target leftTarget;
    public Valve rightValve;
    public Target rightTarget;

    public enum Target
    {
        None,
        Thrust,
        Steering,
        Pitch,
        Elevation,
        Engine,
        Exhaust
    }
}
