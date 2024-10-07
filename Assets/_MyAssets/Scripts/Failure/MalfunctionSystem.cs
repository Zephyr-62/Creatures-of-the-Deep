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
    [SerializeField] private SubmarineUtilitySwitchboard submarineUtilitySwitchboard;

    public SubmarineControlSwitchboard controls => submarineControlSwitchboard;
    public SubmarineUtilitySwitchboard utilities => submarineUtilitySwitchboard;

    private Dictionary<SymptomMask, Symptom> symptoms;
    private List<Malfunction> currentMalfunctions;

    [Header("Malfunctions")]
    [SerializeField] private EngineFailure engineFailure;
    [SerializeField] private HydrolicFailure hydrolicFailure;

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

    private void Start()
    {
        StartCoroutine(ErrorCodeLoop());
    }

    private void Update()
    {
        foreach (var malfunction in currentMalfunctions)
        {
            if (malfunction.IsFixed(this))
            {
                RemoveSymptoms(malfunction);
                currentMalfunctions.Remove(malfunction);
                malfunction.Exit(this);
            }
        }
    }

    private void Failure(Malfunction malfunction)
    {
        if (malfunction == null) return;
        currentMalfunctions.Add(malfunction);
        malfunction.Enter(this);
        ApplySymptoms(malfunction);
    }

    private void ApplySymptoms(Malfunction malfunction)
    {
        foreach (var symptom in symptoms)
        {
            if((malfunction.Symptoms & symptom.Key) == symptom.Key)
            {
                symptom.Value.Do(this);
            }
        }
    }

    private void RemoveSymptoms(Malfunction malfunction)
    {
        foreach (var symptom in symptoms)
        {
            if ((malfunction.Symptoms & symptom.Key) == symptom.Key)
            {
                symptom.Value.Undo(this);
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
        Failure(engineFailure);
    }
}
