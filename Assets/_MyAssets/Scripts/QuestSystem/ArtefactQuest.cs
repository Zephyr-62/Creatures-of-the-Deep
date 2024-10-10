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
}