using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Engine : ElectricalDevice
{
    [SerializeField] private SubmarinePhysicsSystem _system;
    [Header("Controls")]
    [SerializeField] private ClickySwitch _power;
    [SerializeField] private ClickySwitch _ignition;
    [SerializeField] private Pulley _starter;
    [Header("Settings")]
    [SerializeField] private float _minimumStartValue = 0.9f;
    [SerializeField] private float _minimumStartVelocity = 10f;
    [SerializeField] private float _heatCapacity;
    [SerializeField] private float _coolRate = 0.1f;
    [Header("Events")]
    [SerializeField] private UnityEvent _onSuccessfullStart;
    [SerializeField] private UnityEvent _onOverheat;

    private float _heat;
    private bool _overheated;

    public SubmarinePhysicsSystem system => _system;
    public ClickySwitch power => _power;
    public ClickySwitch ignition => _ignition;
    public Pulley starter => _starter;
    public float minimumStartValue => _minimumStartValue;
    public float minimumStartVelocity => _minimumStartVelocity;
    public float heat => _heat;
    public float heatCapacity => _heatCapacity;

    public UnityEvent onSuccessfullStart => _onSuccessfullStart;
    public UnityEvent onOverheat => _onOverheat;

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
        if (!isPowered) return;
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

    private void Update()
    {
        _heat += Mathf.Abs(_system.thrust) * Time.deltaTime;
        _heat += Mathf.Abs(_system.steering) * Time.deltaTime * 0.2f;
        _heat += Mathf.Abs(_system.pitch) * Time.deltaTime * 0.2f;
        _heat += Mathf.Abs(_system.elevation) * Time.deltaTime;

        _heat -= _coolRate * Time.deltaTime * _heat * _heat;

        _heat = Mathf.Max(_heat, 0);
    }

    public override float Measure()
    {
        return _heat;
    }

    public override Vector2 GetRange()
    {
        return new Vector2(0, _heatCapacity);
    }

    public void Overheat(bool value)
    {
        _overheated = value;
        if (value)
        {
            _onOverheat.Invoke();
        }
    }
}
