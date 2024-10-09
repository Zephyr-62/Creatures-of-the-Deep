using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CollectArtefactQuest : Quest
{
    [SerializeField] private Artefact targetArtefact;

    private void OnEnable()
    {
        targetArtefact.pickedUp.AddListener(FinishQuest);
    }

    private void OnDisable()
    {
        targetArtefact.pickedUp.RemoveListener(FinishQuest);
    }

    public override void StartQuest()
    {
        targetArtefact.GetComponent<Artefact>().enabled = true;
    }
}
