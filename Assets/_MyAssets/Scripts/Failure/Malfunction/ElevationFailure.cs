using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ElevationFailure : Malfunction
{
    public Terrain terrain;
    public float elevationLimit = 100;

    private float elevation;
    private MalfunctionTrigger trigger;

    public override void Enter(MalfunctionTrigger trigger = null)
    {
        base.Enter();
        system.physicsSystem.TurnOff();
        this.trigger = trigger;
    }

    public override void Exit()
    {
        base.Exit();
        system.Failure(system.engineFailure);
    }

    public override bool IsFixed()
    {
        return elevation < elevationLimit && (!trigger || !trigger.triggered);
    }

    public override void Update()
    {
        base.Update();

        var t = getHeight(system.physicsSystem.transform.position);

        elevation = system.physicsSystem.transform.position.y - t;

        if (!Enabled && elevation > elevationLimit)
        {
            system.Failure(this);
        }
    }

    private float getHeight(Vector3 pos)
    {
        foreach (var terrain in Terrain.activeTerrains)
        {
            if (IsPointInTerrain(pos, terrain))
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
}
