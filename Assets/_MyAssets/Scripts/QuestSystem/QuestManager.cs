using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<Quest> quests = new();
    [SerializeField] private Quest currentQuest;
    [SerializeField] private Button newQuestButton;

    private void OnEnable()
    {
        newQuestButton.onValueChanged.AddListener(StartNewQuest);
    }

    private void OnDisable()
    {
        newQuestButton.onValueChanged.RemoveListener(StartNewQuest);
    }
    
    private void StartNewQuest()
    {
        if (!newQuestButton.GetBoolValue()) return;
        if (currentQuest == null && quests.Count > 0)
        {
            currentQuest = quests[0];
            quests.RemoveAt(0);
            
            currentQuest.questFinished.AddListener(FinishCurrentQuest);
            // turn button red
            newQuestButton.Block();
            PrintQuestInfo();

        }
        else if (currentQuest != null)
        {
            print("You already have a quest");
            PrintQuestInfo();
        }
    }

    private void FinishCurrentQuest()
    {
        currentQuest = null;
        // turn button green
        newQuestButton.Unblock();
    }

    private void PrintQuestInfo()
    {
        print("Quest Name: " + currentQuest.GetQuestName());
        print("Description " + currentQuest.GetDescription());
    }
}
