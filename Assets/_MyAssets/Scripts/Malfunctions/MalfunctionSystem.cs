using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalfunctionSystem : MonoBehaviour
{
    [SerializeField] private SubmarineControlSwitchboard submarineControlSwitchboard;

    [SerializeField] private List<Malfunction> malfunctions;

    public void Collision(Collision collision)
    {

    }
}
