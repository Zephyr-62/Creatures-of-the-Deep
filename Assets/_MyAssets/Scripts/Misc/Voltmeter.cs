using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voltmeter : MonoBehaviour
{
    [SerializeField] private Transform dial;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    [SerializeField] private float speed;
    [SerializeField] ElectricalDevice device;

    void Update()
    {
        var t = Mathf.InverseLerp(-1, 1, Mathf.Sin(Time.time * speed));

        var angle = Mathf.Lerp(minAngle * Mathf.Clamp01(device.surge), maxAngle * Mathf.Clamp01(device.surge), t);

        dial.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
