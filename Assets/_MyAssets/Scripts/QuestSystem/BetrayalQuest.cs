using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/BetrayalQuest")]
public class BetrayalQuest : Quest
{
    public override Vector3 Debug()
    {
        return Vector3.zero;
    }

    public override bool IsCompleted(QuestSystem qs)
    {
        return false;
    }

    public override void StartQuest(QuestSystem qs)
    {
        qs.malfunctionSystem.Failure(qs.malfunctionSystem.betrayal);
    }
}
