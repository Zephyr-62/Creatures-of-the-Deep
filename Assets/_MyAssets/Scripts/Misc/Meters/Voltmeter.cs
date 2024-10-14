using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voltmeter : Meter
{
    [SerializeField] private float idleSpeed;
    [SerializeField] private float fullSpeed;
    [SerializeField] private Mode mode;

    float phase = 0f;

    protected override float GetAngle(float value)
    {
        if (mode == Mode.Frequency)
        {
            phase += 2 * Mathf.PI * Mathf.Lerp(idleSpeed, fullSpeed, value) * Time.deltaTime;

            if (phase > 2 * Mathf.PI)
                phase -= 2 * Mathf.PI;

            var t = Mathf.InverseLerp(-1f, 1f, Mathf.Sin(phase + seed));
            if (!isPowered) return 0;
            return Mathf.Lerp(minAngle, maxAngle, t);
        }
        else
        {
            var t = Mathf.InverseLerp(-1, 1, Mathf.Sin((Time.time + seed) * idleSpeed));
            var v = value + (isPowered ? flux : 0f);
            return Mathf.Lerp(minAngle * v, maxAngle * v, t);
        }
    }

    enum Mode
    {
        Frequency,
        Amplitude
    }
}
