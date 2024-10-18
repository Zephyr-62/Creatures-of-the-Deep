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

    private void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(collisionSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform);
        

        instance.start();

    }

    private void Update()
    {
        if(instance.isValid() && instance.getPlaybackState(out var state) == RESULT.OK && state == PLAYBACK_STATE.STOPPED)
        {
            instance.release();
            StartCoroutine(DelayedDeletion());
        }
    }

    IEnumerator DelayedDeletion()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
