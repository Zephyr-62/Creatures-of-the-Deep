using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightbulb : ElectricalDevice
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private Light light;
    [SerializeField] private ClickySwitch clickySwitch;

    private string COLOR_KEYWORD = "_Intensity";
    private string SURGE_KEYWORD = "_Interference";

    private float intensity;
    private float initialIntensity;
    private Tween tween;

    private void Start()
    {
        if (light) initialIntensity = light.intensity;
        intensity = initialIntensity;

        if (clickySwitch)
        {
            clickySwitch.onValueChanged.AddListener(Power);
            Power();
        }
    }

    public void TurnOn()
    {
        if (!isPowered) return;

        if (renderer)
        {
            renderer.material.DOKill();
            renderer.material.DOFloat(1f, COLOR_KEYWORD, 0.1f);
        }
        
        if (light)
        {
            if (tween != null) tween.Kill();
            tween = DOTween.To(() => intensity, x => intensity = x, initialIntensity, 0.1f);
        }
    }

    public void TurnOff()
    {

        if (renderer)
        {
            renderer.material.DOKill();
            renderer.material.DOFloat(0f, COLOR_KEYWORD, 0.5f).SetEase(Ease.OutCubic);
        }
        
        if (light)
        {

            if (tween != null) tween.Kill();
            tween = DOTween.To(() => intensity, x => intensity = x, 0f, 0.1f);
        }
    }

    private void Power()
    {
        if ((!clickySwitch || clickySwitch.GetBoolValue()) && isPowered)
        {
            TurnOn();
        } else
        {
            TurnOff();
        }
    }

    protected override void OnPowerGained()
    {
        Power();
    }

    protected override void OnPowerLost()
    {
        Power();
    }

    private void Update()
    {
        if (light) light.intensity = intensity * Mathf.Clamp01(1 - Mathf.PerlinNoise1D(Time.time) * surge);
    }

    protected override void OnSurge()
    {
        if (renderer) renderer.material.SetFloat(SURGE_KEYWORD, surge);
    }
}
