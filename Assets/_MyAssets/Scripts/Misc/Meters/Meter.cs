using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meter : ElectricalDevice
{
    [SerializeField] Measureable device;
    [SerializeField] private Transform dial;
    [SerializeField] protected float minAngle;
    [SerializeField] protected float maxAngle;
    [SerializeField] protected float flux = 0.01f;
    protected float seed;

    float smoothValue;


    private void Awake()
    {
        seed = UnityEngine.Random.Range(0f, 10f);
    }

    protected override void OnPowerLost()
    {
    }

    protected override void OnPowerGained()
    {
    }

    protected override void OnSurge()
    {

    }

    void Update()
    {
        SetAngle(GetAngle(Value()));
    }


    private float Value()
    {
        var value = 0f;
        if (device && HasPower)
        {
            var edges = device.GetRange();
            value = Mathf.Clamp01(Mathf.InverseLerp(edges.x, edges.y, Mathf.Clamp(device.Measure(), edges.x, edges.y)));
        } else
        {
            value = 0;
        }
        var delta = 10f * Time.deltaTime;
        smoothValue = (smoothValue * (1f - delta)) + value * delta;

        var t = Mathf.InverseLerp(-1, 1, Mathf.Sin((Time.time + seed) * 15)) * flux;

        return smoothValue + (t * smoothValue);
    }

    private void SetAngle(float angle)
    {
        dial.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected virtual float GetAngle(float value)
    {
        return Mathf.Lerp(minAngle, maxAngle, value);
    } 
}
