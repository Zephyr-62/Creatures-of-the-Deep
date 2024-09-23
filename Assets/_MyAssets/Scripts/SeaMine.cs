using UnityEngine;
using UnityEngine.Events;

public class SeaMine : MonoBehaviour
{
    [SerializeField] private UnityEvent explodeEvent;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        explodeEvent.Invoke();
    }
}
