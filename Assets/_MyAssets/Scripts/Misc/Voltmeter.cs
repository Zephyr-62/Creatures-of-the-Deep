using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guage : MonoBehaviour
{
    [SerializeField] private Transform dial;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    [SerializeField] private float speed;
    [SerializeField] private float value;

    void Update()
    {

        var t = Mathf.InverseLerp(-1, 1, Mathf.Sin(Time.time * speed));

        var angle = Mathf.Lerp(minAngle * value, maxAngle * value, t);

        dial.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
