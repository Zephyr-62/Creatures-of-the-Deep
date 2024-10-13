using AdvancedEditorTools.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Malfunction;

public class LightBoard : MonoBehaviour
{
    //[SerializeField] private Lightbulb lightBulbPrefab;    
    //[SerializeField] private float margin = 0.1f;
    //private Vector2Int grid = new Vector2Int(4, 4);
    //private Dictionary<Lightbulb, ErrorMask> lightBulbs = new Dictionary<Lightbulb, ErrorMask>();

    //private void Start()
    //{
    //    SpawnLights();
    //}

    //public void SetLights(ErrorMask mask)
    //{
    //    Lightbulb.SetAll(mask);
    //}

    //[Button("Spawn bulbs")]
    //private void SpawnLights()
    //{
    //    foreach (var bulb in lightBulbs)
    //    {
    //        if (!bulb.Key) continue;
    //        DestroyImmediate(bulb.Key.gameObject);
    //    }

    //    lightBulbs.Clear();

    //    IList list = Enum.GetValues(typeof(ErrorMask));

    //    for (int y = 0; y < grid.y; y++)
    //    {
    //        for (int x = 0; x < grid.x; x++)
    //        {
    //            var instance = Instantiate(lightBulbPrefab, transform);
    //            instance.transform.localPosition = new Vector3(x * margin - margin * 1.5f, 0, y * margin - margin * 1.5f);
    //            lightBulbs.Add(instance, (ErrorMask)list[y*x + 1]);
    //        }
    //    }
    //}
}
