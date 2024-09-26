using AdvancedEditorTools.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MalfunctionSymptom;

public class MalfunctionSystem : MonoBehaviour
{
    [SerializeField] private LightBoard lightBoard;

    [SerializeField] private List<Malfunction> malfunctions;

    [SerializeField] private SubmarineControlSwitchboard submarineControlSwitchboard;

    private List<MalfunctionSymptom> symptoms = new List<MalfunctionSymptom>();
    private List<Malfunction> currentMalfunctions = new List<Malfunction>();

    private void Awake()
    {
        symptoms.Add(new EngineCutOff(SymptomMask.EngineCutOff));
        symptoms.Add(new LockThrottle(SymptomMask.LockThrottle));
        symptoms.Add(new LockSteering(SymptomMask.LockSteering));
        symptoms.Add(new LockPitch(SymptomMask.LockPitch));
        symptoms.Add(new LockElevation(SymptomMask.LockElevation));
    }


    public void Collision(Collision collision)
    {

    }

    private void Start()
    {
        foreach (var item in malfunctions)
        {
            Failure(item);
        }
        StartCoroutine(ErrorCodeLoop());
    }

    private void Failure(Malfunction malfunction)
    {
        if (!malfunction) return;

        currentMalfunctions.Add(malfunction);

        ApplySymptoms(malfunction);
    }

    private void ApplySymptoms(Malfunction malfunction)
    {
        foreach (var symptom in symptoms)
        {
            if((malfunction.Symptoms & symptom.Id) == symptom.Id)
            {
                symptom.Do(submarineControlSwitchboard);
            }
        }
    }

    private void RemoveSymptoms(Malfunction malfunction)
    {
        foreach (var symptom in symptoms)
        {
            if ((malfunction.Symptoms & symptom.Id) == symptom.Id)
            {
                symptom.Undo(submarineControlSwitchboard);
            }
        }
    }

    IEnumerator ErrorCodeLoop()
    {
        while (true)
        {
            lightBoard.SetLights(currentMalfunctions[0].ErrorCode);
            currentMalfunctions.Add(currentMalfunctions[0]);
            currentMalfunctions.RemoveAt(0);

            yield return new WaitForSeconds(2f);
            
            lightBoard.SetLights(Malfunction.ErrorMask.None);
            
            yield return new WaitForSeconds(1f);
        }
    }
}
