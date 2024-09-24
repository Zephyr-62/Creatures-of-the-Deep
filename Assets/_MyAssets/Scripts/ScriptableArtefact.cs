using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Artefact", menuName = "Artefact")]
public class ScriptableArtefact : ScriptableObject
{
    public string artName;
    [TextArea] public string artDescription;
    public Collection artCollection;
    public Category artCategory;
    public Substance artSubstance;

    private string ArtefactID => name;

    private static Dictionary<string, ScriptableArtefact> _cache;

    private static Dictionary<string, ScriptableArtefact> Cache
    {
        get
        {
            if (_cache == null)
            {
                // Load all Scriptable Artefacts from our Resources folder
                ScriptableArtefact[] cards = Resources.LoadAll<ScriptableArtefact>("");

                _cache = cards.ToDictionary(card => card.ArtefactID, card => card);
            }

            return _cache;
        }
    }

    public static ScriptableArtefact GetArtefactInfo(string artID)
    {
        return Cache[artID];
    }
}

[Serializable]
public enum Category
{
    Statue,
    Jewelry,
    Armor,
    Weapon
}

[Serializable]
public enum Collection
{
    Greek,
    Roman,
    Rare
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