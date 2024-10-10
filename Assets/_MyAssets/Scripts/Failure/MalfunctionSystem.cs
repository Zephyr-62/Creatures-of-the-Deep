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

    private List<Malfunction> allMalfunctions;

    private void Awake()
    {
        allMalfunctions = new List<Malfunction>();
        RegisterMalfunction(engineFailure);
        RegisterMalfunction(throttleHydraulicFailure);
        RegisterMalfunction(steeringHydraulicFailure);
        RegisterMalfunction(pitchHydraulicFailure);
        RegisterMalfunction(elevationHydraulicFailure);
        RegisterMalfunction(sonarVoltageSurge);
        RegisterMalfunction(screenVoltageSurge);
        RegisterMalfunction(lightsVoltageSurge);

    }

    private void Start()
    {
        StartCoroutine(ErrorCodeLoop());
    }

    private void Update()
    {
        foreach (var malfunction in allMalfunctions)
        {
            malfunction.Update();
            if (malfunction.Enabled && malfunction.IsFixed())
            {
                malfunction.Exit();
            }
        }
    }

    public void Failure(Malfunction malfunction)
    {
        if (malfunction == null) return;
        if (malfunction.Enabled) return;
        malfunction.AttachSystem(this);
        malfunction.Enter();
    }

    

    IEnumerator ErrorCodeLoop()
    {
        var index = 0;

        while (true)
        {
            for (var i = 0; i < allMalfunctions.Count; i++)
            {
                index++;
                if(index >= allMalfunctions.Count)
                {
                    index = 0;
                }
                if (allMalfunctions[index].Enabled)
                {
                    Debug.Log("Enabled!");
                    break;
                }
            }

            if (allMalfunctions[index].Enabled)
            {
                ErrorBulb.SetAll(allMalfunctions[index].ErrorCode);

                yield return new WaitForSeconds(2f);

                ErrorBulb.SetAll(Malfunction.ErrorMask.None);

                yield return new WaitForSeconds(.5f);
            } else
            {
                yield return null;
            }
        }
    }

    private void RegisterMalfunction(Malfunction malfunction)
    {
        malfunction.AttachSystem(this);
        allMalfunctions.Add(malfunction);
    }

    [Button("Test")]
    public void Test()
    {
        Failure(screenVoltageSurge);
    }
}
