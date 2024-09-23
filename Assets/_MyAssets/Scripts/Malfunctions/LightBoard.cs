using AdvancedEditorTools.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Malfunction;

public class LightBoard : MonoBehaviour
{
    [SerializeField] private Lightbulb lightBulbPrefab;
    [SerializeField] private Malfunction malfunction;

    private List<Lightbulb> lightBulbs = new List<Lightbulb>();

    private void Start()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                var instance = Instantiate(lightBulbPrefab, transform);
                instance.transform.localPosition = new Vector3(x * 0.1f, 0, y * 0.1f);
                lightBulbs.Add(instance);
            }
        }
    }

    private void Update()
    {
        SetLights(malfunction.ErrorCode);
    }

    public void SetLights(ErrorMask mask)
    {
        IList list = Enum.GetValues(typeof(ErrorMask));
        for (int i = 1; i < list.Count; i++)
        {
            SetLight(lightBulbs[i-1], (ErrorMask)list[i], mask);
        }
    }

    private void SetLight(Lightbulb bulb, ErrorMask error, ErrorMask mask)
    {
        bulb.Set((mask & error) == error);
    }
}
