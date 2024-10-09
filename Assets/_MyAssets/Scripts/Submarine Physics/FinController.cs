using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinController : MonoBehaviour
{
    [SerializeField] private SubmarinePhysicsSystem physicsSystem;
    [SerializeField] private float max, min;
    [SerializeField] private Transform fin;

    private void Update()
    {
        fin.localRotation = Quaternion.Euler(0f, Mathf.Lerp(min, max, physicsSystem.steering), 0f);
    }
}
