using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public WordManager wordManager;
    public ObjectSpawner spawner;
    public FrogController player;

    [Header("Настройки")]
    public float spawnInterval = 2f;

    private string currentTargetWord = "";
    private string currentCollected = "";
    private int hp = 3;

    void Start()
    {
        StartNewWord();
    }

    void StartNewWord()
    {
        currentTargetWord = wordManager.GetRandomWord();
        currentCollected = "";
        Debug.Log("🎯 Новое слово: " + currentTargetWord);
        StartCoroutine(SpawnWordRoutine());
    }

    IEnumerator SpawnWordRoutine()
    {
        foreach (char letter in currentTargetWord)
        {
            int lane = Random.Range(0, 3);
            spawner.SpawnLetter(letter, lane);

            // шум в других рядах
            for (int i = 0; i < 3; i++)
            {
                if (i != lane && Random.value < 0.5f) spawner.SpawnNoise();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void OnLetterCollected(Letter letterObj)
    {
        char letter = letterObj.letter;

        currentCollected += char.ToUpper(letter);
        Debug.Log($"🔡 Подобрана буква: {letter} (текущее слово: {currentCollected})");

        // Проверка префикса: есть ли хотя бы одно слово, которое начинается с currentCollected
        if (!wordManager.IsPossibleWord(currentCollected))
        {
            LoseHP("невозможно продолжить слово");
            currentCollected = "";
            Debug.Log("📝 Текущее слово сброшено, так как префикс невозможен");
        }
        else
        {
            // Можно подсчитать очки за букву, если нужно
            Debug.Log($"✅ Слово всё ещё возможно: {currentCollected}");
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmWord();
        }
    }

    void ConfirmWord()
    {
        if (wordManager.IsExactWord(currentCollected))
        {
            Debug.Log("✅ Слово собрано! Очки начислены: " + currentCollected);
            StopAllCoroutines();
            StartNewWord();
        }
        else
        {
            LoseHP("неверное слово");
            currentCollected = "";
            StopAllCoroutines();
            StartNewWord();
        }
    }

    void LoseHP(string reason)
    {
        hp--;
        Debug.Log("❌ Потеря HP: " + reason + " (" + hp + "/3)");
        if (hp <= 0)
        {
            Debug.Log("💀 Игра окончена!");
            StopAllCoroutines();
            // Можно добавить логику конца игры здесь
        }
    }
}
