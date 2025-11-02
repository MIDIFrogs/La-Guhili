using System.Collections;
using UnityEngine;
using TMPro;

public class ObjectSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject letterPrefab;     // Префаб с TMP_Text и компонентом Letter
    public GameObject obstaclePrefab;   // Префаб препятствия
    public Transform player;            // Ссылка на игрока

    [Header("Spawn Settings")]
    public float spawnDistance = 25f;   // На каком расстоянии впереди игрока спавнится объект
    public float spawnInterval = 2f;    // Интервал между спавнами
    public float rowOffset = 3.5f;      // Расстояние между рядами (по оси X)
    public int maxObjectsPerRow = 3;    // Чтобы не было наложений

    private float[] rows;               // Координаты рядов по X
    private GameController gc;

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
        char c = GetRandomRussianLetter();
        txt.text = c.ToString();

        // Записываем в сам компонент Letter
        Letter letter = go.GetComponent<Letter>();
        if (letter != null)
        {
            letter.letter = c;
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
