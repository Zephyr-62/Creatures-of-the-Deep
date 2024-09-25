using UnityEngine;

public class Artefact : MonoBehaviour
{
    [SerializeField] private ScriptableArtefact artefactInfo;

    public ScriptableArtefact GetInfo()
    {
        return artefactInfo;
    }
}
