using AdvancedEditorTools.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineSwitchboard : MonoBehaviour
{

    [BeginColumnArea(areaStyle = LayoutStyle.BoxRound)]
    [Header("Core systems")]
    [SerializeField] private SubmarinePhysicsSystem physicsSystem;
    [EndColumnArea]

    [BeginColumnArea(areaStyle = LayoutStyle.BoxRound)]
    [Header("Submarine piloting PCS")]
    [SerializeField] private PhysicalControlSurface throttle;
    [SerializeField] private PhysicalControlSurface steering;
    [SerializeField] private PhysicalControlSurface pitch;
    [SerializeField] private PhysicalControlSurface vertical;
    [EndColumnArea]

    [SerializeField] private float test;

    private void OnEnable()
    {
        if (throttle) throttle.onValueChanged.AddListener(SetThrottle);
        if (steering) steering.onValueChanged.AddListener(SetSteering);
        if (pitch) pitch.onValueChanged.AddListener(SetPitch);
        if (vertical) vertical.onValueChanged.AddListener(SetVertical);
    }

    private void OnDisable()
    {
        if (throttle) throttle.onValueChanged.RemoveListener(SetThrottle);
        if (steering) steering.onValueChanged.RemoveListener(SetSteering);
        if (pitch) pitch.onValueChanged.RemoveListener(SetPitch);
        if (vertical) vertical.onValueChanged.RemoveListener(SetVertical);
    }

    private void SetThrottle()
    {
        physicsSystem.ThrustInput = throttle.GetFloatValue();
    }

    private void SetSteering()
    {
        physicsSystem.SteerInput = steering.GetFloatValue();
    }

    private void SetPitch()
    {
        physicsSystem.PitchInput = pitch.GetFloatValue();
    }

    private void SetVertical()
    {
        physicsSystem.BuoyancyInput = vertical.GetFloatValue();
    }
}
