using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightbulb : ElectricalDevice
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private Light light;
    
    private string COLOR_KEYWORD = "_Intensity";
    private string SURGE_KEYWORD = "_Interference";

    private bool state;
    private float intensity;
    private float initialIntensity;
    private Tween tween;

    private void Start()
    {
        if(light) initialIntensity = light.intensity;
        intensity = initialIntensity;
    }

    public void TurnOn()
    {
        if (!isPowered) return;
        this.state = true;
        renderer.material.DOKill();
        renderer.material.DOFloat(1f, COLOR_KEYWORD, 0.1f);
        if (light)
        {
            if (tween != null) tween.Kill();
            tween = DOTween.To(() => intensity, x => intensity = x, initialIntensity, 0.1f);
            //light.DOKill();
            //light.DOIntensity(intensity, 0.1f);
        }
    }

    public void TurnOff()
    {
        this.state = false;
        renderer.material.DOKill();
        renderer.material.DOFloat(0f, COLOR_KEYWORD, 0.5f).SetEase(Ease.OutCubic);
        if (light)
        {

            if (tween != null) tween.Kill();
            tween = DOTween.To(() => intensity, x => intensity = x, 0f, 0.1f);
            //light.DOKill();
            //light.DOIntensity(0f, 0.1f);
        }
    }

    protected override void OnPowerGained()
    {
        TurnOn();
    }

    protected override void OnPowerLost()
    {
        TurnOff();
    }

    private void Update()
    {
        if(light) light.intensity = intensity * Mathf.Clamp01(1 - Mathf.PerlinNoise1D(Time.time) * surge);
    }

    protected override void OnSurge()
    {
        renderer.material.SetFloat(SURGE_KEYWORD, surge);
    }
}
