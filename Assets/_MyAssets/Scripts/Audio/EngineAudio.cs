using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Profiling.RawFrameDataView;
using static UnityEngine.Rendering.DebugUI;

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
        instance.start();
    }

    private void StopEngine()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
