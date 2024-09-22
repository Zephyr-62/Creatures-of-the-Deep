using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalfunctionSystem : MonoBehaviour
{
    [SerializeField] private SubmarineControlSwitchboard submarineSwitchboard;

    [SerializeField] private List<Malfunction> malfunctions;

    public void Impact(Vector3 force, Vector3 position)
    {
        
    }
}
