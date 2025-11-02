using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("References")]
    public ObjectSpawner spawner;
    public TMP_Text scoreText;
    public TMP_Text hpText;
    public TMP_Text collectedText;
    public TMP_Text usedWordsText;

    [Header("Settings")]
    public int maxHP = 3;
    public string wordsFilePath = "Assets/words.txt";

    private WordManager wordManager;
    private string currentCollected = "";
    private HashSet<string> usedWords = new HashSet<string>();
    private int hp;
    private int score;
    private bool gameOver = false;

    private void Start()
    {
        hp = maxHP;
        score = 0;

        wordManager = new WordManager();
        wordManager.LoadWords();

        UpdateHP();
        UpdateScore();
        UpdateCollected();
        UpdateUsedWords();

        Debug.Log("🎮 Игра запущена. Словарь загружен, буквы будут спавниться.");

        if (spawner != null)
            StartCoroutine(spawner.StartSpawning());
        else
            Debug.LogWarning("⚠️ Не назначен ObjectSpawnerTMP!");
    }

    private void Update()
    {
        if (gameOver) return;

        // Проверяем нажатие пробела — подтверждение слова
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentCollected.Length == 0) return;

            if (wordManager.IsFullWord(currentCollected))
            {
                int gained = CalculateScore(currentCollected);
                score += gained;
                usedWords.Add(currentCollected);

                Debug.Log($"🏆 Собрано слово '{currentCollected}' (+{gained} очков)");
                currentCollected = "";

                UpdateUsedWords();
                UpdateScore();
                UpdateCollected();
            }
            else
            {
                LoseHP("❌ Слова не существует!");
                currentCollected = "";
                UpdateCollected();
            }
        }
    }

    public void OnLetterCollected(Letter letterObj)
    {
        char letter = letterObj.letter;
        currentCollected += char.ToUpper(letter);

        Debug.Log($"🔠 Подобрана буква: {letter}, текущее слово: {currentCollected}");

        // Проверяем, есть ли слова с таким префиксом
        if (!wordManager.IsPossibleWord(currentCollected))
        {
            LoseHP("⚠️ Префикс невозможен!");
            currentCollected = "";
        }

        UpdateCollected();
    }

    public void OnObstacleHit()
    {
        LoseHP("💥 Столкновение с препятствием!");
    }

    private void LoseHP(string reason)
    {
        if (gameOver) return;

        hp--;
        Debug.Log($"{reason} Осталось ❤️ {hp}");

        UpdateHP();

        if (hp <= 0)
        {
            Debug.Log("☠️ Игра окончена!");
            gameOver = true;
            hpText.text = "💀";
        }
    }

    private int CalculateScore(string word)
    {
        int sum = 0;
        foreach (char c in word)
        {
            switch (c)
            {
                case 'Щ': sum += 10; break;
                case 'Ф':
                case 'Ъ':
                case 'Ы':
                case 'Э':
                case 'Ю': sum += 3; break;
                case 'Ж':
                case 'Ш':
                case 'Ч': sum += 2; break;
                default: sum += 1; break;
            }
        }

        if (word.Length >= 7) sum += 10;
        else if (word.Length >= 5) sum += 5;

        return sum;
    }

    private void UpdateHP()
    {
        string hearts = "";
        for (int i = 0; i < hp; i++) hearts += "❤️";
        hpText.text = hearts;
    }

    private void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    private void UpdateCollected()
    {
        collectedText.text = currentCollected;
    }

    private void UpdateUsedWords()
    {
        usedWordsText.text = string.Join("\n", usedWords);
    }
}
