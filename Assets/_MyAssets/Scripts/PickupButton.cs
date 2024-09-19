using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupButton : MonoBehaviour
{
    [SerializeField] private Material materialGreen;
    [SerializeField] private Material materialRed;
    
    [SerializeField] private GameObject buttonObject;
    private Renderer _renderer;
    
    private void Start()
    {
        _renderer = buttonObject.GetComponent<MeshRenderer>();
    }

    public void Activate()
    {
        _renderer.material = materialGreen;
    }
    
    public void Deactivate()
    {
        _renderer.material = materialRed;
    }
}
