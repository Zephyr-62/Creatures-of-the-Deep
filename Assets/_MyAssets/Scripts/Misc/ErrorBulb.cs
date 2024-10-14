using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Malfunction;

public class ErrorBulb : ElectricalDevice
{
    private static List<ErrorBulb> all = new List<ErrorBulb>();

    [SerializeField] private Renderer renderer;
    [SerializeField] private Light light;
    [SerializeField] private float intensity;
    [SerializeField] private TMP_Text label;
    [SerializeField] private ErrorMask errorMask;

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
        if (!isPowered) return;
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

    public void SetLabel(string label)
    {
        if (!this.label) return;
        this.label.text = label;
    }

    public void Set(ErrorMask mask)
    {
        if (errorMask == ErrorMask.None) return;
        Set((mask & errorMask) == errorMask);
    }

    public static void SetAll(ErrorMask mask)
    {
        foreach (var light in all)
        {
            light.Set(mask);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        all.Add(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        all.Remove(this);
    }

    private void OnValidate()
    {
        SetLabel(Enum.GetName(typeof(ErrorMask), errorMask));
    }

    protected override void OnPowerGained()
    {
        Set(state);
    }

    protected override void OnPowerLost()
    {
        Off();
    }

    protected override void OnSurge()
    {
        
    }
}
