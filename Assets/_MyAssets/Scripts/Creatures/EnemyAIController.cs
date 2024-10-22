using AdvancedEditorTools.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAIController : MonoBehaviour
{
    public Action<State> OnStateChanged;
    [SerializeField] [ReadOnly] protected State CurrentState;
    [SerializeField][ReadOnly] protected Vector3 MovingDirection;
    [SerializeField] protected Transform FollowTarget;


    [BeginFoldout("Idle")]
    [Tooltip("Evaluated every time a patrol target is reached")]
    [Range(0,1)]
    [SerializeField] protected float IdleChance = 0.3f;
    [SerializeField] protected Vector2 IdleDuration = new(5, 10);
    [SerializeField] [ReadOnly] protected float idleTime = 0;
    [SerializeField] [ReadOnly] protected float idleTimer = 0;
    [EndFoldout]

    [BeginFoldout("Patrol")]
    [SerializeField] protected bool RandomPatrol = false;
    [Tooltip("The next patrol target to aim for. Set to -1 for a random target")]
    [SerializeField] protected int PatrolTargetIdx = -1; 
    [SerializeField] protected TargetFollowSettings PatrolToTargetSettings;
    [SerializeField] protected List<Transform> PatrolTargets = new();
    [EndFoldout]

    [BeginFoldout("Chase")]
    [SerializeField] protected Transform ChaseTarget; // Target to chase (the player/submarine)
    [SerializeField] protected ChaseTrigger chaseTrigger;
    [SerializeField] protected TargetFollowSettings ChaseTargetSettings;
    [Tooltip("How close the target has to be to switch to hunt state")]
    [SerializeField] protected float DetectionRange = 30;
    [Tooltip("Measured in degrees in front of the enemy")]
    [Range(0,360)]
    [SerializeField] protected float DetectionViewAngle = 100;  
    [Tooltip("How far the target has to be to switch to search state")]
    [SerializeField] protected float AggroRange = 50;
    [Tooltip("Measured in degrees in front of the enemy")]
    [Range(0, 360)]
    [SerializeField] protected float AggroViewAngle = 130;
    [EndFoldout]
    
    //[BeginFoldout("Attack")]
    //[EndFoldout]

    [BeginFoldout("Search")]
    [SerializeField] protected float SearchTime = 15; // in seconds
    [SerializeField] [ReadOnly] protected float searchTimer = 0; // in seconds
    [SerializeField] protected bool WalkingToLastDetectedSpot;
    [SerializeField] protected Vector3 LastSeenTargetPos;
    [EndFoldout]

    //[BeginFoldout("Debug settings")]
    [SerializeField] private float GizmosFollowTargetLenght = 5f;
    [SerializeField] private Color GizmosDetectionConeColor = Color.green;
    [SerializeField] private bool GizmosDrawAggroCone = false;
    [SerializeField] private Color GizmosAggroConeColor = Color.gray;
    //[EndFoldout(includeLast = true)]
    [SerializeField] protected TargetFollowSettings DebugTargetFollowSettings;




    public enum ChaseTrigger
    {
        Light,
        Heat,
        Sound,
        Always,
    }

    public enum State
    {
        Idle,
        Patrolling,
        Chasing,
        Attacking,
        Searching,
        DEBUG_ManualFollow,
    }

    [System.Serializable]
    public struct TargetFollowSettings
    {
        public float Speed;
        [Tooltip("Minimum distance to target required to trigger OnTargetReached")]
        public float MinDistanceToTarget;
        public float AngleToTargetThreshold;
        public float RotationDelay;
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case State.Idle:
                IdleUpdate();
                if (TryChaseTargetDetection(DetectionRange, DetectionViewAngle)) SwitchToState(State.Chasing);
                break;
            case State.Patrolling:
                FollowTargetUpdate(PatrolToTargetSettings, OnPatrollingTargetReached);
                if (TryChaseTargetDetection(DetectionRange, DetectionViewAngle)) SwitchToState(State.Chasing);
                break;
            case State.Chasing:
                if (TryChaseTargetDetection(AggroRange, AggroViewAngle))
                {
                    LastSeenTargetPos = ChaseTarget.position;
                    FollowTargetUpdate(ChaseTargetSettings, () => SwitchToState(State.Attacking), LastSeenTargetPos);
                }
                else SwitchToState(State.Searching);
                break;
            case State.Attacking:
                // TODO wth do i implement here
                Debug.Log("Attack!");
                SwitchToState(State.Chasing);
                break;
            case State.Searching:
                if(TryChaseTargetDetection(AggroRange, AggroViewAngle)) SwitchToState(State.Chasing);
                else SearchingTargetUpdate();
                break;
            case State.DEBUG_ManualFollow:
                FollowTargetUpdate(DebugTargetFollowSettings, () => { });
                break;

        }
    }

    // Make sure animations are finished before switching??
    [Button("Switch to state")]
    public void SwitchToState(State newState)
    {
        switch (newState)
        {
            case State.Idle:
                OnStateChanged?.Invoke(State.Idle);
                idleTime = Random.Range(IdleDuration.x, IdleDuration.y);
                idleTimer = 0;
                break;

            case State.Patrolling:
                OnStateChanged?.Invoke(State.Patrolling);
                if (FollowTarget == null)
                    FollowTarget = PatrolTargets[0];
                break;

            case State.Chasing:
                OnStateChanged?.Invoke(State.Chasing);
                break;

            case State.Attacking:
                OnStateChanged?.Invoke(State.Attacking);
                break;

            case State.Searching:                
                searchTimer = 0;
                WalkingToLastDetectedSpot = true;
                break;

            default:
                break;
        }

        CurrentState = newState;
    }

    protected void OnPatrollingTargetReached()
    {
        // Set random target if invalid index
        if(PatrolTargetIdx < 0 || PatrolTargetIdx >= PatrolTargets.Count)
        {
            PatrolTargetIdx = Random.Range(0, PatrolTargets.Count);
        }
        // Choose a different random target
        else if (RandomPatrol)
        {
            var newTargetIdx = Random.Range(0, PatrolTargets.Count);
            if (newTargetIdx == PatrolTargetIdx)
                PatrolTargetIdx++;
            else PatrolTargetIdx = newTargetIdx;
        }
        // Choose next target in list
        else
        {
            PatrolTargetIdx++;
        }

        if (PatrolTargetIdx >= PatrolTargets.Count)
            PatrolTargetIdx = 0;

        FollowTarget = PatrolTargets[PatrolTargetIdx];

        if (Random.value < IdleChance)
            SwitchToState(State.Idle);
    }

    protected void IdleUpdate()
    {
        idleTimer += Time.fixedDeltaTime;
        if (idleTimer > idleTime)
            SwitchToState(State.Patrolling);
    }

    protected void FollowTargetUpdate(TargetFollowSettings followSettings, System.Action OnTargetReached)
    {
        FollowTargetUpdate(followSettings, OnTargetReached, FollowTarget.position);
    }

    protected void FollowTargetUpdate(TargetFollowSettings followSettings, System.Action OnTargetReached, Vector3 targetPos)
    {
        var TargetDir = (targetPos - this.transform.position);
        MovingDirection = TargetDir.normalized;
        this.transform.position = Vector3.Lerp(this.transform.position, this.transform.position + MovingDirection, Time.deltaTime * followSettings.Speed);

        var angleToTarget = Vector3.SignedAngle(this.transform.forward, MovingDirection, Vector3.up);
        if (Mathf.Abs(angleToTarget) > followSettings.AngleToTargetThreshold)
            this.transform.Rotate(Vector3.up, angleToTarget * Time.fixedDeltaTime / followSettings.RotationDelay);

        if (TargetDir.magnitude <= followSettings.MinDistanceToTarget)
            OnTargetReached.Invoke();
    }

    protected bool TryChaseTargetDetection(float distance, float viewAngle)
    {
        var targetDetected = (ChaseTarget.position - this.transform.position).magnitude <= distance &&                                  // Min Distance Reached
            Vector3.Angle(this.transform.forward, (ChaseTarget.position - this.transform.position).normalized) <= viewAngle / 2.0f;     // Inside view angle

        return targetDetected;
    }

    protected void SearchingTargetUpdate()
    {
        if (WalkingToLastDetectedSpot)
        {
            FollowTargetUpdate(ChaseTargetSettings, () =>
            {
                WalkingToLastDetectedSpot = false;
                OnStateChanged?.Invoke(State.Searching);
            }, LastSeenTargetPos);
            return;
        }

        searchTimer += Time.fixedDeltaTime;
        if (searchTimer >= SearchTime)
            SwitchToState(State.Patrolling);
    }

    [Button("Autocomplete patrol targets")]
    public void AutofillPatrolTargetsFromParent(Transform parent)
    {
        PatrolTargets = new();
        foreach (Transform child in parent)
            PatrolTargets.Add(child);
    }


    private void OnDrawGizmos()
    {
        if(FollowTarget != null && (CurrentState == State.Patrolling || CurrentState == State.Chasing))
        {
            var moveDir = (FollowTarget.position - this.transform.position).normalized;
            var reachPos = this.transform.position + moveDir * GizmosFollowTargetLenght;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(reachPos, 1);
            Gizmos.DrawLine(this.transform.position, reachPos);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(FollowTarget.position, 1);
            Gizmos.DrawLine(FollowTarget.position, reachPos);
        }

        // Player detection cone
        Gizmos.color = GizmosDetectionConeColor;
        DrawSphericalConeGizmo(transform.position, this.transform.forward, DetectionViewAngle/2.0f, DetectionRange);
        Gizmos.color = GizmosAggroConeColor;
        if(GizmosDrawAggroCone)
            DrawSphericalConeGizmo(transform.position, this.transform.forward, AggroViewAngle/2.0f, AggroRange);


        if (ChaseTarget != null)
        {
            var ChaseTargetAngle = Vector3.Angle(this.transform.forward, (ChaseTarget.position - this.transform.position).normalized);
            var targetInViewCone = 
                (ChaseTarget.position - this.transform.position).magnitude <= DetectionRange && // Min Distance Reached
                ChaseTargetAngle <= DetectionViewAngle/2.0f;

            Gizmos.color = targetInViewCone ? Color.magenta : Color.cyan;
            Gizmos.DrawSphere(ChaseTarget.position, 1);
        }

        if(CurrentState == State.Searching && WalkingToLastDetectedSpot)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(LastSeenTargetPos, 1);
        }
    }


    private readonly int numLatitudeSegments = 10; // Number of concentric circles in the cap
    private readonly int numLongitudeSegments = 20; // Number of points on each circle

    private void DrawSphericalConeGizmo(Vector3 apex, Vector3 direction, float angle, float radius)
    {
        // Normalize the direction to ensure it's a unit vector
        direction.Normalize();

        // Find a vector perpendicular to the direction (central axis)
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);
        if (perpendicular == Vector3.zero) // Handle the case where the direction is exactly upwards
        {
            perpendicular = Vector3.Cross(direction, Vector3.right);
        }
        perpendicular.Normalize();

        // Generate another perpendicular vector (to form the plane of the cap circles)
        Vector3 perpendicular2 = Vector3.Cross(direction, perpendicular).normalized;

        // Convert the angle to radians for trigonometric functions
        float angleRad = angle * Mathf.Deg2Rad;

        // Loop through latitude segments (concentric circles on the cap)
        for (int lat = 0; lat <= numLatitudeSegments; lat++)
        {
            // Fraction of the angular radius for this latitude circle
            float t = (float)lat / numLatitudeSegments;
            float currentAngle = t * angleRad; // Angular distance from the center axis

            // Compute the radius of this latitude circle on the spherical cap
            float circleRadius = Mathf.Sin(currentAngle) * radius;

            // Compute the height along the central axis
            float height = Mathf.Cos(currentAngle) * radius;

            // Compute the center of this latitude circle
            Vector3 circleCenter = apex + direction * height;

            // Draw points around the circumference of this latitude circle
            Vector3 lastPoint = Vector3.zero;
            for (int lon = 0; lon <= numLongitudeSegments; lon++)
            {
                // Longitude angle around the circle
                float theta = lon * Mathf.PI * 2 / numLongitudeSegments;

                // Compute the point on the circle
                Vector3 pointOnCircle = circleCenter + (perpendicular * Mathf.Cos(theta) + perpendicular2 * Mathf.Sin(theta)) * circleRadius;

                // Draw the lines between points on the latitude circle
                if (lon > 0)
                {
                    Gizmos.DrawLine(lastPoint, pointOnCircle);
                }

                // Store the point as the last point for drawing circle segments
                lastPoint = pointOnCircle;

                // Draw lines from the apex (tip) to the edge points (forming the cone)
                if (lat == numLatitudeSegments)
                {
                    Gizmos.DrawLine(apex, pointOnCircle);
                }
            }
        }
    }
}
