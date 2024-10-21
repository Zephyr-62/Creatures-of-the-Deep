
using echo17.EndlessBook;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
public class BookPCS : PhysicalControlSurface
{
    public EndlessBook book;
    public float turnspeed;

    public float turnStopSpeed;
    [Header("Values")]
    [FormerlySerializedAs("value"), SerializeField] private bool _value;
    [Header("Moving parts")]
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float minAngle, maxAngle;
    [SerializeField] private float range = 1f;
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private Ease animationEase = Ease.Linear;
    [SerializeField] private float switchAngle = 0f;

    [Header("Extra events")]
    [SerializeField] private UnityEvent onSwitchedOn;
    [SerializeField] private UnityEvent onSwitchedOff;

    private Vector3 point;
    private Vector3 dir;
    private float targetAngle;
    private float clampedAngle;
    private bool old;

    public bool value
    {
        get
        {
            return _value;
        }
        private set
        {
            if(_value != value)
            {
                old = _value;
                _value = value;
                if (old != _value)
                {
                    onValueChanged.Invoke();
                    if(_value) onSwitchedOn.Invoke();
                    else onSwitchedOff.Invoke();
                }
            }
        }
    }

    internal override void Release(bool fireEvent = true)
    {
        base.Release();
    }

    public override void HandleInput()
    {
        if (book.IsTurningPages || book.IsDraggingPage || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            // exit if already turning
            return;
        }
        var plane = new Plane(transform.up, transform.position);
        var ray = FirstPersonCamera.GetRay();

        if (plane.Raycast(ray, out var e))
        {
            point = ray.GetPoint(e);
            dir = point - rotatePoint.position;

            var angle = Vector3.SignedAngle(Vector3.right, transform.InverseTransformDirection(dir), Vector3.up);
            var dirPage = Page.TurnDirectionEnum.TurnForward;
            if (angle > 0)
            {
                dirPage = Page.TurnDirectionEnum.TurnForward;
            }
            else
            {
                dirPage = Page.TurnDirectionEnum.TurnBackward;
            }
            book.TurnPageDragStart(dirPage);
            //book.TurnPageDrag(0.1f);
            book.TurnPageDragStop(turnStopSpeed, PageTurnCompleted, false);
            //this.value = angle >= switchAngle;

            //AdjustToAngle(angle);

        }
    }
    
    private void AdjustToValue(bool value, bool skipAnimation = false)
    {
        //this.value = value;
        //AdjustToAngle(this.value ? maxAngle : minAngle, skipAnimation);
    }
    
    
    public override float GetFloatValue()
    {
        return value ? 1f : 0f;
    }

    public override bool GetBoolValue()
    {
        return value;
    }

    public override int GetIntValue()
    {
        return value ? 1 : 0;
    }

    public override void SetFloatValue(float value)
    {
        AdjustToValue(value != 0);
    }

    public override void SetBoolValue(bool value)
    {
        AdjustToValue(value);
    }

    public override void SetIntValue(int value)
    {
        AdjustToValue(value != 0);
    }
    private void OnDrawGizmos()
    {
        if (!grabbed) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(point, 0.05f);
        Gizmos.DrawRay(rotatePoint.position, dir);
#if UNITY_EDITOR
        Handles.color = Color.blue;
        Handles.DrawWireDisc(rotatePoint.position, transform.right, range);
        Handles.Label(transform.position, value.ToString());
#endif
    }
    /// <summary>
    /// Called when the page completes its manual turn
    /// </summary>
    protected virtual void PageTurnCompleted(int leftPageNumber, int rightPageNumber)
    {
        //isTurning = false;
    }

    public override float Get01FloatValue()
    {
        return value ? 1f : 0f;
    }

    public override void Set01FloatValue(float value)
    {
        AdjustToValue(_value);
    }
}