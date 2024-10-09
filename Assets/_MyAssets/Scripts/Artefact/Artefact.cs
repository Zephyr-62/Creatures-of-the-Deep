using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Artefact : MonoBehaviour
{
    public string artName;
    [TextArea] public string artDescription;
    
    [SerializeField] public UnityEvent pickedUp;
}
