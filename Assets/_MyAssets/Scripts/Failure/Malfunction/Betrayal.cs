using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[Serializable]
public class Betrayal : Malfunction
{
    [SerializeField] private float controlDelay = 2f;
    [SerializeField] private float engineDelay = 5f;
    [SerializeField] private float surgeDelay = 8f;
    [SerializeField] private float breakerDelay = 1f;
    [SerializeField] private UnityEngine.Transform weightPoint;

    [SerializeField] private FMODUnity.EventReference audio;

    private FMOD.Studio.EventInstance instance;
    private float time;

    private bool control, engine, surge, breaker;

    public override bool IsFixed()
    {
        return false;
    }

    public override void Enter(MalfunctionTrigger trigger = null)
    {
        base.Enter(trigger);

        instance = FMODUnity.RuntimeManager.CreateInstance(audio);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, system.transform);
        instance.start();

        
        time = Time.time;

        
    }

    public override void Update()
    {
        base.Update();
        
        if (Enabled)
        {
            if (!control && time + controlDelay <= Time.time)
            {
                system.physicsSystem.elevationControl.Block();
                system.physicsSystem.pitchControl.Block();

                system.physicsSystem.throttleControl.Block();
                system.physicsSystem.steeringControl.Block();
                control = true;
            }

            if (!engine && time + engineDelay <= Time.time)
            {
                system.physicsSystem.TurnOff();
                engine = true;
            }

            if (!surge && time + surgeDelay <= Time.time)
            {
                ElectricalDevice.SurgeAll(3f);
                surge = true;
            }

            if (!breaker && time + breakerDelay <= Time.time)
            {
                system.breaker.Break();
                breaker = true;
            }

            errorCode = (ErrorMask)UnityEngine.Random.Range(1, 65536);

            system.physicsSystem.RB.AddForceAtPosition(Physics.gravity * system.physicsSystem.RB.mass * 0.03f, weightPoint.position);
        }
    }
}
