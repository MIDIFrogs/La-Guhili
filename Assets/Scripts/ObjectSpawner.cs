using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject letterPrefab;     // Префаб с TMP_Text и компонентом Letter
    public GameObject obstaclePrefab;   // Префаб препятствия
    public Transform player;            // Ссылка на игрока
    public float despawnDistance = 5f;

    [Header("Spawn Settings")]
    public float spawnDistance = 25f;   // На каком расстоянии впереди игрока спавнится объект
    public float spawnInterval = 2f;    // Интервал между спавнами
    public float rowOffset = 3.5f;      // Расстояние между рядами (по оси X)
    public int maxObjectsPerRow = 3;    // Чтобы не было наложений

    private float[] rows;               // Координаты рядов по X
    private GameController gc;

    public WordManager wordManager;

    public List<Letter> letterList = new List<Letter>();

    private void Start()
    {
        rows = new float[] { -rowOffset, 0, rowOffset };
        gc = FindObjectOfType<GameController>();
    }

    /// <summary>
    /// Запускает бесконечный цикл спавна объектов
    /// </summary>
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
        // Дестрой объектов, которые остались позади игрока
        for (int j = letterList.Count - 1; j >= 0; j--)
        {
            if (letterList[j] == null)
            {
                letterList.RemoveAt(j);
                continue;
            }

            if (player.position.z - letterList[j].transform.position.z > despawnDistance)
            {
                if(gc.ult.HighlightLetter == letterList[j])
                {
                    gc.UltHighLight();
                }
                Destroy(letterList[j].gameObject);
                letterList.RemoveAt(j);
                
            }
        }
    }

    /// <summary>
    /// Один цикл спавна: проверяет все ряды и спавнит объекты с шансом
    /// </summary>
    private void TrySpawnObjectsBatch()
    {
        foreach (float rowX in rows)
        {
            float chance = Random.value;

            if (chance < 0.3f)
            {
                // 30% шанс — буква
                Vector3 pos = new Vector3(rowX, player.position.y, player.position.z + spawnDistance);
                SpawnLetter(pos);
            }
            else if (chance < 0.5f)
            {
                // 20% шанс — препятствие
                Vector3 pos = new Vector3(rowX, player.position.y, player.position.z + spawnDistance);
                Instantiate(obstaclePrefab, pos, Quaternion.identity);
                Debug.Log($"🚧 Спавн препятствия в ряду {rowX}");
            }
            // 50% шанс — ничего не спавним
        }
    }

    /// <summary>
    /// Создает букву на указанной позиции
    /// </summary>
    private void SpawnLetter(Vector3 pos)
    {
        GameObject go = Instantiate(letterPrefab, pos, Quaternion.identity);

        // Находим TMP-текст внутри префаба и задаем букву
        TMP_Text txt = go.GetComponentInChildren<TMP_Text>();
        int chance = Random.Range(1, 100);
        char c = ' ';
        if (chance <= 60)
        {
            c = gc.GetNextLetter();
        }
        else
        {
            c = GetRandomRussianLetter();
        }
        txt.text = c.ToString();

        // Записываем в сам компонент Letter
        Letter letter = go.GetComponent<Letter>();
        if (letter != null)
        {
            letter.letter = c;
            letterList.Add(letter);
        }

        Debug.Log($"🔤 Спавн буквы '{c}' в позиции {pos}");
    }

    /// <summary>
    /// Возвращает случайную букву из русского алфавита
    /// </summary>
    private char GetRandomRussianLetter()
    {
        const string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        return alphabet[Random.Range(0, alphabet.Length)];
    }
}
