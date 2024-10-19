using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MineDetectorSystem : MonoBehaviour
{
    [SerializeField] public Transform submarineTransform;
    [SerializeField] public SphereCollider sensorCollider;

    [SerializeField] private float detectionRange = 10;
    [SerializeField] private float lethalRange = 5;

    [SerializeField] public UnityEvent minesDetected;
    [SerializeField] public UnityEvent minesExited;
    [SerializeField] public UnityEvent minesExploded;

    private int _visibleMinesCount;

    private void OnValidate()
    {
        sensorCollider.radius = detectionRange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(submarineTransform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(submarineTransform.position, lethalRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out SeaMine mine))
        {
            if (_visibleMinesCount == 0) minesDetected.Invoke();
            _visibleMinesCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out SeaMine mine))
        {
            _visibleMinesCount--;
            if (_visibleMinesCount == 0) minesExited.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out SeaMine mine) &&
            Vector3.Distance(submarineTransform.position, mine.transform.position) <= lethalRange)
        {
            minesExploded.Invoke();
        }
    }

    public void DebugLogMinesEntered()
    {
        // Warnings should go of in the submarine
        Debug.Log("Mine field is close!");
    }

    public void DebugLogMinesExited()
    {
        // Warnings should stop
        Debug.Log("You're safe from the mines");
    }

    public void DebugLogMinesExploded()
    {
        // You die
        Debug.Log("You exploded!");
    }
}