using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/ArtefactQuest")]
public class ArtefactQuest : Quest
{
    [SerializeField] private int targetArtefactID;

    public override void StartQuest(QuestSystem qs)
    {
        qs.artefactSystem.SetTargetArtefact(targetArtefactID);
    }

    public override bool IsCompleted(QuestSystem qs)
    {
        return qs.artefactSystem.WasArtefactCollected(targetArtefactID);
    }

    public override Vector3 Debug()
    {
        Vector3 artefactPosition = FindObjectsByType<Artefact>(FindObjectsSortMode.None).ToList().Find(a => a.artID == targetArtefactID)
            .transform.position;

        Gizmos.DrawWireSphere(artefactPosition, 1);

        return artefactPosition;
    }
}