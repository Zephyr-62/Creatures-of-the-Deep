using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailureCollider : MonoBehaviour
{
    [SerializeField] private MalfunctionSystem system;

    private void OnCollisionEnter(Collision collision)
    {
        system.Collision(collision);
    }
}
