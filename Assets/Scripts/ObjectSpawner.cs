using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject letterPrefab;
    public GameObject obstaclePrefab;
    public Transform player;
    public float despawnDistance = 5f;

    [Header("Spawn Settings")]
    public float spawnDistance = 25f;
    public float spawnInterval = 2f;
    public float rowOffset = 3.5f;
    public int maxObjectsPerRow = 3;

    [Header("Height Settings")]
    public float lettersY = 0.5f;       // Фиксированная высота для букв
    public float obstaclesY = 0.2f;     // Фиксированная высота для препятствий

    private float[] rows;
    private GameController gc;

    public WordManager wordManager;
    public List<Letter> letterList = new List<Letter>();

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

            if (chance < 0.5f)
            {
                // 30% шанс — буква
                Vector3 pos = new Vector3(rowX, lettersY, player.position.z + spawnDistance);
                SpawnLetter(pos);
            }
            else if (chance < 0.7f)
            {
                // 20% шанс — препятствие
                Vector3 pos = new Vector3(rowX, obstaclesY, player.position.z + spawnDistance);
                Instantiate(obstaclePrefab, pos, obstaclePrefab.transform.rotation);
                Debug.Log($"🚧 Спавн препятствия в ряду {rowX}");
            }
            // 50% шанс — ничего не спавним
        }
    }

    private void SpawnLetter(Vector3 pos)
    {
        GameObject go = Instantiate(letterPrefab, pos, Quaternion.identity);
        TMP_Text txt = go.GetComponentInChildren<TMP_Text>();

        char c = Random.value <= 0.6f ? gc.GetNextLetter() : GetRandomRussianLetter();
        txt.text = c.ToString();

        Letter letter = go.GetComponent<Letter>();
        if (letter != null)
        {
            letter.letter = c;
            letterList.Add(letter);
        }

        Debug.Log($"🔤 Спавн буквы '{c}' в позиции {pos}");
    }

    private char GetRandomRussianLetter()
    {
        const string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        return alphabet[Random.Range(0, alphabet.Length)];
    }
}
