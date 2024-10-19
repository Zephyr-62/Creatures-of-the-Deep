using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraulicPump : Measureable
{
    [SerializeField] private HandCrank crankA;
    [SerializeField] private HandCrank crankB;
    [SerializeField] private HandCrank crankC;
    [SerializeField] private HandCrank crankD;

    public override Vector2 GetRange()
    {
        return new Vector2(0, 1f);
    }

    public override float Measure()
    {
        return 0f;
    }
}
