using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem : MonoBehaviour
{
    [SerializeField] private MalfunctionSystem system;
    [SerializeField] private CollisionAudio audioPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        system.Collision(collision);
        var instance = Instantiate(audioPrefab, transform);
        instance.transform.position = collision.GetContact(0).point;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MalfunctionTrigger>(out var comp))
        {
            comp.triggered = true;
            switch (comp.failure)
            {
                case MalfunctionTrigger.Failure.EngineCutoff:
                    system.Failure(system.engineFailure);
                    break;
                case MalfunctionTrigger.Failure.ElevationFailure:
                    system.Failure(system.elevationFailure);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MalfunctionTrigger>(out var comp))
        {
            comp.triggered = false;
        }
    }
}
