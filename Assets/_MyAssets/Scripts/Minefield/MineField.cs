using System.Collections.Generic;
using UnityEngine;

public class MineField : MonoBehaviour
{
    [SerializeField] private BoxCollider minefieldCollider;
    [SerializeField] private GameObject hazardObject;
    [SerializeField] private float hazardScale = 1f;
    [SerializeField] [Range(2f, 10f)] public float radius = 5;
    [SerializeField] private float xWidth = 100f;
    [SerializeField] private float yHeight = 10f;
    [SerializeField] private float zDepth = 100f;
    [SerializeField] private int rejectionSamples = 30;

    private List<Vector2> _points;

    void OnValidate()
    {
        Vector3 colliderSize = new Vector3(xWidth, yHeight, zDepth);

        minefieldCollider.size = colliderSize;
        minefieldCollider.center = colliderSize / 2;

        _points = PoissonDiscSampling.GeneratePoints(radius, new Vector2(xWidth, zDepth), rejectionSamples);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector2 point in _points)
        {
            Gizmos.DrawWireSphere(new Vector3(point.x, yHeight / 2, point.y) + transform.position, 1);
        }
    }

    private void Start()
    {
        foreach (Vector2 point in _points) SpawnHazardObject(point);
    }

    private void SpawnHazardObject(Vector2 point)
    {
        var position = new Vector3(
            point.x + transform.position.x,
            Random.Range(transform.position.y, transform.position.y + yHeight),
            point.y + transform.position.z);

        GameObject go = Instantiate(hazardObject, position, Random.rotationUniform, transform);
        go.transform.localScale *= hazardScale;
    }
}