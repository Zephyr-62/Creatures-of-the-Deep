using UnityEngine;

public class MineField : MonoBehaviour
{
    [SerializeField] private GameObject hazardObject;
    [SerializeField] private int spawnCount = 20;
    [SerializeField] private float yStartPosition = 5f;
    [SerializeField] private float xWidth = 100f;
    [SerializeField] private float yHeight = 10f;
    [SerializeField] private float zDepth = 100f;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, yStartPosition, transform.position.z);
        for (var i = 0; i < spawnCount; i++) SpawnHazardObject();
    }

    private void SpawnHazardObject()
    {
        var spawnPointX = Random.Range(transform.position.x - xWidth / 2, transform.position.x + xWidth / 2);
        var spawnPointY = Random.Range(transform.position.y, transform.position.y + yHeight);
        var spawnPointZ = Random.Range(transform.position.z - zDepth / 2, transform.position.z + zDepth / 2);

        Instantiate(hazardObject, new Vector3(spawnPointX, spawnPointY, spawnPointZ), Quaternion.identity, transform);
    }
}