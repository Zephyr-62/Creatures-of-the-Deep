using UnityEngine;
using UnityEngine.Events;

public class PickupButton : MonoBehaviour
{
    [SerializeField] private Material materialGreen;
    [SerializeField] private Material materialRed;
    [SerializeField] private GameObject buttonObject;
    
    [SerializeField] private UnityEvent onButtonPressed;
    
    private Renderer _renderer;
    private bool _buttonEnabled;
    
    private void Start()
    {
        _renderer = buttonObject.GetComponent<MeshRenderer>();
    }
    
    private void Update()
    {
        if (_buttonEnabled && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform == buttonObject.transform)
                {
                    onButtonPressed.Invoke();
                }
            }
        }
    }

    public void Activate()
    {
        _buttonEnabled = true;
        _renderer.material = materialGreen;
    }
    
    public void Deactivate()
    {
        _buttonEnabled = false;
        _renderer.material = materialRed;
    }
}
