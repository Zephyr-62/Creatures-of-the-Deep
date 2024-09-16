using AdvancedEditorTools.Attributes;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Gradient gradient;

    [Button("Test button")]
    public void TestFunc(float saturation, float value)
    {
        var n = Random.Range(2, 9);
        GradientColorKey[] gradientColorKey = new GradientColorKey[n];
        GradientAlphaKey[] gradientAlphaKey = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1, 0)
        };

        for(int i = 0; i < n; i++)
        {
            var color = Color.HSVToRGB(Random.value, saturation, value);           

            gradientColorKey[i] = new()
            {
                color = color,
                time = i / (float)(n-1),                
            };
        }

        gradient.SetKeys(gradientColorKey, gradientAlphaKey);
        Debug.Log("New random gradient set");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
