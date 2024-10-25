using UnityEngine;

public class EnvironmentAreaCulling : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    private Renderer[] _renderers;
    private Collider[] _colliders;
    
    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _colliders = GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
        SetChildrenRendering(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out SubmarinePhysicsSystem _))
        {
            SetChildrenRendering(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out SubmarinePhysicsSystem _))
        {
            SetChildrenRendering(false);
        }
    }

    private void SetChildrenRendering(bool enable)
    {
        foreach (var childRenderer in _renderers)
        {
            childRenderer.enabled = enable;
        }
            
        foreach (var childCollider in _colliders)
        {
            childCollider.enabled = enable;
        }
        
        boxCollider.enabled = true;
    }
}
