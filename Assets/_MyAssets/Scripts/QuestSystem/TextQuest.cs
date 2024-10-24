using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Quest/TextQuest")]
public class TextQuest : Quest
{
    public override Vector3 Debug()
    {
        return Vector3.zero;
    }

    public override bool IsCompleted(QuestSystem qs)
    {
        return true;
    }

    public override void StartQuest(QuestSystem qs)
    {
        
    }
}
