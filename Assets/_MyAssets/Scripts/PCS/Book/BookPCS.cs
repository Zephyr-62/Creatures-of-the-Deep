
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
    public bool reversePageIfNotMidway = true;

    [Header("Values")]
    [FormerlySerializedAs("value"), SerializeField] private float _value;
    [Header("Moving parts")]
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float minAngle, maxAngle;
    [SerializeField] private float range = 1f;
    //[SerializeField] private float animationDuration = 0.1f;
    //[SerializeField] private Ease animationEase = Ease.Linear;
    //[SerializeField] private float switchAngle = 0f;

    [Header("Extra events")]
    [SerializeField] private UnityEvent onSwitchedOn;
    [SerializeField] private UnityEvent onSwitchedOff;

    
    private Vector3 point;
    private Vector3 dir;
    private float clampedAngle;

    internal override void Release(bool fireEvent = true)
    {
        base.Release(fireEvent);
        if (book.IsDraggingPage && !book.IsTurningPages)
        {
            book.TurnPageDragStop(1, PageTurnCompleted, reverse: reversePageIfNotMidway ? (book.TurnPageDragNormalizedTime < 0.5f) : false);
        }
    }

    public override void HandleInput()
    {
        if (book.IsTurningPages)
        {
            // exit if already turning
            return;
        }

        var plane = new Plane(transform.forward, grabPoint);
        var ray = FirstPersonCamera.GetRay();

        if (plane.Raycast(ray, out var e))
        {
            point = ray.GetPoint(e);
            dir = point - rotatePoint.position;

            var angle = Vector3.SignedAngle(Vector3.up, transform.InverseTransformDirection(dir), Vector3.forward);
            if (angle < minAngle)
            {
                var adjustAngle = angle + minAngle;
                angle = minAngle + adjustAngle;
            }
            else if (angle > maxAngle)
            {
                var adjustAngle = angle - maxAngle;
                angle = maxAngle - adjustAngle;
            }
            if(dir.magnitude > range)
            {
                FirstPersonCamera.ForceRelease();
                return;
            }
            
            clampedAngle = Mathf.Clamp(angle, minAngle, maxAngle);

            var turningTime = Mathf.InverseLerp(minAngle, maxAngle, clampedAngle);
            if (book.CurrentState == EndlessBook.StateEnum.OpenMiddle && book.IsDraggingPage)
            {
                book.TurnPageDrag(1 - turningTime);
                
            }

        }
    }

    internal override void Grab(FirstPersonCamera firstPersonCamera, Vector3 grabPoint, bool fireEvent = true)
    {
        if (blocked) return;
        if (book.IsTurningPages || book.IsDraggingPage)
        {
            // exit if already turning
            return;
        }
        base.Grab(firstPersonCamera, grabPoint, fireEvent);
        var plane = new Plane(transform.forward, grabPoint);
        var ray = FirstPersonCamera.GetRay();

        if (plane.Raycast(ray, out var e))
        {
            point = ray.GetPoint(e);
            dir = point - rotatePoint.position;
            var angle = Vector3.SignedAngle(Vector3.up, transform.InverseTransformDirection(dir), Vector3.forward);
            if (angle < 0)
            {
                if(book.CurrentState != EndlessBook.StateEnum.OpenBack)
                    book.TurnPageDragStart(Page.TurnDirectionEnum.TurnForward);
            }
            else
            {
                if(book.CurrentState != EndlessBook.StateEnum.OpenFront)
                    book.TurnPageDragStart(Page.TurnDirectionEnum.TurnBackward);
            }
        }
    }
    
    
    public override float GetFloatValue()
    {
        return 0;
    }

    public override bool GetBoolValue()
    {
        return false;
    }

    public override int GetIntValue()
    {
        return 0;
    }

    public override void SetFloatValue(float value)
    {
    }

    public override void SetBoolValue(bool value)
    {
    }

    public override void SetIntValue(int value)
    {
    }
    private void OnDrawGizmos()
    {
        if (!grabbed) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(point, 0.05f);
        Gizmos.DrawRay(rotatePoint.position, dir);
#if UNITY_EDITOR
        Handles.color = Color.blue;
        Handles.DrawWireDisc(grabPoint, transform.forward, range);
        Handles.Label(transform.position, clampedAngle.ToString());
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
        return 0;
    }

    public override void Set01FloatValue(float value)
    {
    }

    public override void Block()
    {
        base.Block();
        this.GetComponent<Collider>().enabled = false;
    }

    public override void Unblock()
    {
        base.Unblock();
        this.GetComponent<Collider>().enabled = true;
    }
}