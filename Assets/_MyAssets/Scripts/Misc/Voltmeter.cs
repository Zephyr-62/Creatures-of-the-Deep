using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voltmeter : ElectricalDevice
{
    [SerializeField] private Transform dial;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    [SerializeField] private float speed;
    [SerializeField] ElectricalDevice device;
    [SerializeField] float flux = 0.05f;

    float seed;
    bool power = true;

    private void Awake()
    {
        seed = UnityEngine.Random.Range(0f, 10f);
    }

    protected override void OnPowerOff()
    {
        power = false;
    }

    protected override void OnPowerOn()
    {
        power = true;
    }

    protected override void OnSurge()
    {
        
    }

    void Update()
    {
        if (!power || !device) return;

        var t = Mathf.InverseLerp(-1, 1, Mathf.Sin((Time.time + seed) * speed));

        var angle = Mathf.Lerp(minAngle * Mathf.Clamp01(device.surge + flux), maxAngle * Mathf.Clamp01(device.surge + flux), t);

        dial.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
