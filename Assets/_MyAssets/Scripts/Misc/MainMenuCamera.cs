using DG.Tweening;
using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform middle;
    [SerializeField] Transform end;
    [SerializeField] CanvasGroup blackout;
    [SerializeField] CanvasGroup label;
    [SerializeField] private int levelScene;
    [SerializeField] private FMODUnity.EventReference music;

    private IDisposable m_EventListener;

    EventInstance fmodInstance;
    private void OnEnable()
    {
        fmodInstance = FMODUnity.RuntimeManager.CreateInstance(music);


        transform.position = start.position;
        label.alpha = 0;
        transform.DOMove(middle.position, 2).onComplete += () =>
        {
            fmodInstance.start();
            fmodInstance.release();

            m_EventListener = InputSystem.onAnyButtonPress.CallOnce(MovePast);
            label.DOFade(1, 1f).SetEase(Ease.OutCirc);
        };
        blackout.alpha = 1f;
        blackout.DOFade(0, 1.5f);

    }

    private void OnDisable()
    {
        m_EventListener.Dispose();
    }

    private void MovePast(InputControl control)
    {
        fmodInstance.stop(STOP_MODE.ALLOWFADEOUT);
        label.DOKill();
        transform.DOKill();
        blackout.DOKill();

        label.DOFade(0f, .5f);
        transform.DOMove(end.position, 3f).SetEase(Ease.InSine);
        blackout.DOFade(1f, 3f).onComplete += () => SceneManager.LoadSceneAsync(levelScene);
    }
}
