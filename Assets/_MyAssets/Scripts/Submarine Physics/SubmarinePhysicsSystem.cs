using AdvancedEditorTools.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class SubmarinePhysicsSystem : MonoBehaviour
{
    [BeginColumnArea(areaStyle = LayoutStyle.BoxRound)]

    [SerializeField] private PhysicalControlSurface throttleControl;
    [SerializeField] private PhysicalControlSurface steeringControl;
    [SerializeField] private PhysicalControlSurface pitchControl;
    [SerializeField] private PhysicalControlSurface elevationControl;

    public float thrust => Mathf.Clamp(throttleControl.GetFloatValue(), -limit, limit);
    public float steering => Mathf.Clamp(steeringControl.GetFloatValue(), -limit, limit);
    public float pitch => Mathf.Clamp(pitchControl.GetFloatValue(), -limit, limit);
    public float elevation => Mathf.Clamp(elevationControl.GetFloatValue(), -limit, limit);
    
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
    private bool engineEnabled;
    private float limit = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        TurnOn();
    }

    void FixedUpdate()
    {
        rb.maxAngularVelocity = maxAngularVelocity;
        
        if (engineEnabled)
        {
            // Thrust
            if ((thrust >= thrustDeadzone || thrust <= -thrustDeadzone) &&
                rb.velocity.magnitude < maxThrustSpeed)
                rb.AddForce(thrustAccelerationForce * thrust * this.gameObject.transform.forward, ForceMode.Force);
            // TODO: OnStartEngine, OnEngineSpeedNotch  ?? Does this belong to the PCL or the PS

            // Yaw - Steering
            if (steering >= steeringDeadzone || steering <= -steeringDeadzone)
                rb.AddTorque(steeringAccelerationForce * steering * Vector3.up, ForceMode.Acceleration);

            // Pitch
            prevPitchInput = Mathf.Lerp(prevPitchInput, pitch, Time.deltaTime / pitchInputDelay);
            TargetPitch = -prevPitchInput * MaxPitch;

            if (elevation != 0)
                rb.AddForce(BuoyancyStrength * elevation * Vector3.up, ForceMode.Force);
        }
        
        PitchStabilization();

        // Roll
        RollStabilization();

        // Buoyancy
        
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

    public void TurnOff()
    {
        engineEnabled = false;
        onStopEngine.Invoke();
    }

    public void TurnOn()
    {
        engineEnabled = true;
        onStartEngine.Invoke();
    }

    public void SetMaxPower(float max)
    {
        limit = Mathf.Clamp01(max);
    }
}
