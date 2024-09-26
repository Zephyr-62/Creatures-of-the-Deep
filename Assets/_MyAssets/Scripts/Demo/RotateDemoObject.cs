using UnityEngine;

public class RotateDemoObject : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
    }
}
