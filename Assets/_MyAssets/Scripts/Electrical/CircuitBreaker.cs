using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBreaker : MonoBehaviour
{
    [SerializeField] private PhysicalControlSurface pcs;

    private void OnEnable()
    {
        pcs.onValueChanged.AddListener(OnChange);
    }

    private void OnChange()
    {
        ElectricalDevice.PowerAll(pcs.GetBoolValue());
    }
}
