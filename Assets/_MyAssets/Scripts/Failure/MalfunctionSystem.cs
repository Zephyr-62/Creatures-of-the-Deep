using AdvancedEditorTools.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalfunctionSystem : MonoBehaviour
{
    [SerializeField] private SubmarineUtilitySwitchboard _utilities;
    [SerializeField] private SubmarinePhysicsSystem _engine;

    [Header("Malfunctions")]
    [SerializeField] private EngineFailure engineFailure;
    [SerializeField] private MissfireFailure missfireFailure;
    [SerializeField] private HydraulicFailure throttleHydraulicFailure;
    [SerializeField] private HydraulicFailure steeringHydraulicFailure;
    [SerializeField] private HydraulicFailure pitchHydraulicFailure;
    [SerializeField] private HydraulicFailure elevationHydraulicFailure;
    [SerializeField] private LocalVoltageSurge sonarVoltageSurge;
    [SerializeField] private LocalVoltageSurge screenVoltageSurge;
    [SerializeField] private LocalVoltageSurge lightsVoltageSurge;

    public SubmarineUtilitySwitchboard utilities => _utilities;
    public SubmarinePhysicsSystem engine => _engine;

    private List<Malfunction> currentMalfunctions;

    private void Awake()
    {
        currentMalfunctions = new List<Malfunction>();
        engineFailure.AttachSystem(this);
        throttleHydraulicFailure.AttachSystem(this);
        steeringHydraulicFailure.AttachSystem(this);
        pitchHydraulicFailure.AttachSystem(this);
        elevationHydraulicFailure.AttachSystem(this);
        sonarVoltageSurge.AttachSystem(this);
    }

    private void Start()
    {
        StartCoroutine(ErrorCodeLoop());
    }

    private void Update()
    {
        foreach (var malfunction in currentMalfunctions)
        {
            malfunction.Update();
            if (malfunction.IsFixed())
            {
                RemoveSymptoms(malfunction);
                currentMalfunctions.Remove(malfunction);
                malfunction.Exit();
            }
        }
    }

    public void Failure(Malfunction malfunction)
    {
        if (malfunction == null) return;
        malfunction.AttachSystem(this);
        currentMalfunctions.Add(malfunction);
        malfunction.Enter();
        ApplySymptoms(malfunction);
    }

    private void ApplySymptoms(Malfunction malfunction)
    {
        //foreach (var symptom in symptoms)
        //{
        //    if((malfunction.Symptoms & symptom.Key) == symptom.Key)
        //    {
        //        symptom.Value.Do(this);
        //    }
        //}
    }

    private void RemoveSymptoms(Malfunction malfunction)
    {
        //foreach (var symptom in symptoms)
        //{
        //    if ((malfunction.Symptoms & symptom.Key) == symptom.Key)
        //    {
        //        symptom.Value.Undo(this);
        //    }
        //}
    }

    IEnumerator ErrorCodeLoop()
    {
        while (true)
        {
            yield return new WaitUntil(() => currentMalfunctions.Count > 0);

            Lightbulb.SetAll(currentMalfunctions[0].ErrorCode);

            currentMalfunctions.Add(currentMalfunctions[0]);
            currentMalfunctions.RemoveAt(0);

            yield return new WaitForSeconds(2f);

            Lightbulb.SetAll(Malfunction.ErrorMask.None);

            yield return new WaitForSeconds(1f);
        }
    }

    [Button("Test")]
    public void Test()
    {
        Failure(screenVoltageSurge);
    }
}
