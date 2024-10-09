using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverClick : MonoBehaviour
{
    [SerializeField] private Lever lever;
    [SerializeField] private FMODUnity.EventReference baseEngine;
    [SerializeField] private int division = 100;
    private FMOD.Studio.EventInstance instance;
    private float last;
    private float interval;

    void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(baseEngine);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, lever.transform, lever.GetComponent<Rigidbody>());
        interval = 1f / division;
        last = Mathf.Floor(lever.GetFloatValue() / interval) * interval;
    }

    private void OnEnable()
    {
        lever.onValueChanged.AddListener(ValueChanged);
    }

    private void OnDisable()
    {
        lever.onValueChanged.AddListener(ValueChanged);
    }

    private void ValueChanged()
    {
        float current = Mathf.Floor(lever.GetFloatValue() / interval) * interval;

        if (current != last)
        {
            instance.start();
            last = current;
        }
    }
}
