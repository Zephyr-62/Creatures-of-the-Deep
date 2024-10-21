using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSink : MonoBehaviour
{
    [SerializeField] private Engine engine;
    [SerializeField] private Light heatLight;
    [SerializeField] private Renderer heatRenderer;
    [SerializeField] private float threshold;

    private void Update()
    {
        heatLight.intensity = Mathf.Max((engine.heat / engine.heatCapacity) - threshold, 0f) / (1-threshold);
        heatRenderer.material.SetFloat("_Threshold", threshold);
        heatRenderer.material.SetFloat("_Heat", engine.heat / engine.heatCapacity);
    }
}
