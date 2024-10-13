using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMeter : ElectricalDevice
{
    [SerializeField] private Transform dial;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    [SerializeField] Measureable device;
    [SerializeField] float flux = 0.05f;

    float seed;
    bool power = true;

    private void Awake()
    {
        seed = UnityEngine.Random.Range(0f, 10f);
    }

    protected override void OnPowerLost()
    {
        power = false;
    }

    protected override void OnPowerGained()
    {
        power = true;
    }

    protected override void OnSurge()
    {

    }

    void Update()
    {
        if (!power) return;

        var t = Mathf.InverseLerp(-1, 1, Mathf.Sin((Time.time + seed) * 2));

        var v = Value();

        var angle = Mathf.Lerp(minAngle * v, maxAngle * v, t);

        dial.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private float Value()
    {
        if(!power) return 0f;
        if (!device) return flux;
        var edges = device.GetRange();
        return Mathf.Clamp01(Mathf.InverseLerp(edges.x, edges.y, Mathf.Clamp(device.Measure() + flux, edges.x, edges.y)) + flux);
    }
}
