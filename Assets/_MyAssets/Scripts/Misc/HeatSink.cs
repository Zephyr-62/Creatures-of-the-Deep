using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSink : MonoBehaviour
{
    [SerializeField] private Engine engine;
    [SerializeField] private Light heatLight;
    [SerializeField] private Renderer heatRenderer;

    private void Update()
    {
        heatLight.intensity = Mathf.Max((engine.heat / engine.heatCapacity) - 0.5f, 0f) * 2f;
        heatRenderer.material.SetFloat("_Heat", engine.heat / engine.heatCapacity);
    }
}
