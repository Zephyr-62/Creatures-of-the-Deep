using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float TargetMinSqrDistance = 400;
    public float AngleToTargetThreshold = 0.1f;
    public float WalkSpeed = 10.0f;
    public float RotationDelay = 0.25f;
    private Vector3 InitPos = Vector3.zero;
    public Transform Target;

    // Update is called once per frame
    void Update()
    {
        var TargetDir = (Target.position - this.transform.position);
        if (IsWalking && TargetDir.sqrMagnitude >= TargetMinSqrDistance)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, this.transform.position + TargetDir.normalized, Time.deltaTime * WalkSpeed);
        }

        var angleToTarget = Vector3.SignedAngle(this.transform.forward, TargetDir, Vector3.up);
        if(Mathf.Abs(angleToTarget) > AngleToTargetThreshold)
            this.transform.Rotate(Vector3.up, angleToTarget / RotationDelay);

    }

    private bool IsWalking = false;
    [Button("Set New InitPos")]
    public void SetInitPos() => InitPos = this.transform.position;
    [Button("Start Walking")]
    public void StartWalk() => IsWalking = true;
    [Button("Stop Walking")]
    public void StopWalk() => IsWalking = false;
    [Button("Reset Position")]
    public void ResetPos() => this.transform.position = InitPos;


    private void OnDrawGizmosSelected()
    {
        var TargetDir = (Target.position - this.transform.position);

        Gizmos.color = Color.yellow;
        var TargetDesiredPos = this.transform.position + TargetDir.normalized * Mathf.Sqrt(TargetMinSqrDistance);
        Gizmos.DrawLine(this.transform.position, TargetDesiredPos);
        Gizmos.DrawSphere(TargetDesiredPos, 1);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(TargetDesiredPos, Target.position);

    }


}
