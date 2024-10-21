using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingScript : MonoBehaviour
{

    public float speed = 1.5f;
    public float limit = 20f;
    public bool randomStart = false;
    public float random = 0;

    private void Awake()
    {
        if (randomStart)
            random = Random.Range(0f, 1f);
        
    }


    void Update()
    {
        float angle = limit *Mathf.Sin(Time.time+random*speed);
        transform.localRotation = Quaternion.Euler(0,0,angle);
        
    }
}
