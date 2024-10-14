using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBreaker : Measureable
{
    [SerializeField] private PhysicalControlSurface pcs;
    [SerializeField] private float surgeCap = 4f;

    private void OnEnable()
    {
        pcs.onValueChanged.AddListener(OnChange);
    }

    private void OnChange()
    {
        ElectricalDevice.PowerAll(pcs.GetBoolValue());
    }

    public override float Measure()
    {
        return ElectricalDevice.TotalSurge();
    }

    public override Vector2 GetRange()
    {
        return new Vector2(0f, surgeCap);
    }
}
