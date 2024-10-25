using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class HeightIndicator : MonoBehaviour
{
    [SerializeField] private float height;

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        var pos = SceneView.currentDrawingSceneView.camera.transform.position;

        for (int y = -5; y < 5; y++)
        {
            for (int x = -5; x < 5; x++)
            {
                var p = pos + (Vector3.forward * x + Vector3.right * y) * 10f;
                p.y = getHeight(p);
                var h = p;
                h.y += height;
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(h, 1);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(h, p);
            }
        }
    }

    private float getHeight(Vector3 pos)
    {
        foreach (var terrain in Terrain.activeTerrains)
        {
            if(IsPointInTerrain(pos, terrain))
            {
                return terrain.SampleHeight(pos);
            }
        }
        return 0;
    }

    public bool IsPointInTerrain(Vector3 point, Terrain terrain)
    {
        Vector3 terrainPosition = terrain.transform.position;
        TerrainData terrainData = terrain.terrainData;
        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;

        bool withinXBounds = point.x >= terrainPosition.x && point.x <= terrainPosition.x + terrainWidth;
        bool withinZBounds = point.z >= terrainPosition.z && point.z <= terrainPosition.z + terrainLength;

        return withinXBounds && withinZBounds;
    }
#endif
}
