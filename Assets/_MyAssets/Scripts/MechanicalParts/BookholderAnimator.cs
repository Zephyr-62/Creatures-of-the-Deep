using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BookholderAnimator : MonoBehaviour
{
    [BeginFoldout("Inputs")]
    [Header("Main Inputs")]
    [Range(0f, 1f)]
    public float BaseRotation = 0;
    [Range(0f, 1f)]
    public float TargetDepth = 0.5f;
    [Range(0f, 1f)]
    public float TargetHeight = 0.5f;

    [Header("Secondary Inputs")] // These are used to animate the book pick and drop
    [Range(0f, 1f)]
    [SerializeField] private float BaseRackDistance = 0f;
    [Range(0f, 1f)]
    [SerializeField] private float ArmHeightMount = 0f;
    [Space(10)]
    public bool ControlsEnabled = false;
    [EndFoldout]


    [BeginFoldout("Configs")]
    [Header("Basic")]
    [SerializeField] private Vector2 BaseRackDistRange = new(0, -0.5f);
    [SerializeField] private Vector2 ArmHeightMountRange = new(0, -0.155f);
    [Header("BookDisplay")]
    [SerializeField] private Vector2 BaseRotationRange = new(250, 110);
    [SerializeField] private Vector2 TargetHeightRange = new(-0.85f, -0.4f);
    [SerializeField] private Vector2 DepthMinRange = new(0, -0.25f);
    [SerializeField] private Vector2 DepthMaxRange = new(-0.6f, -0.5f);
    [Header("Animation Times")]
    [SerializeField] private float CollapseTime = 1.5f;        
    [SerializeField] private float ClawOpenTime = 0.35f;        
    [SerializeField] private float SwitchToBookDisplayTime = 1.5f;      
    [SerializeField] private int BookselfInteractionDurationMS = 250;      
    
    [Header("CollapsedState")]
    [SerializeField] private Vector2 OpenedClawsAngle = new(-80, 120);      // X-axis 
    [SerializeField] private Vector2 CollapsedClawsAngle = new(-30, 175);   // X-axis
    [SerializeField] private float CollapsedRackDistance = 0.5f;    
    [Header("BookSelection")]
    [SerializeField] private float BookSelectionBaseRotation = 0.75f;
    [SerializeField] private float BookSelectionTargetDistanceToInteract = -0.2f;    // Right axis
    [SerializeField] private float BookSelectionInteractionDistance = 0.4f;
    [SerializeField] private Vector3 BookSelectionTargetInteractionRotation = new(0, -65, 0);


    private const float DepthRotationCompensation = 1 - 2/Mathf.PI; 
    [EndFoldout]

    [BeginFoldout("Transforms")]
    [SerializeField] private Transform IK_RIG;
    [SerializeField] private Transform IK_hint;
    [SerializeField] private Transform IK_target;
    [SerializeField] private Transform PlayerPOV;
    [LineSeparator]
    [SerializeField] private Transform PivotCeilMount;
    [SerializeField] private Transform PivotCeilMountRack;
    [SerializeField] private Transform PivotHeightHinge;
    [SerializeField] private Transform PivotArm1;
    [SerializeField] private Transform PivotArm2;
    [SerializeField] private Transform PivotBook;
    [SerializeField] private Transform Claw1;
    [SerializeField] private Transform Claw2;
    [Header("Collapsed State Hints")]
    [SerializeField] private Transform CollapsedHint;
    [SerializeField] private Transform CollapsedTarget;
    [EndFoldout]
    [EndFoldout]


    [SerializeField] private List<RotationMatcher> RotationMatchers;

    [System.Serializable]
    public class RotationMatcher
    {
        public Transform Matched; // Rotation applied here
        public Transform Matcher; // Rotation read from here
        public float factor; // Scales the rotation applied

        public void ApplyRotation(Vector3 forward)
        {
            Matched.localRotation = Quaternion.AngleAxis(Vector3.SignedAngle(forward, Matcher.forward, Vector3.up) * factor, Vector3.up);
        }
    }


    public enum ClawState
    {
        Collapsed = -1,
        BookDisplay,
        BookshelfInteract,
    }

    [ReadOnly]
    [SerializeField]
    private ClawState CurrentState = ClawState.Collapsed;
    [SerializeField] private Transform TargetBookSlot;
    private Transform Submarine;

    public void SetTargetBookSlot(Transform target) => TargetBookSlot = target; 

    [Button("Change state")]
    async public Task ChangeToState(ClawState newState)
    {
        switch (newState)
        {
            case ClawState.BookshelfInteract:
                if (CurrentState == ClawState.BookDisplay)
                    ControlsEnabled = false;
                await SetBaseRotation(BookSelectionBaseRotation, 0.5f);
                await Task.Delay(250);
                await SetRackDistance(1, 0.75f);
                await Task.Delay(150);

                if (CurrentState == ClawState.Collapsed)
                    _ = SetClawOpen(true);
                await SetHeightMount(1, 0.5f);

                break;

            case ClawState.Collapsed:   // Can only come from BookshelfInteract
                await DOCollapseArm();
                break;
            case ClawState.BookDisplay: // Can only come from BookshelfInteract 
                await SetHeightMount(0, 0.5f);
                await Task.Delay(250);
                await SetRackDistance(0, 0.75f);
                await Task.Delay(150);
                await AsyncBookDisplayUpdate();
                ControlsEnabled = true;
                break;
            default:
                break;
        }

        CurrentState = newState;
    }

    [Button("InteractWithBookshelf")]
    async private void _InteractWithBookshelf()
    {
        await InteractWithBookshelf(() => { });
    }

    async public Task InteractWithBookshelf(TweenCallback duringInteraction)
    {
        await IK_target.DOLocalMove(
            IK_RIG.InverseTransformPoint(IK_target.position + TargetBookSlot.up * BookSelectionInteractionDistance), 0.5f)
            .OnComplete(duringInteraction)
            .AsyncWaitForCompletion();
        await Task.Delay(BookselfInteractionDurationMS);
        await IK_target.DOLocalMove(
            IK_RIG.InverseTransformPoint(IK_target.position - TargetBookSlot.up * BookSelectionInteractionDistance), 0.5f)
            .AsyncWaitForCompletion();
    }

    [Button("Tween Base Rotation")]
    async private Task SetBaseRotation(float val, float time)
    {
        await DOTween.To(() => BaseRotation, x => BaseRotation = x, val, time).AsyncWaitForCompletion();
    }

    [Button("Tween Height Mount")]
    async private Task SetHeightMount(float val, float time)
    {
        await DOTween.To(() => ArmHeightMount, x => ArmHeightMount = x, val, time).AsyncWaitForCompletion();
    }

    [Button("Tween Rack Distance")]
    async private Task SetRackDistance(float val, float time)
    {
        await DOTween.To(() => BaseRackDistance, x => BaseRackDistance = x, val, time).AsyncWaitForCompletion();
    }

    [Button("Tween Claw Open")]
    async private Task SetClawOpen(bool isOpen)
    {
        var clawState = isOpen ? OpenedClawsAngle : CollapsedClawsAngle;

        var tasks = new Task[2];
        tasks[0] = Claw1.DOLocalRotate(new Vector3(clawState.x, 0, 0), ClawOpenTime).AsyncWaitForCompletion();
        tasks[1] = Claw2.DOLocalRotate(new Vector3(clawState.y, 0, 0), ClawOpenTime).AsyncWaitForCompletion();
        await Task.WhenAll(tasks);
    }

    [Button("Tween Collapse Arm")]
    async private Task DOCollapseArm()
    {
        await SetHeightMount(0, 0.75f);
        _ = SetRackDistance(CollapsedRackDistance, 1f);
        await SetClawOpen(false);
        await SetBaseRotation(0.5f, 1.25f);

        var tasks = new Task[3];
        tasks[0] = IK_target.DOLocalMove(IK_RIG.InverseTransformPoint(CollapsedTarget.position), CollapseTime).AsyncWaitForCompletion();
        tasks[1] = IK_hint.DOLocalMove(IK_RIG.InverseTransformPoint(CollapsedHint.position), CollapseTime).AsyncWaitForCompletion();
        tasks[2] = IK_target.DORotate(Quaternion.LookRotation(CollapsedTarget.forward, CollapsedTarget.up).eulerAngles, CollapseTime).AsyncWaitForCompletion();
        await Task.WhenAll(tasks);
    }

    [Button("Tween To Book Slot")]
    async public Task DOTweenToBookSlot()
    {
        var targetPos = TargetBookSlot.position + BookSelectionTargetDistanceToInteract * TargetBookSlot.up;
        var hintPos = TargetBookSlot.position - 2 * TargetBookSlot.up;

        var tasks = new Task[3];
        tasks[0] = IK_target.DOLocalMove(IK_RIG.InverseTransformPoint(targetPos), SwitchToBookDisplayTime).AsyncWaitForCompletion();
        tasks[1] = IK_hint.DOLocalMove(IK_RIG.InverseTransformPoint(hintPos), SwitchToBookDisplayTime).AsyncWaitForCompletion();
        tasks[2] = IK_target.DORotate(Quaternion.LookRotation(TargetBookSlot.up, TargetBookSlot.forward).eulerAngles, SwitchToBookDisplayTime).AsyncWaitForCompletion();
        await Task.WhenAll(tasks);
        // await IK_target.DOLookAt(TargetBookSlot.position, 0.5f, up: Submarine.up).AsyncWaitForCompletion();
    }

    private void Start()
    {
        Submarine = FindAnyObjectByType<SubmarinePhysicsSystem>().transform;
    }

    // Update is called once per frame
    void Update()
    {       
        // Calculate base arm position
        PivotCeilMount.localRotation = Quaternion.Euler(0, Mathf.Lerp(BaseRotationRange.x, BaseRotationRange.y, BaseRotation), 0);
        PivotCeilMountRack.localPosition = Vector3.forward * Mathf.Lerp(BaseRackDistRange.x, BaseRackDistRange.y, BaseRackDistance);
        PivotHeightHinge.localPosition = Vector3.up * Mathf.Lerp(ArmHeightMountRange.x, ArmHeightMountRange.y, ArmHeightMount);


        if(CurrentState == ClawState.BookDisplay)
            BookDisplayUpdate();

        // Apply all rotation matchers
        foreach (var rotMatcher in RotationMatchers)
            rotMatcher.ApplyRotation(this.transform.forward);
    }

    private void BookDisplayUpdate()
    {
        var ArmDir = PivotBook.position - PlayerPOV.position;
        ArmDir = Vector3.Scale(ArmDir, new Vector3(1, 0, 1)).normalized;

        var heightVal = Mathf.Lerp(TargetHeightRange.x, TargetHeightRange.y, TargetHeight);
        var depthMin = Mathf.Lerp(DepthMinRange.x, DepthMinRange.y, TargetHeight);
        var depthMax = Mathf.Lerp(DepthMaxRange.x, DepthMaxRange.y, TargetHeight);
        var depthVal = Mathf.Lerp(depthMin, depthMax, TargetDepth) + Mathf.Sin(BaseRotation * Mathf.PI) * DepthRotationCompensation;

        IK_target.position = PivotArm1.position + PivotCeilMount.up * heightVal + ArmDir * depthVal;
        IK_target.LookAt(PlayerPOV, Submarine.up);
        IK_hint.position = PivotArm1.position + ArmDir;
    }

    [Button("Tween Display Book")]
    async private Task AsyncBookDisplayUpdate()
    {
        var ArmDir = PivotBook.position - PlayerPOV.position;
        ArmDir = Vector3.Scale(ArmDir, new Vector3(1, 0, 1)).normalized;

        var heightVal = Mathf.Lerp(TargetHeightRange.x, TargetHeightRange.y, TargetHeight);
        var depthMin = Mathf.Lerp(DepthMinRange.x, DepthMinRange.y, TargetHeight);
        var depthMax = Mathf.Lerp(DepthMaxRange.x, DepthMaxRange.y, TargetHeight);
        var depthVal = Mathf.Lerp(depthMin, depthMax, TargetDepth) + Mathf.Sin(BaseRotation * Mathf.PI) * DepthRotationCompensation;


        var targetPos = PivotArm1.position + PivotCeilMount.up * heightVal + ArmDir * depthVal;
        var hintPos = PivotArm1.position + ArmDir;

        var tasks = new Task[3];
        tasks[0] = IK_target.DOLocalMove(IK_RIG.InverseTransformPoint(targetPos), SwitchToBookDisplayTime).AsyncWaitForCompletion();
        tasks[1] = IK_hint.DOLocalMove(IK_RIG.InverseTransformPoint(hintPos), SwitchToBookDisplayTime).AsyncWaitForCompletion();
        tasks[2] = IK_target.DORotate(Quaternion.LookRotation(PlayerPOV.position - targetPos, Submarine.up).eulerAngles, 0.5f).AsyncWaitForCompletion();
        await Task.WhenAll(tasks);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(TargetBookSlot.position, TargetBookSlot.position + TargetBookSlot.up * 0.25f);
        Gizmos.color = Color.red;
        if(Submarine != null)
            Gizmos.DrawLine(TargetBookSlot.position, TargetBookSlot.position + TargetBookSlot.forward * 0.35f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(IK_RIG.position, IK_RIG.position + IK_RIG.InverseTransformPoint(CollapsedTarget.position));
    }


}
