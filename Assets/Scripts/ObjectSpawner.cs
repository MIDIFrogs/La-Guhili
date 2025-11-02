using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public Transform player;
    public float spawnDistance = 30f;
    public float laneDistance = 2f;
    public float waterHeight = 0f;
    public float objectLifetime = 12f;
    public float spawnCheckRadius = 0.5f; // радиус проверки пересечений

    [Header("Префабы")]
    public GameObject letterPrefab3D;
    public GameObject obstaclePrefab;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private System.Random rnd = new System.Random();

    // ----------------------------------
    private Vector3 GetLanePosition(int laneIndex)
    {
        float x = (laneIndex - 1) * laneDistance;
        float z = player.position.z + spawnDistance;
        return new Vector3(x, waterHeight, z);
    }

    private bool IsPositionFree(Vector3 position)
    {
        // Проверяем только на буквы и препятствия
        Collider[] hits = Physics.OverlapSphere(position, spawnCheckRadius);
        foreach (var hit in hits)
        {
            // Игнорируем игрока, землю и другие ненужные коллайдеры
            if (hit.gameObject.CompareTag("Letter") || hit.gameObject.CompareTag("Obstacle"))
                return false;
        }
        return true;
    }


    public GameObject SpawnLetter(char letter, int laneIndex)
    {
        Vector3 spawnPos = GetLanePosition(laneIndex);

        // Проверка, свободна ли позиция
        if (!IsPositionFree(spawnPos))
        {
            Debug.Log($"⚠️ Спавн буквы {letter} отменён: место занято");
            return null;
        }

        GameObject obj = Instantiate(letterPrefab3D, spawnPos, Quaternion.identity);
        TMP_Text tmp = obj.GetComponent<TMP_Text>();
        if (tmp != null) tmp.text = letter.ToString();

        spawnedObjects.Add(obj);
        Destroy(obj, objectLifetime);

        Debug.Log($"🅰️ Спавн буквы {letter} в ряду {laneIndex} на позиции {spawnPos}");
        return obj;
    }

    public void SpawnNoise()
    {
        float chance = Random.value;
        if (chance < 0.3f) // буква
        {
            int lane = rnd.Next(0, 3);
            char letter = GetRandomLetter();
            SpawnLetter(letter, lane);
        }
        else if (chance < 0.5f) // препятствие
        {
            int lane = rnd.Next(0, 3);
            Vector3 spawnPos = GetLanePosition(lane);
            if (!IsPositionFree(spawnPos))
            {
                Debug.Log($"⚠️ Спавн препятствия отменён: место занято");
                return;
            }
            GameObject obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
            spawnedObjects.Add(obj);
            Destroy(obj, objectLifetime);
            Debug.Log($"🚧 Спавн препятствия в ряду {lane} на позиции {spawnPos}");
        }
    }

    private char GetRandomLetter()
    {
        string letters = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        int index = rnd.Next(0, letters.Length);
        return letters[index];
    }

    void Update()
    {
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
