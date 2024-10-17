using FMOD;
using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAudio : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference collisionSound;
    private FMOD.Studio.EventInstance instance;

    private bool done;

    private void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(collisionSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform);
        
        var callback = new FMOD.Studio.EVENT_CALLBACK(CallBack);
        instance.setCallback(callback, EVENT_CALLBACK_TYPE.STOPPED);

        instance.start();

    }

    private void Update()
    {
        if (done)
        {
            Destroy(gameObject);
        }
    }

    private RESULT CallBack(EVENT_CALLBACK_TYPE type, IntPtr _event, IntPtr parameters)
    {
        done = true;
        return RESULT.OK;
    }
}
