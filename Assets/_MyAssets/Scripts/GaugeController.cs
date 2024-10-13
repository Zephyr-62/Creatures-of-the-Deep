using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GaugeController : MonoBehaviour
{
    public GameManager gameManager;

    public Transform[] needles;

    //Variables for the gauges
    public float maxOxigenLevel = 0.5f;
    public float minOxigenLevel = -0.5f;
    public float maxNitroValue = 0.4f;
    public float minNitroValue = -0.3f;
    public float oxigenDepletionTimer = 10f;
    public float currentOxygenValue;
    public float oxigenDepletionSpeed = 0.5f;
    public float oscillationSpeed = 0.1f;

    private float normalizedValue, newYPos, movement;
    private float changeInterval = 1f;
    private bool movingUp;

    //Variables for the vignette
    public Volume volume;
    private Vignette vignette;

    public float vignetteStartThreshold = 5f; 
    public float vignetteMaxIntensity = 0.7f; 
    public float vignetteIntensityMultiplier = 0.5f; 


    private void Start()
    {
        currentOxygenValue = oxigenDepletionTimer;

        InvokeRepeating("RandomlyChangeDirection", changeInterval, changeInterval);

        if (volume.profile.TryGet(out vignette))
        {
            vignette.intensity.value = 0; 
        }
        else
        {
            Debug.LogError("Vignette effect not found");
        }
    }

    private void Update()
    {
        UpdateOxygenNeedle();

        UpdateNitroNeedle();
    }

    void UpdateOxygenNeedle()
    {
        currentOxygenValue -= oxigenDepletionSpeed * Time.deltaTime;
        currentOxygenValue = Mathf.Clamp(currentOxygenValue, 0f, oxigenDepletionTimer);

        normalizedValue = currentOxygenValue / oxigenDepletionTimer;

        newYPos = Mathf.Lerp(minOxigenLevel, maxOxigenLevel, normalizedValue);

        needles[0].localPosition = new Vector3(needles[0].localPosition.x, newYPos, needles[0].localPosition.z);

        if (vignette != null)
        {
            if (currentOxygenValue < vignetteStartThreshold)
            {
                float normalizedVignetteValue = Mathf.InverseLerp(vignetteStartThreshold, 0, currentOxygenValue);
                vignette.intensity.value = Mathf.Lerp(0, vignetteMaxIntensity, normalizedVignetteValue * vignetteIntensityMultiplier);
            }
            else
            {
                vignette.intensity.value = 0;
            }
        }

        if (currentOxygenValue == 0f)
        {
            gameManager.GameOver();
        }
    }

    void UpdateNitroNeedle()
    {
        movement = oscillationSpeed * Time.deltaTime * (movingUp ? 1 : -1);

        needles[1].localPosition += new Vector3(0, movement, 0);

        if (needles[1].localPosition.y >= maxNitroValue)
        {
            needles[1].localPosition = new Vector3(needles[1].localPosition.x, maxNitroValue, needles[1].localPosition.z);

            movingUp = false;
        }
        else if(needles[1].localPosition.y <= minNitroValue)
        {
            needles[1].localPosition = new Vector3(needles[1].localPosition.x, minNitroValue, needles[1].localPosition.z);

            movingUp = true;
        }
    }

    void RandomlyChangeDirection()
    {
        if(Random.value > 0.5f)
        {
            movingUp = !movingUp;
        }
    }
}
