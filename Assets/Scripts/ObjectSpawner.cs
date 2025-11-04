using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject obstaclePrefab1;
    public GameObject obstaclePrefab2;
    public GameObject obstaclePrefab3;
    public GameObject obstaclePrefab4;
    public float despawnDistance = 5f;

    [Header("Spawn Settings")]
    public float spawnDistance = 25f;
    public float spawnInterval = 2f;
    public float rowOffset = 3.5f;

    [Header("Height Settings")]
    public float lettersY = 0.5f;
    public float obstaclesY = 0.2f;

    [Header("Letter Prefabs")]
    [Tooltip("Список всех 3D-префабов букв в порядке алфавита.")]
    public List<GameObject> letterPrefabs = new List<GameObject>();

    private float[] rows;
    private GameController gc;

    public WordManager wordManager;
    [HideInInspector] public List<Letter> letterList = new List<Letter>();

    private const string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

    private void Start()
    {
        rows = new float[] { -rowOffset, 0, rowOffset };
        gc = FindObjectOfType<GameController>();
    }

    public IEnumerator StartSpawning()
    {
        Debug.Log("🌱 Начинаем спавн объектов...");
        while (true)
        {
            TrySpawnObjectsBatch();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void Update()
    {
        for (int j = letterList.Count - 1; j >= 0; j--)
        {
            if (letterList[j] == null)
            {
                letterList.RemoveAt(j);
                continue;
            }

            if (player.position.z - letterList[j].transform.position.z > despawnDistance)
            {
                if (gc.ult.HighlightLetter == letterList[j])
                    gc.UltHighLight();

                if (letterList[j].gameObject.scene.rootCount != 0)
                    Destroy(letterList[j].gameObject);

                letterList.RemoveAt(j);
            }
        }
    }

    private void TrySpawnObjectsBatch()
    {
        foreach (float rowX in rows)
        {
            float chance = Random.value;

            if (chance < 0.6f)
            {
                Vector3 pos = new Vector3(rowX, lettersY, player.position.z + spawnDistance);
                SpawnLetter(pos);
            }
            else if (chance < 0.8f)
            {
                Vector3 pos = new Vector3(rowX, obstaclesY, player.position.z + spawnDistance);
                var obstaclePrefab = obstaclePrefab1;
                var r = Random.Range(1, 4);
                if (r == 1)
                {
                    obstaclePrefab = obstaclePrefab1;
                }

                else if (r == 2)
                {
                    obstaclePrefab = obstaclePrefab2;
                }

                else if(r == 3)
                {
                    obstaclePrefab = obstaclePrefab3;
                }

                else if(r == 4)
                {
                    obstaclePrefab = obstaclePrefab4;
                }

                Instantiate(obstaclePrefab, pos, obstaclePrefab.transform.rotation);
                Debug.Log($"🚧 Спавн препятствия в ряду {rowX}");
            }
        }
    }

    private void SpawnLetter(Vector3 pos)
    {
        if (letterPrefabs == null || letterPrefabs.Count == 0)
        {
            Debug.LogWarning("⚠️ Не заданы префабы букв в инспекторе!");
            return;
        }

        char c = Random.value <= 0.6f ? gc.GetNextLetter() : GetRandomRussianLetter();
        GameObject prefab = GetLetterPrefab(c);

        if (prefab == null)
        {
            Debug.LogWarning($"❌ Префаб для буквы '{c}' не найден!");
            return;
        }

        GameObject go = Instantiate(prefab, pos, prefab.transform.rotation);
        go.name = prefab.name + "_Instance";

        Letter letter = go.GetComponent<Letter>();
        if (letter != null)
        {
            letter.letter = c;
            letterList.Add(letter);
        }

        Debug.Log($"🔤 Спавн 3D-буквы '{c}' в позиции {pos}");
    }

    private GameObject GetLetterPrefab(char c)
    {
        int index = alphabet.IndexOf(c);
        if (index < 0 || index >= letterPrefabs.Count)
            return null;

        return letterPrefabs[index];
    }

    private char GetRandomRussianLetter()
    {
        return alphabet[Random.Range(0, alphabet.Length)];
    }
}
