using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineUtilitySwitchboard : MonoBehaviour
{
    [Header("Engine")]
    public PhysicalControlSurface power;
    public PhysicalControlSurface ignition;
    public Pulley starter;

    [Header("Hydraulics")]
    public PhysicalControlSurface compressorReset;
    public PhysicalControlSurface powerReset;
}
