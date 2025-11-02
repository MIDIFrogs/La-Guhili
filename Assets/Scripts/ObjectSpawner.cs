using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public Transform player;
    public GameObject letterPrefab;
    public GameObject obstaclePrefab;

    public float spawnDistance = 30f;
    public float laneDistance = 2f;
    public float objectLifetime = 10f;
    public float waterHeight = 0f; // высота воды

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private Vector3 GetLanePosition(int laneIndex)
    {
        float x = (laneIndex - 1) * laneDistance;
        float z = player.position.z + spawnDistance;
        return new Vector3(x, waterHeight, z);
    }

    public GameObject SpawnOnLeftRow(GameObject prefab = null) => SpawnObject(prefab ?? letterPrefab, 0);
    public GameObject SpawnOnCenterRow(GameObject prefab = null) => SpawnObject(prefab ?? letterPrefab, 1);
    public GameObject SpawnOnRightRow(GameObject prefab = null) => SpawnObject(prefab ?? letterPrefab, 2);

    private GameObject SpawnObject(GameObject prefab, int laneIndex)
    {
        Vector3 spawnPos = GetLanePosition(laneIndex);
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
        spawnedObjects.Add(obj);
        Destroy(obj, objectLifetime);
        return obj;
    }

    void Update()
    {
        // автоудаление ушедших объектов
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] == null)
            {
                spawnedObjects.RemoveAt(i);
                continue;
            }

            if (spawnedObjects[i].transform.position.z < player.position.z - 10f)
            {
                Destroy(spawnedObjects[i]);
                spawnedObjects.RemoveAt(i);
            }
        }
    }
}
