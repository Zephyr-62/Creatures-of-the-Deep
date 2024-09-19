using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ArtefactPickupSensor : MonoBehaviour
{
    [SerializeField] string tagFilter = "Artefact";
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GameObject().CompareTag(tagFilter))
        {
            onTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GameObject().CompareTag(tagFilter))
        {
            onTriggerExit.Invoke();
        }
    }
}
