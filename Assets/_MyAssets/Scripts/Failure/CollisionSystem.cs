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
}
