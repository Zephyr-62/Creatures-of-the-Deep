using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Serializable]
public abstract class Quest : MonoBehaviour
{
    [SerializeField] private string questName;
    [SerializeField] [TextArea] private string description;
    
    private bool _isFinished;
    
    [SerializeField] public UnityEvent questFinished;

    public string GetQuestName() => questName;
    public string GetDescription() => description;

    public abstract void StartQuest();

    protected void FinishQuest()
    {
        if (!_isFinished)
        {
            _isFinished = true;
            
            questFinished.Invoke();
            
            Destroy(gameObject);
        }
    }
}
