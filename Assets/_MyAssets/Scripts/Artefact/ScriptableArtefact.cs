using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Artefact", menuName = "Artefact")]
public class ScriptableArtefact : ScriptableObject
{
    public string artName;
    [TextArea] public string artDescription;
    public Substance[] artSubstance;
}

[Serializable]
public enum Substance
{
    Iron,
    Copper,
    Silver,
    Gold,
    Glass,
    Wood,
    Marble,
    Stone
}