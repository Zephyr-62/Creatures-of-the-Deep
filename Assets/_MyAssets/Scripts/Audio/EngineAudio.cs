using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    [SerializeField] private Engine engine;
    [SerializeField] private FMODUnity.EventReference baseEngine;
    [SerializeField] private string engineThrustParameter = "engine_level";
    [SerializeField] private string engineElevationParameter = "elevation_level";

    [SerializeField] private FMODUnity.EventReference heat;
    [SerializeField] private string heatParameter = "heat_level";

    private FMOD.Studio.EventInstance engineInstance;
    private FMOD.Studio.EventInstance heatInstance;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        engine.system.onStartEngine.AddListener(StartEngine);
        engine.system.onStopEngine.AddListener(StopEngine);

        engine.onOverheat.AddListener(Overheat);
    }

    private void OnDisable()
    {
        engine.system.onStartEngine.RemoveListener(StartEngine);
        engine.system.onStopEngine.RemoveListener(StopEngine);
        
        engine.onOverheat.RemoveListener(Overheat);
    }

    private void Update()
    {
        if (engineInstance.isValid())
        {
            engineInstance.setParameterByName(engineThrustParameter, Mathf.Abs(engine.system.thrust));
            engineInstance.setParameterByName(engineElevationParameter, Mathf.Abs(engine.system.elevation));
        }
        if (heatInstance.isValid())
        {
            heatInstance.setParameterByName(heatParameter, engine.heat / engine.heatCapacity);
        }
    }

    private void StartEngine()
    {
        engineInstance = FMODUnity.RuntimeManager.CreateInstance(baseEngine);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(engineInstance, transform);
        engineInstance.start();
    }

    private void StopEngine()
    {
        engineInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        engineInstance.release();
    }

    private void Overheat()
    {
        heatInstance = FMODUnity.RuntimeManager.CreateInstance(heat);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(heatInstance, transform);
        heatInstance.start();
        heatInstance.release();
    }
}
