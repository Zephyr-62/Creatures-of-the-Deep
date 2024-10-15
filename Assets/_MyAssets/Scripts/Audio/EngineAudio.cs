using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    [SerializeField] private SubmarinePhysicsSystem physicsSystem;
    [SerializeField] private FMODUnity.EventReference baseEngine;
    [SerializeField] private string parameter;

    private FMOD.Studio.EventInstance instance;

    void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(baseEngine);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform);
    }

    private void OnEnable()
    {
        physicsSystem.onStartEngine.AddListener(StartEngine);
        physicsSystem.onStopEngine.AddListener(StopEngine);
    }

    private void OnDisable()
    {
        physicsSystem.onStartEngine.RemoveListener(StartEngine);
        physicsSystem.onStopEngine.RemoveListener(StopEngine);
    }

    private void Update()
    {
        instance.setParameterByName(parameter, Mathf.Abs(physicsSystem.thrust));
    }

    private void StartEngine()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform);
        instance.start();
    }

    private void StopEngine()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
