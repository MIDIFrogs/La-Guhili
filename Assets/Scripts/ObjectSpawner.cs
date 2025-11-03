using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Игрок")]
    public Transform player;

    [Header("Префабы")]
    public GameObject letterPrefab;
    public GameObject obstaclePrefab;

    [Header("Ряды по X")]
    public float leftX = -3f;
    public float centerX = 0f;
    public float rightX = 3f;

    [Header("Настройки спавна")]
    public float spawnDistance = 20f;
    public float spawnOffsetY = 0.5f;
    public float spawnRandomOffset = 5f;
    public float despawnDistance = 5f;
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 3f;

    [Header("Вероятности")]
    [Range(0f, 1f)] public float spawnChance = 0.5f;
    [Range(0f, 1f)] public float letterChance = 0.65f;
    [Range(0f, 1f)] public float obstacleChance = 0.35f;

    [Header("WordManager")]
    public WordManager wordManager;

    private Dictionary<int, Stack<GameObject>> rowStacks = new Dictionary<int, Stack<GameObject>>();
    private Dictionary<int, float> lastSpawnTime = new Dictionary<int, float>();
    private List<GameObject> spawnedObjects = new List<GameObject>();
    char[] frequent = new char[] { 'а', 'о', 'е', 'и', 'н', 'т', 'с', 'р', 'в', 'л' };

    private void Start()
    {
        rowStacks[0] = new Stack<GameObject>();
        rowStacks[1] = new Stack<GameObject>();
        rowStacks[2] = new Stack<GameObject>();

        lastSpawnTime[0] = Time.time;
        lastSpawnTime[1] = Time.time;
        lastSpawnTime[2] = Time.time;

        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        // Дестрой объектов, которые остались позади игрока
        for (int j = spawnedObjects.Count - 1; j >= 0; j--)
        {
            if (spawnedObjects[j] == null)
            {
                spawnedObjects.RemoveAt(j);
                continue;
            }

            if (player.position.z - spawnedObjects[j].transform.position.z > despawnDistance)
            {
                Destroy(spawnedObjects[j]);
                spawnedObjects.RemoveAt(j);
            }
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            for (int row = 0; row < 3; row++)
            {
                if (Time.time - lastSpawnTime[row] >= Random.Range(minSpawnInterval, maxSpawnInterval))
                {
                    TrySpawn(row);
                    lastSpawnTime[row] = Time.time;
                }
            }
            yield return null;
        }
    }

    private void TrySpawn(int row)
    {
        if (Random.value > spawnChance) return;

        GameObject prefabToSpawn = null;
        char letterChar = ' ';

        float r = Random.value;
        if (r < letterChance)
        {
            prefabToSpawn = letterPrefab;
            letterChar = PickLetterForSpawn(); //мб
        }
        else if (r < letterChance + obstacleChance)
        {
            prefabToSpawn = obstaclePrefab;
        }
        else return;

        Vector3 pos = GetSpawnPosition(row);

        GameObject obj = Instantiate(prefabToSpawn, pos, Quaternion.identity);
        if (obj.TryGetComponent<Letter>(out Letter letterObj))
        {
            letterObj.SetLetter(letterChar);
        }
        spawnedObjects.Add(obj);
        rowStacks[row].Push(obj);

        if (prefabToSpawn == letterPrefab)
        {
            TextMeshPro tmp = obj.GetComponentInChildren<TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = letterChar.ToString();
                tmp.color = Color.white;
            }
        }

        Debug.Log($"Спавн {prefabToSpawn.name} в ряду {row} на позиции {pos}. Буква: {letterChar}");
    }

    private Vector3 GetSpawnPosition(int row)
    {
        float x = centerX;
        if (row == 0) x = leftX;
        else if (row == 2) x = rightX;

        float z = player.position.z + spawnDistance + Random.Range(0f, spawnRandomOffset);
        float y = spawnOffsetY;

        return new Vector3(x, y, z);
    }

    private char PickLetterForSpawn()
    {
        string prefix = GameController.Instance.GetCurrentPrefix();
        char nextLetter = wordManager.GetNextLetter(prefix);
        // с небольшой вероятностью даём случайную частую букву
        if (Random.value < 0.2f)
        {
            nextLetter = frequent[Random.Range(0, frequent.Length)];
        }
        return nextLetter;
    }

    public void HighlightLettersForWord(string word)
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj == null) continue;
            TextMeshPro tmp = obj.GetComponentInChildren<TextMeshPro>();
            if (tmp != null && word.Contains(tmp.text))
            {
                tmp.color = Color.yellow;
            }
        }
    }

    public List<char> GetVisibleLetters()
    {
        List<char> result = new List<char>();
        foreach (var obj in spawnedObjects)
        {
            if (obj == null) continue;
            TextMeshPro tmp = obj.GetComponentInChildren<TextMeshPro>();
            if (tmp != null && !string.IsNullOrEmpty(tmp.text))
            {
                result.Add(tmp.text[0]);
            }
        }
        return result;
    }
}
