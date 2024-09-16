using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarinePhysicsSystem : MonoBehaviour
{
    [Range(-1f, 1f)]
    public float ThrustInput = 0;
    [Range(-1f, 1f)]
    public float SteerInput = 0;
    [Range(-1f, 1f)]
    public float BuoyancyInput = 0;


    [Header("Speed parameters")]
    [SerializeField] private float maxThrustSpeed = 3f;
    [SerializeField] private float thrustAccelerationForce = 10;
    [SerializeField] private float thrustDeadzone = 0.05f;

    [SerializeField] private float maxAngularVelocity = 1f;
    [SerializeField] private float steeringAccelerationForce = 10;
    [SerializeField] private float steeringDeadzone = 0.05f;


    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        rb.maxAngularVelocity = maxAngularVelocity;
        if ((ThrustInput >= thrustDeadzone || ThrustInput <= -thrustDeadzone) && 
            rb.velocity.magnitude < maxThrustSpeed)
                rb.AddForce(thrustAccelerationForce * ThrustInput * this.gameObject.transform.forward, ForceMode.Force);

        if (SteerInput >= steeringDeadzone || SteerInput <= -steeringDeadzone)
            rb.AddTorque(steeringAccelerationForce * SteerInput * Vector3.up, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ThrustInput = 0;
    }


}
