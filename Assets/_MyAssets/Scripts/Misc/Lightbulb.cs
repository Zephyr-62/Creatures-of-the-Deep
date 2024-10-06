using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightbulb : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private Light light;
    [SerializeField] private float intensity;

    private string COLOR_KEYWORD = "_Intensity";
    private bool state;

    [Button("Toggle On/Off")]
    public void Toggle()
    {
        if(state)
        {
            Off();
        } else
        {
            On();
        }
        state = !state;
    }

    public void Set(bool state)
    {
        if (state)
        {
            On();
        }
        else
        {
            Off();
        }
        this.state = state;
    }

    public void On()
    {
        renderer.material.DOKill();
        renderer.material.DOFloat(1f, COLOR_KEYWORD, 0.1f);
        if (light)
        {
            light.DOKill();
            light.DOIntensity(intensity, 0.1f);
        }
    }

    public void Off()
    {
        renderer.material.DOKill();
        renderer.material.DOFloat(0f, COLOR_KEYWORD, 0.5f).SetEase(Ease.OutCubic);
        if (light)
        {
            light.DOKill();
            light.DOIntensity(0f, 0.1f);
        }
    }
}
