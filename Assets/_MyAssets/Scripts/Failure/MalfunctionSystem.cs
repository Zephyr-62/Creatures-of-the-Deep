using AdvancedEditorTools.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalfunctionSystem : MonoBehaviour
{
    [SerializeField] private Engine _engine;
    [SerializeField] private HydraulicPump _pump;
    [SerializeField] private CircuitBreaker _breaker;

    [SerializeField] private SubmarinePhysicsSystem _physicsSystem;

    [Header("Malfunctions")]
    public EngineFailure engineFailure;
    public OverheatingFailure missfireFailure;
    public HydraulicFailure throttleHydraulicFailure;
    public HydraulicFailure steeringHydraulicFailure;
    public HydraulicFailure pitchHydraulicFailure;
    public HydraulicFailure elevationHydraulicFailure;
    public LocalVoltageSurge sonarVoltageSurge;
    public LocalVoltageSurge screenVoltageSurge;
    public LocalVoltageSurge lightsVoltageSurge;
    public CriticalVoltageSurge criticalVoltageSurge;



    [SerializeField] private FMODUnity.EventReference alert;

    public Engine engine => _engine;
    public HydraulicPump pump => _pump;
    public CircuitBreaker breaker => _breaker;
    public SubmarinePhysicsSystem physicsSystem => _physicsSystem;

    private List<Malfunction> allMalfunctions;
    private FMOD.Studio.EventInstance instance;

    private void Awake()
    {
        allMalfunctions = new List<Malfunction>();
        RegisterMalfunction(engineFailure);
        RegisterMalfunction(missfireFailure);
        RegisterMalfunction(throttleHydraulicFailure);
        RegisterMalfunction(steeringHydraulicFailure);
        RegisterMalfunction(pitchHydraulicFailure);
        RegisterMalfunction(elevationHydraulicFailure);
        RegisterMalfunction(sonarVoltageSurge);
        RegisterMalfunction(screenVoltageSurge);
        RegisterMalfunction(lightsVoltageSurge);
        RegisterMalfunction(criticalVoltageSurge);

        throttleHydraulicFailure.affectedControl = physicsSystem.throttleControl;
        steeringHydraulicFailure.affectedControl = physicsSystem.steeringControl;
        pitchHydraulicFailure.affectedControl = physicsSystem.pitchControl;
        elevationHydraulicFailure.affectedControl = physicsSystem.elevationControl;

        instance = FMODUnity.RuntimeManager.CreateInstance(alert);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform);
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
                    break;
                }
            }

            if (allMalfunctions[index].Enabled)
            {
                ErrorBulb.SetAll(allMalfunctions[index].ErrorCode);
                
                
                if((allMalfunctions[index].Symptoms & Symptom.SymptomMask.Alert) == Symptom.SymptomMask.Alert)
                {
                    instance.start();
                    FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform);
                }

                yield return new WaitForSeconds(1f);

                ErrorBulb.SetAll(Malfunction.ErrorMask.None);

                yield return new WaitForSeconds(.3f);
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

    public void Collision(Collision collision)
    {
        Rigidbody rb1 = physicsSystem.GetComponentInParent<Rigidbody>();
        Rigidbody rb2 = collision.rigidbody;

        float mass1 = rb1.mass;
        float mass2 = rb2 != null ? rb2.mass : 1000f;

        Vector3 relativeVelocity = collision.relativeVelocity;

        float reducedMass = (2 * mass1 * mass2) / (mass1 + mass2);
        float collisionForce = reducedMass * relativeVelocity.magnitude;

        foreach (var malfunction in allMalfunctions)
        {
            malfunction.OnCollision(collision, collisionForce);
        }
    }
}
