using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBreaker : Measureable
{
    [SerializeField] private PhysicalControlSurface pcs;

    private void OnEnable()
    {
        pcs.onValueChanged.AddListener(OnChange);
    }

    private void Update()
    {
        //ElectricalDevice.TotalSurge();
    }

    private void OnChange()
    {
        ElectricalDevice.PowerAll(pcs.GetBoolValue());
    }

    public override float Measure()
    {
        return ElectricalDevice.globalSurge;
    }

    public override Vector2 GetRange()
    {
        return new Vector2(0f, 1f);
    }
}
