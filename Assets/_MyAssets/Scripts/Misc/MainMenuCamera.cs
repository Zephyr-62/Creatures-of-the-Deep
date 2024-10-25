using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform middle;
    [SerializeField] Transform end;
    [SerializeField] CanvasGroup blackout;
    [SerializeField] CanvasGroup label;
    [SerializeField] private int levelScene;

    private IDisposable m_EventListener;


    private void OnEnable()
    {
        transform.position = start.position;
        label.alpha = 0;
        transform.DOMove(middle.position, 2).onComplete += () =>
        {
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
        label.DOKill();
        transform.DOKill();
        blackout.DOKill();

        label.DOFade(0f, .5f);
        transform.DOMove(end.position, 3f).SetEase(Ease.InSine);
        blackout.DOFade(1f, 3f).onComplete += () => SceneManager.LoadSceneAsync(levelScene);
    }
}
