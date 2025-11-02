using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public Transform player;
    public float spawnDistance = 30f;   // как далеко спавн от игрока по Z
    public float laneDistance = 2f;     // расстояние между рядами
    public float waterHeight = 0f;      // высота уровня воды
    public float objectLifetime = 12f;

    [Header("Префабы")]
    public GameObject letterPrefab3D;   // 3D TextMeshPro префаб для букв
    public GameObject obstaclePrefab;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private System.Random rnd = new System.Random();

    // ----------------------------------
    // Вспомогательный метод: позиция ряда
    private Vector3 GetLanePosition(int laneIndex)
    {
        float x = (laneIndex - 1) * laneDistance; // 0:center, -1:left, +1:right
        float z = player.position.z + spawnDistance;
        return new Vector3(x, waterHeight, z);
    }

    // ----------------------------------
    // Спавн конкретной буквы в указанном ряду
    public GameObject SpawnLetter(char letter, int laneIndex)
    {
        Vector3 spawnPos = GetLanePosition(laneIndex);
        GameObject obj = Instantiate(letterPrefab3D, spawnPos, Quaternion.identity);

        // Меняем текст TMP на нужную букву
        TMP_Text tmp = obj.GetComponent<TMP_Text>();
        if (tmp != null)
            tmp.text = letter.ToString();

        spawnedObjects.Add(obj);
        Destroy(obj, objectLifetime); // удаление через время
        return obj;
    }

    // ----------------------------------
    // Спавн "шума" с вероятностью (рандомная буква или препятствие)
    public void SpawnNoise()
    {
        float chance = Random.value;
        if (chance < 0.3f) // 30% буква
        {
            int lane = rnd.Next(0, 3);
            char letter = GetRandomLetter();
            SpawnLetter(letter, lane);
        }
        else if (chance < 0.5f) // 20% препятствие
        {
            int lane = rnd.Next(0, 3);
            Vector3 spawnPos = GetLanePosition(lane);
            GameObject obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
            spawnedObjects.Add(obj);
            Destroy(obj, objectLifetime);
        }
        // 50% ничего не спавнится
    }

    // ----------------------------------
    // Получение случайной буквы (русская или латинская)
    private char GetRandomLetter()
    {
        // Пример: русские буквы А-Я
        string letters = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        int index = rnd.Next(0, letters.Length);
        return letters[index];
    }

    // ----------------------------------
    void Update()
    {
        // Удаление объектов, которые ушли за игрока
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
