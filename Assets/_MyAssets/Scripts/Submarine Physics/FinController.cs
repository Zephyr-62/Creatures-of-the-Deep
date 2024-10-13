using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinController : MonoBehaviour
{
    [SerializeField] private PhysicalControlSurface pcs;
    [SerializeField] private float max, min;
    [SerializeField] private Transform fin;
    [SerializeField] private bool flip;

    private void Update()
    {
        var t = Mathf.InverseLerp(-1, 1, pcs.GetFloatValue() * (flip ? -1 : 1));
        fin.localRotation = Quaternion.Euler(0f, Mathf.Lerp(min, max, t), 0f);
    }
}
