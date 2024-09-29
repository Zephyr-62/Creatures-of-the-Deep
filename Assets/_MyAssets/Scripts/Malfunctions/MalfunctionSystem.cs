using AdvancedEditorTools.Attributes;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Symptom;

public class MalfunctionSystem : MonoBehaviour
{
    [SerializeField] private LightBoard lightBoard;

    [SerializeField] private List<Malfunction> malfunctions;

    [SerializeField] private SubmarineControlSwitchboard submarineControlSwitchboard;

    private Dictionary<SymptomMask, Symptom> symptoms;
    private List<Malfunction> currentMalfunctions;

    private void Awake()
    {
        currentMalfunctions = new List<Malfunction>();
        symptoms = new Dictionary<SymptomMask, Symptom>
        {
            { SymptomMask.EngineCutOff,     new EngineCutOff()  },
            { SymptomMask.LockThrottle,     new LockThrottle()  },
            { SymptomMask.LockSteering,     new LockSteering()  },
            { SymptomMask.LockPitch,        new LockPitch()     },
            { SymptomMask.LockElevation,    new LockElevation() }
        };
    }

    public void Collision(Collision collision)
    {

    }

    private void Start()
    {
        StartCoroutine(ErrorCodeLoop());
    }

    private void Failure(Malfunction malfunction)
    {
        if (!malfunction) return;
        if (currentMalfunctions.Contains(malfunction)) return;

        currentMalfunctions.Add(malfunction);

        ApplySymptoms(malfunction);
    }

    private void ApplySymptoms(Malfunction malfunction)
    {
        foreach (var symptom in symptoms)
        {
            if((malfunction.Symptoms & symptom.Key) == symptom.Key)
            {
                symptom.Value.Do(submarineControlSwitchboard);
            }
        }
    }

    private void RemoveSymptoms(Malfunction malfunction)
    {
        foreach (var symptom in symptoms)
        {
            if ((malfunction.Symptoms & symptom.Key) == symptom.Key)
            {
                symptom.Value.Undo(submarineControlSwitchboard);
            }
        }
    }

    IEnumerator ErrorCodeLoop()
    {
        while (true)
        {
            yield return new WaitUntil(() => currentMalfunctions.Count > 0);

            lightBoard.SetLights(currentMalfunctions[0].ErrorCode);
            currentMalfunctions.Add(currentMalfunctions[0]);
            currentMalfunctions.RemoveAt(0);

            yield return new WaitForSeconds(2f);
            
            lightBoard.SetLights(Malfunction.ErrorMask.None);
            
            yield return new WaitForSeconds(1f);
        }
    }

    [Button("Test")]
    public void Test()
    {
        Failure(malfunctions[0]);
    }
}
