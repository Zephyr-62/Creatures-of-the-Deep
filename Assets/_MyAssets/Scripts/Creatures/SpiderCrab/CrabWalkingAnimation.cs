using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabWalkingAnimation : MonoBehaviour
{
    [SerializeField] private float BodyHeightAdjustDelay = 0.2f;
    [SerializeField] private float BaseHeight = 20.0f;

    [SerializeField]
    private List<LegTargetHint> LegTargetHints;

    [SerializeField] public LayerMask WalkableLayer;
    [SerializeField] private Ease animationEase = Ease.OutCubic;


    [System.Serializable]
    protected class LegTargetHint
    {
        public Transform Leg;
        public Transform LegIKTarget;
        public Vector3 TargetHintPosition { get => Leg.TransformPoint(RelativeTargetHintPosition); }
        public Vector3 RelativeTargetHintPosition;   // Relative position to Leg of IK target hint
        public float MaxStepDistance;      // = 4;

        public float HintBaseHeight;    // = -3.9f;
        public Vector2 HintHeightRange; // = new (-4.25f, -2f);

        public Vector3 LastPosition; // Last valid position where the actual target should rest at
        public Sequence Sequence;
    }

    public float StepHeight = 5.5f;
    public Vector2 StepDurationRange = new(8, 12);


    [ContextMenu("Reset Last Position")]
    private void ResetLastPos()
    {
        foreach (LegTargetHint lth in LegTargetHints)
        {
            lth.LastPosition = lth.TargetHintPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update body
        float heightAcc = 0;
        foreach (var legTargetHint in LegTargetHints)
        {
            heightAcc += legTargetHint.LegIKTarget.position.y;
        }
        this.transform.position = new (transform.position.x,
            Mathf.Lerp(transform.position.y, (heightAcc / LegTargetHints.Count) + BaseHeight, Time.deltaTime / BodyHeightAdjustDelay),
                transform.position.z
            );


        // Update legs
        foreach (LegTargetHint lth in LegTargetHints)
        {
            // Update relative target hint
            if (Physics.Raycast(new Vector3(lth.TargetHintPosition.x, this.transform.position.y, lth.TargetHintPosition.z), Vector3.down, out var raycastHit, 100, WalkableLayer))
            {
                lth.RelativeTargetHintPosition = lth.Leg.InverseTransformPoint(raycastHit.point); // TODO fix max height or smth
            }

            // If target is too far from target hint, then update last valid position
            if(lth.Sequence == null || !lth.Sequence.active)
            {
                // Set target to last valid global position
                lth.LegIKTarget.position = lth.LastPosition;

                var zDiff = (lth.LegIKTarget.position - lth.TargetHintPosition).sqrMagnitude;
                if (zDiff > lth.MaxStepDistance || zDiff < -lth.MaxStepDistance) // TODO prevent step if too many legs moving
                {
                    lth.LastPosition = lth.TargetHintPosition;

                    // lth.LegIKTarget.DOKill(true);
                    lth.Sequence = lth.LegIKTarget.DOJump(lth.LastPosition, StepHeight, 1, Random.Range(StepDurationRange.x, StepDurationRange.y)).SetEase(animationEase);
                }
            }
        }
    }

    [Button("Tween Test")]
    public void TweenTest()
    {
        foreach (LegTargetHint lth in LegTargetHints)
        {
            lth.LegIKTarget.DOKill(true);
            lth.Sequence = lth.LegIKTarget.DOJump(lth.LegIKTarget.position + Vector3.forward * 5, StepHeight, 1, StepDurationRange.x).SetEase(animationEase);
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
            var targetHintPos = lth.TargetHintPosition;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetHintPos, LegTargetHintRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, targetHintPos);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(lth.LastPosition, targetHintPos);
            Gizmos.DrawSphere(lth.LastPosition, LegTargetHintRadius);

        }
    }
}
