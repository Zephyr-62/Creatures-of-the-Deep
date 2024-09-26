using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MalfunctionGroup : ScriptableObject
{
    [SerializeField] private List<Malfunction> malfunctions;
}
