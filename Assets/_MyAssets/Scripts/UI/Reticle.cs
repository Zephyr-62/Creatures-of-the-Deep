using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    private Image reticle;
    private CanvasGroup primaryCanvasGroup;

    [SerializeField] private CanvasGroup grabCanvasGroup;
    [SerializeField] private Image GrabRight;
    [SerializeField] private Image GrabLeft;

    [SerializeField] private Vector3 defaultOffset;
    [SerializeField] private Vector3 grabbedOffset;

    [SerializeField] private float moveSpeed = 0.3f;
    [SerializeField] private float fadeSpeed = 0.5f;

    [SerializeField] private Ease moveEase = Ease.Linear;
    [SerializeField] private Ease fadeEase = Ease.Linear;

    private bool grabHidden;
    private bool grabMoved;

    private void OnValidate()
    {
        if (GrabRight) GrabRight.rectTransform.localPosition = defaultOffset;
        if (GrabLeft) GrabLeft.rectTransform.localPosition = -defaultOffset;
    }

    private void Awake()
    {
        reticle = GetComponent<Image>();
        primaryCanvasGroup = GetComponent<CanvasGroup>();
        Set(Mode.Normal);
    }

    public enum Mode
    {
        Normal,
        Hover,
        Grabbed
    }

    public void Set(Mode mode)
    {
        switch (mode)
        {
            case Mode.Normal:
                HideGrabSprites(true);
                MoveGrabSprites(false);
                break;
            case Mode.Hover:
                HideGrabSprites(false);
                MoveGrabSprites(false);
                break;
            case Mode.Grabbed:
                HideGrabSprites(false);
                MoveGrabSprites(true);
                break;
            default:
                break;
        }
    }

    private void HideGrabSprites(bool hide)
    {
        if(grabHidden != hide)
        {
            grabCanvasGroup.DOKill();
            grabCanvasGroup.DOFade(hide ? 0f : 1f, fadeSpeed).SetEase(fadeEase);
            grabHidden = hide;
        }
    }

    private void MoveGrabSprites(bool move)
    {
        if(move)
        {
            GrabRight.rectTransform.DOLocalMove(grabbedOffset, moveSpeed).SetEase(moveEase);
            GrabLeft.rectTransform.DOLocalMove(-grabbedOffset, moveSpeed).SetEase(moveEase);
        } else
        {
            GrabRight.rectTransform.DOLocalMove(defaultOffset, moveSpeed).SetEase(moveEase);
            GrabLeft.rectTransform.DOLocalMove(-defaultOffset, moveSpeed).SetEase(moveEase);
        }
    }
}
