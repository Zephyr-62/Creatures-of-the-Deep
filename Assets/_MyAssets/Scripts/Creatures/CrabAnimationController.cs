using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabAnimationController : EnemyAnimationController<CrabAIController>
{
    [SerializeField] [ReadOnly] private float TargetBodyHeight;
    [SerializeField] [ReadOnly] private float stepForwardPredictionDistance;
    [SerializeField] [ReadOnly] private Vector2 stepDurationRange;
    [SerializeField] [ReadOnly] private float targetScanAngle;
    [SerializeField] private bool ScanningTarget = false;
    [SerializeField] private bool AttackingTarget = false;

    [SerializeField] private FMODUnity.EventReference shriekSound;
    [SerializeField] private FMODUnity.EventReference stepSound;

    [Header("Body Settings")]
    [SerializeField] private float BodyHeightAdjustDelay = 0.2f;
    [SerializeField] private float StandBodyHeight = 22.5f;
    [SerializeField] private float WalkBodyHeight = 15;
    [SerializeField] private float AttackBodyHeight = 10;
    [SerializeField] private float FeetHeight = 3;
    [LineSeparator]
    [SerializeField] private float LArmHeight = -5;
    [SerializeField] private float RArmHeight = -5;
    [SerializeField] private float LArmAmplitude = 3;
    [SerializeField] private float RArmAmplitude = 3;
    [SerializeField] private float LArmFreq = 3;
    [SerializeField] private float RArmFreq = 3;
    [SerializeField] private Transform LArmTarget;
    [SerializeField] private Transform RArmTarget;
    private Vector3 RightArmDefaultTargetPos;
    [LineSeparator]
    [SerializeField] private float StepHeight = 6f;
    [SerializeField] private float PatrollingStepForwardPredictionDistance = 10f;
    [SerializeField] private float ChaseStepForwardPredictionDistance = 15f;
    [SerializeField] private Vector2 PatrollingStepDurationRange = new(3.5f, 5f);
    [SerializeField] private Vector2 ChaseStepDurationRange = new(3.5f, 5f);
    [LineSeparator]
    [Tooltip("Crab will look at +- this angle")]
    [SerializeField] private Vector2 TargetScanAngleRange = new(30, 60);
    [SerializeField] private float ScanDelay = 3.5f;
    [LineSeparator]
    [SerializeField] private List<LegTargetHint> LegTargetHints;
    [SerializeField] private LayerMask WalkableLayer;
    [SerializeField] private Ease animationEase = Ease.OutCubic;
    [LineSeparator]
    [Header("Attack settings")]
    private Vector3 ArmPosBeforeAttack;
    private Vector3 ArmPosOnAttackHit;
    [SerializeField] private Transform AttackStartPos;
    [SerializeField] private float AttackSetupTime = 1.5f;
    [SerializeField] private float AttackTime = 0.5f;
    [SerializeField] [ReadOnly] private float AttackSetupTimer = 0;
    [SerializeField] [ReadOnly] private float AttackTimer = 0;
    [SerializeField] [ReadOnly] private float AttackResetTimer = 0;

    [System.Serializable]
    protected class LegTargetHint
    {
        public Transform Leg;
        public Transform LegIKTarget;
        public Vector3 TargetHintPosition { get => Leg.TransformPoint(RelativeTargetHintPosition); }
        public Vector3 TargetHintPositionAdjusted;
        public Vector3 RelativeTargetHintPosition;   // Relative position to Leg of IK target hint
        public float MaxStepDistance;      // = 4;

        public float HintBaseHeight;    // = -3.9f;
        public Vector2 HintHeightRange; // = new (-4.25f, -2f);

        public Vector3 LastPosition; // Last valid position where the actual target should rest at
        public Sequence Sequence;
    }



    [ContextMenu("Reset Last Position")]
    private void ResetLastPos()
    {
        foreach (LegTargetHint lth in LegTargetHints)
        {
            lth.LastPosition = lth.TargetHintPosition;
            lth.TargetHintPositionAdjusted = lth.TargetHintPosition;
        }
    }
    protected override void Init()
    {
        stepForwardPredictionDistance = PatrollingStepForwardPredictionDistance;
        stepDurationRange = PatrollingStepDurationRange;
        RightArmDefaultTargetPos = RArmTarget.localPosition;
    }


    void FixedUpdate()
    {
        // Update body
        float heightAcc = 0;
        foreach (var legTargetHint in LegTargetHints)
        {
            heightAcc += legTargetHint.LegIKTarget.position.y;
        }
        this.transform.position = new (transform.position.x,
            Mathf.Lerp(transform.position.y, (heightAcc / LegTargetHints.Count) + TargetBodyHeight, Time.fixedDeltaTime / BodyHeightAdjustDelay),
                transform.position.z
            );


        // Update legs
        foreach (LegTargetHint lth in LegTargetHints)
        {
            // Calculate desired leg pos based on desired pos + moving prediction
            var newDesiredLegPos = AIController.GetMovingDir() * stepForwardPredictionDistance;
            newDesiredLegPos = new Vector3(lth.TargetHintPosition.x + newDesiredLegPos.x, this.transform.position.y, lth.TargetHintPosition.z + newDesiredLegPos.z);

            // calculate height of target hint
            if (Physics.Raycast(newDesiredLegPos, Vector3.down, out var raycastHit, 100, WalkableLayer))
            {
                lth.TargetHintPositionAdjusted = raycastHit.point + FeetHeight * Vector3.up; // TODO fix max height or smth
            }

            // If leg not currently taking a step
            if(lth.Sequence == null || !lth.Sequence.active)
            {
                // Set actual IK target to last valid feet global position
                lth.LegIKTarget.position = lth.LastPosition;

                // If target is too far from target hint, then take step (update last valid position to current hint pos)
                var zDiff = (lth.LegIKTarget.position - lth.TargetHintPositionAdjusted).sqrMagnitude;
                if (zDiff > lth.MaxStepDistance || zDiff < -lth.MaxStepDistance) // TODO prevent step if too many legs moving --> prevent movement?
                {
                    lth.LastPosition = lth.TargetHintPositionAdjusted;

                    // lth.LegIKTarget.DOKill(true);
                    lth.Sequence = lth.LegIKTarget.DOJump(lth.LastPosition, StepHeight, 1, Random.Range(stepDurationRange.x, stepDurationRange.y))
                        .SetEase(animationEase)
                        .OnComplete(() => FMODUnity.RuntimeManager.PlayOneShotAttached(stepSound, this.gameObject));
                }
            }
        }

        // Rotate body if scanning
        if (ScanningTarget)
        {
            var angleDiff = targetScanAngle - Vector3.SignedAngle(Vector3.forward, this.transform.forward, Vector3.up);
            if (Mathf.Abs(angleDiff) > 15)
                this.transform.Rotate(Vector3.up, angleDiff * Time.fixedDeltaTime / ScanDelay);
            else GetNewScanAngle();
        }

        // Lerp arms
        LArmTarget.localPosition = new Vector3(LArmTarget.localPosition.x,
            LArmHeight + Mathf.Sin(Time.time * LArmFreq) * LArmAmplitude,
            LArmTarget.localPosition.z);
        if (!AttackingTarget)
        {
            RArmTarget.localPosition = new Vector3(RArmTarget.localPosition.x,
                RArmHeight + Mathf.Sin(Time.time * RArmFreq) * RArmAmplitude,
                RArmTarget.localPosition.z);
        }
        else // Play attack animation
        {
            RArmTarget.localPosition = RightArmDefaultTargetPos;
            if(AttackSetupTimer < AttackSetupTime)
            {
                RArmTarget.position = Vector3.Lerp(ArmPosBeforeAttack, AttackStartPos.position, AttackSetupTimer / AttackSetupTime);
                AttackSetupTimer += Time.fixedDeltaTime;
            }
            else if(AttackTimer < AttackTime)
            {
                RArmTarget.position = Vector3.Lerp(AttackStartPos.position, AIController.GetChaseTarget().position, AttackTimer / AttackTime);
                AttackTimer += Time.fixedDeltaTime;
                if(AttackTimer >= AttackTime)
                {
                    ArmPosOnAttackHit = RArmTarget.localPosition;
                    AIController.HitChaseTarget();
                }
            }
            else if (AttackResetTimer < AttackSetupTime)
            {
                var destinationPos = new Vector3(RightArmDefaultTargetPos.x,
                    RArmHeight + Mathf.Sin(Time.time * RArmFreq) * RArmAmplitude,
                    RightArmDefaultTargetPos.z);

                RArmTarget.localPosition = Vector3.Lerp(ArmPosOnAttackHit, destinationPos, AttackResetTimer / AttackSetupTime);
                AttackResetTimer += Time.fixedDeltaTime;
            }
            else
            {
                AIController.SwitchToState(EnemyAIController.State.Patrolling);
            }

        } 


    }

    [BeginFoldout("Gizmos")]
    [SerializeField] 
    private float LegTargetHintRadius = 1.0f;
    [SerializeField] 
    private bool OnDraw = true;
    [EndFoldout(includeLast = true)]
    [SerializeField] 
    private bool OnDrawSelected = false;

    private void OnDrawGizmos() {
        if (OnDraw) IKGizmos();
    }
    private void OnDrawGizmosSelected() {
        if (OnDrawSelected) IKGizmos();
    }   

    private void IKGizmos()
    {
        foreach (var lth in LegTargetHints)
        {
            var targetHintPos = lth.TargetHintPositionAdjusted;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetHintPos, LegTargetHintRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, targetHintPos);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(lth.LastPosition, targetHintPos);
            Gizmos.DrawSphere(lth.LastPosition, LegTargetHintRadius);

        }
    }



    protected override void OnAIStateChanged(EnemyAIController.State newState)
    {
        switch (newState)
        {
            case EnemyAIController.State.Idle:
                ScanningTarget = true;
                AttackingTarget = false;
                GetNewScanAngle();
                TargetBodyHeight = StandBodyHeight;
                break;
            case EnemyAIController.State.Patrolling:
                ScanningTarget = false;
                AttackingTarget = false;
                stepForwardPredictionDistance = PatrollingStepForwardPredictionDistance;
                TargetBodyHeight = WalkBodyHeight;
                stepDurationRange = PatrollingStepDurationRange;
                break;
            case EnemyAIController.State.Chasing:
                ScanningTarget = false;
                AttackingTarget = false;
                stepForwardPredictionDistance = ChaseStepForwardPredictionDistance;
                TargetBodyHeight = WalkBodyHeight;
                stepDurationRange = ChaseStepDurationRange;
                FMODUnity.RuntimeManager.PlayOneShotAttached(shriekSound, this.gameObject);
                break;
            case EnemyAIController.State.Attacking:
                ScanningTarget = false;
                AttackingTarget = true;
                AttackSetupTimer = 0;
                AttackTimer = 0;
                AttackResetTimer = 0;
                ArmPosBeforeAttack = RArmTarget.position;
                TargetBodyHeight = AttackBodyHeight;
                break;
            case EnemyAIController.State.Searching:
                AttackingTarget = false;
                ScanningTarget = true;
                GetNewScanAngle();
                TargetBodyHeight = StandBodyHeight;
                break;

            default:
                break;
        }

    }

    private void GetNewScanAngle()
    {
        var newAngle = Random.Range(TargetScanAngleRange.x, TargetScanAngleRange.y);
        newAngle *= Random.value > 0.5f ? 1 : -1;
        targetScanAngle = Vector3.SignedAngle(Vector3.forward, this.transform.forward, Vector3.up) + newAngle;
        targetScanAngle = ((targetScanAngle + 180) % 360) - 180.0f; // Make sure target angle is in range [-180, 180]
    }


}
