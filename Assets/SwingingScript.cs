using System.Collections;
using UnityEngine;

public class SwingingScript : MonoBehaviour
{
    public float initialSpeed = 1.5f; 
    public float limit = 20f;          // Maximum angle of swing
    public bool randomStart = false;   
    public float decelerationRate = 0.1f; // Rate at which the speed decreases
    private float currentSpeed;         
    private float random = 0;           // Random value for starting angle
    private bool isSwinging = false;    // Controls whether the object is currently swinging
    private bool isAtLimit = false;     // Indicates if the swing has reached its limit

    private void Awake()
    {
        if (randomStart)
            random = Random.Range(0f, 1f);

        currentSpeed = 0f; // Initialize current speed to 0
    }

    void Update()
    {
        // Start swinging when space key is pressed
        if (!isSwinging && Input.GetKeyDown(KeyCode.Space))
        {
            isSwinging = true;
            currentSpeed = initialSpeed;
            isAtLimit = false;
        }

        // Apply swinging motion if enabled
        if (isSwinging)
        {
            float angle = limit * Mathf.Sin(Time.time * currentSpeed + random);
            transform.localRotation = Quaternion.Euler(0, 0, angle);

            // Adjust limit when reaching extremes
            if ((angle >= limit || angle <= -limit) && !isAtLimit)
            {
                limit -= 0.5f;
                isAtLimit = true;
            }
            else if (angle < limit && angle > -limit)
            {
                isAtLimit = false;
            }

            // Gradually decrease speed
            currentSpeed -= decelerationRate * Time.deltaTime;
            if (currentSpeed <= 0)
            {
                currentSpeed = 0;
                isSwinging = false;
            }
        }
    }
}
