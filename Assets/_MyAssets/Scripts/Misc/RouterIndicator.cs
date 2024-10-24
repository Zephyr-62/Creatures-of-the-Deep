using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouterIndicator : MonoBehaviour
{
    [SerializeField] private Valve valve;
    [SerializeField] private RouterIndicator right;
    [SerializeField] private RouterIndicator left;
    private Renderer line;

    private void Awake()
    {
        line = GetComponent<Renderer>();
    }

    public void Set(bool connected)
    {
        line.enabled = connected;

        if (left) left.Set(connected && valve.crank.Get01FloatValue() < 0.8f);
        if (right) right.Set(connected && valve.crank.Get01FloatValue() > 0.2f);
    }
}
