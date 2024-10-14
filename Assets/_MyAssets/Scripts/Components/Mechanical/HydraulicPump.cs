using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraulicPump : Measureable
{
    [SerializeField] private Button pressureRelease;

    public override Vector2 GetRange()
    {
        return new Vector2(0, 1f);
    }

    public override float Measure()
    {
        return 0f;
    }
}
