using AdvancedEditorTools.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class SubmarinePhysicsSystem : MonoBehaviour
{
    [BeginColumnArea(areaStyle = LayoutStyle.BoxRound)]
    [Range(-1f, 1f)]
    public float ThrustInput = 0;
    [Range(-1f, 1f)]
    public float SteerInput = 0;
    [Range(-1f, 1f)]
    public float PitchInput = 0;
    [Tooltip("Positive => Submarine floats")]
    public float BuoyancyInput = 0;
    [EndColumnArea]

    [SerializeField] private float BuoyancyStrength = 75;

    [BeginColumnArea(.5f, columnStyle = LayoutStyle.Bevel)]
    [Header("Thrust")]
    [SerializeField] private float maxThrustSpeed = 3f;
    [SerializeField] private float thrustAccelerationForce = 10;
    [SerializeField] private float thrustDeadzone = 0.05f;

    [NewColumn]
    [Header("Steering")]
    [Tooltip("In radians")]
    [SerializeField] private float maxAngularVelocity = 1f;
    [SerializeField] private float steeringAccelerationForce = 10;
    [SerializeField] private float steeringDeadzone = 0.05f;
    [EndColumnArea]


    [BeginColumnArea(.5f, columnStyle = LayoutStyle.Bevel)]
    [Header("Pitch")]
    [SerializeField] private float pitchInputDelay = 0.35f; 
    [SerializeField] private float TargetPitch = 0.0f;
    [SerializeField] private float MaxPitch = 45;
    [Header("Pitch PID Constants")]
    [SerializeField] private float pitchPGain = 0.15f;  // Proportional gain
    [SerializeField] private float pitchIGain = 0.025f;  // Integral gain
    [SerializeField] private float pitchDGain = 0.02f;  // Derivative gain     

    [NewColumn]
    [Header("Roll")]
    [SerializeField] private float TargetRoll = 0.0f;
    [Header("Roll PID Constants")]
    [SerializeField] private float rollPGain = 0.15f;  // Proportional gain
    [SerializeField] private float rollIGain = 0.025f;  // Integral gain
    [SerializeField] private float rollDGain = 0.02f;  // Derivative gain
    [EndColumnArea]

    [Header("Other settings")]
    [SerializeField] private float minRelativeCollisionVelocityForThrustShutdown = 0.5f;

    [SerializeField] public UnityEvent onStartEngine;
    [SerializeField] public UnityEvent onStopEngine;


    // Private variables for PID control
    private float prevRollErr = 0.0f;
    private float rollIntegrarErrAcc = 0.0f;
    private float prevPitchInput = 0.0f;
    private float prevPitchErr = 0.0f;
    private float pitchIntegrarErrAcc = 0.0f;

    private Rigidbody rb;
    private bool engineEnabled = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartEngine();
    }

    void FixedUpdate()
    {
        rb.maxAngularVelocity = maxAngularVelocity;
        
        if (engineEnabled)
        {
            // Thrust
            if ((ThrustInput >= thrustDeadzone || ThrustInput <= -thrustDeadzone) &&
                rb.velocity.magnitude < maxThrustSpeed)
                rb.AddForce(thrustAccelerationForce * ThrustInput * this.gameObject.transform.forward, ForceMode.Force);
            // TODO: OnStartEngine, OnEngineSpeedNotch  ?? Does this belong to the PCL or the PS

            // Yaw - Steering
            if (SteerInput >= steeringDeadzone || SteerInput <= -steeringDeadzone)
                rb.AddTorque(steeringAccelerationForce * SteerInput * Vector3.up, ForceMode.Acceleration);

            // Pitch
            prevPitchInput = Mathf.Lerp(prevPitchInput, PitchInput, Time.deltaTime / pitchInputDelay);
            TargetPitch = -prevPitchInput * MaxPitch;
        }
        
        PitchStabilization();

        // Roll
        RollStabilization();

        // Buoyancy
        if (BuoyancyInput != 0)
            rb.AddForce(BuoyancyStrength * BuoyancyInput * Vector3.up, ForceMode.Force);
    }

    private void PitchStabilization()
    {
        float currentPitch = transform.eulerAngles.x;
        float pitchErr = TargetPitch - currentPitch;
        if (pitchErr > 180) pitchErr -= 360;
        else if (pitchErr < -180) pitchErr += 360;
        float pitchErrDiff = pitchErr - prevPitchErr;
        prevPitchErr = pitchErr;

        // Integral term (accumulate error over time)
        pitchIntegrarErrAcc += pitchErr * Time.fixedDeltaTime;

        // Calculate the PID output
        float pitchPIDOut = pitchPGain * pitchErr + pitchIGain * pitchIntegrarErrAcc + pitchDGain * pitchErrDiff / Time.fixedDeltaTime;
        rb.AddTorque(pitchPIDOut * transform.right, ForceMode.Acceleration);
    }

    private void RollStabilization()
    {
        float currentRoll = transform.eulerAngles.z;
        float rollErr = TargetRoll - currentRoll;
        if (rollErr > 180) rollErr -= 360;
        else if (rollErr < -180) rollErr += 360;
        float rollErrDiff = rollErr - prevRollErr;
        prevRollErr = rollErr;

        // Integral term (accumulate error over time)
        rollIntegrarErrAcc += rollErr * Time.fixedDeltaTime;

        // Calculate the PID output
        float rollPIDOut = rollPGain * rollErr + rollIGain * rollIntegrarErrAcc + rollDGain * rollErrDiff / Time.fixedDeltaTime;
        rb.AddTorque(transform.forward * rollPIDOut, ForceMode.Acceleration);
    }

    public void StopEngine()
    {
        engineEnabled = false;
        onStopEngine.Invoke();
    }

    public void StartEngine()
    {
        engineEnabled = true;
        onStartEngine.Invoke();
    }
}
