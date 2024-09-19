using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ArtefactPickupSensor : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;
    
    private readonly string _tagFilter = "Artefact";
    private List<Collider> _colliders = new();
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GameObject().CompareTag(_tagFilter))
        {
            _colliders.Add(other);
            onTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GameObject().CompareTag(_tagFilter))
        {
            _colliders.Remove(other);
            onTriggerExit.Invoke();
        }
    }

    public void PickUpArtefact()
    {
        GameObject artefact = _colliders[0].gameObject;
        ScriptableArtefact artefactInfo = ScriptableArtefact.GetArtefactInfo(artefact.name);
        
        _colliders.Clear();
        Destroy(artefact);
        
        print("Picked up " + artefactInfo.artName + " artefact!");
    }
    
    
}
