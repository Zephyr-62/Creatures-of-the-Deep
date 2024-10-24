using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Engine : ElectricalDevice
{
    [Header("Controls")]
    [SerializeField] private ClickySwitch _power;
    [SerializeField] private ClickySwitch _ignition;
    [SerializeField] private Pulley _starter;
    [Header("Settings")]
    [SerializeField] private float _minimumStartValue = 0.9f;
    [SerializeField] private float _minimumStartVelocity = 10f;
    [Header("Events")]
    [SerializeField] private UnityEvent _onSuccessfullStart;

    public ClickySwitch power => _power;
    public ClickySwitch ignition => _ignition;
    public Pulley starter => _starter;
    public float minimumStartValue => _minimumStartValue;
    public float minimumStartVelocity => _minimumStartVelocity;
    public UnityEvent onSuccessfullStart => _onSuccessfullStart;

    protected override void OnEnable()
    {
        base.OnEnable();
        starter.onPulledToMax.AddListener(AttemptStart);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        starter.onPulledToMax.RemoveListener(AttemptStart);
    }

    private void AttemptStart()
    {
        if (isPowered) return;
        if (!ignition.GetBoolValue()) return;
        if (!power.GetBoolValue()) return;
        if (starter.GetFloatValue() >= minimumStartValue && starter.Velocity >= minimumStartVelocity)
        {
            onSuccessfullStart.Invoke();
        }
    }

    protected override void OnPowerGained()
    {
        
    }

    protected override void OnPowerLost()
    {
        
    }

    protected override void OnSurge()
    {
        
    }
}
