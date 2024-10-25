using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalfunctionTrigger : MonoBehaviour
{
    public Failure failure;
    public bool triggered;

    public enum Failure
    {
        EngineCutoff,
        ElevationFailure,
        SonarVoltage,
        ScreenVoltage
    }
}
