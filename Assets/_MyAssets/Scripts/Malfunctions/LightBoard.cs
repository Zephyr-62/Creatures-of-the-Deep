using AdvancedEditorTools.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Malfunction;

public class LightBoard : MonoBehaviour
{
    [SerializeField] private Lightbulb lightBulbPrefab;

    [SerializeField] private List<Lightbulb> lightBulbs = new List<Lightbulb>();
    
    [SerializeField] private float margin = 0.1f;

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

    [Button("Spawn bulbs")]
    private void SpawnLights()
    {
        foreach (var bulb in lightBulbs)
        {
            DestroyImmediate(bulb.gameObject);
        }

        lightBulbs.Clear();

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                var instance = Instantiate(lightBulbPrefab, transform);
                instance.transform.localPosition = new Vector3(x * margin - margin * 1.5f, 0, y * margin - margin * 1.5f);
                lightBulbs.Add(instance);
            }
        }
    }
}
