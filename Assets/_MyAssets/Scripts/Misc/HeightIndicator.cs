using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class HeightIndicator : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    [SerializeField] private float height;


    private void OnDrawGizmos()
    {
        var pos = SceneView.currentDrawingSceneView.camera.transform.position;

        

        for (int y = -5; y < 5; y++)
        {
            for (int x = -5; x < 5; x++)
            {
                var p = pos + (Vector3.forward * x + Vector3.right * y) * 10f;
                p.y = terrain.SampleHeight(p) + height;
                Gizmos.DrawSphere(p, 1);
            }
        }
    }
}
