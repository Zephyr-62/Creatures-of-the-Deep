using System.Collections.Generic;
using UnityEngine;

public class ArtefactPickupSensor : MonoBehaviour
{
    [SerializeField] private Vector3 sensorSize = new(5f, 3f, 5f);
    [SerializeField] private Button pickupButton;

    private BoxCollider _sensorCollider;
    private readonly List<Artefact> _artefacts = new();

    private void OnValidate()
    {
        _sensorCollider = GetComponent<BoxCollider>();
        _sensorCollider.size = sensorSize;
    }

    private void Start()
    {
        pickupButton.Block();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, sensorSize);
    }

    private void OnEnable()
    {
        pickupButton.onGrabbed.AddListener(PickUpArtefact);
    }

    private void OnDisable()
    {
        pickupButton.onGrabbed.RemoveListener(PickUpArtefact);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Artefact artefact))
        {
            _artefacts.Add(artefact);
            pickupButton.Unblock();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Artefact artefact))
        {
            _artefacts.Remove(artefact);
            pickupButton.Block();
        }
    }

    public void PickUpArtefact()
    {
        if (_artefacts.Count == 0) return;
        
        Artefact artefact = _artefacts[0];
        _artefacts.Clear();
        pickupButton.Block();

        ScriptableArtefact artefactInfo = artefact.GetInfo();

        print(artefactInfo.artName);
        print(artefactInfo.artDescription);

        Destroy(artefact.gameObject);
    }
}