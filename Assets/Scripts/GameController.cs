using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Игрок")]
    public Transform player;
    public int maxHP = 3;
    private int currentHP;

    [Header("UI")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI collectedLettersText;
    public TextMeshProUGUI usedWordsText;
    public GameObject startOverButton;

    [Header("Word System")]
    public WordManager wordManager;
    public ObjectSpawner spawner;

    [Header("Insight")]
    public float insightMax = 25f;
    public float insightCharge = 0f;
    public GameObject insightBarFront; 
    public GameObject insightPanelKnob; 

    private string currentCollected = "";
    private List<string> usedWords = new List<string>();
    private int score = 0;

    private void Awake()
    {
        Instance = this;
        currentHP = maxHP;
        UpdateHPUI();
        UpdateScoreUI();
        UpdateCollectedLettersUI();
        UpdateUsedWordsUI();
        startOverButton.SetActive(false);
        wordManager.LoadWords();
    }

    private void Update()
    {
        if (insightCharge < insightMax) insightCharge += Time.deltaTime;
        UpdateInsightUI();

        if (Input.GetKeyDown(KeyCode.F) && insightCharge >= insightMax)
        {
            ActivateInsight();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentCollected.Length > 0) ConfirmCurrentWord();
        }
    }

    public void OnLetterCollected(char letter)
    {
        currentCollected += letter;
        UpdateCollectedLettersUI();

        if (!wordManager.IsPossiblePrefix(currentCollected))
        {
            Debug.Log($"Неверный префикс: {currentCollected}. Минус 1 HP.");
            LoseHP(1);
            currentCollected = "";
            UpdateCollectedLettersUI();
        }
        else
        {
            Debug.Log($"Собран префикс: {currentCollected}");
        }
    }

    public void OnObstacleHit()
    {
        Debug.Log("Игрок столкнулся с препятствием. Минус 1 HP.");
        LoseHP(1);
    }

    private void ConfirmCurrentWord()
    {
        string collected = currentCollected.ToLower();
        if (wordManager.IsWord(collected))
        {
            if (!usedWords.Contains(collected)) usedWords.Add(collected);
            int points = CalculatePoints(collected);
            score += points;
            Debug.Log($"Подтверждено слово '{collected}', получено {points} очков.");
            UpdateScoreUI();
            UpdateUsedWordsUI();
        }
        else
        {
            Debug.Log($"Неверное слово '{collected}'. Минус 1 HP.");
            LoseHP(1);
        }
        currentCollected = "";
        UpdateCollectedLettersUI();
    }

    private int CalculatePoints(string word)
    {
        int points = 0;
        foreach (char c in word)
        {
            if (wordManager.letterWeights.TryGetValue(c, out float w))
            {
                if (w >= 0.04f) points += 1; // часто
                else if (w >= 0.01f) points += 2; // средние
                else points += 3; // редкие
            }
        }

        if (word.Length >= 3 && word.Length <= 4) points += 5;
        else if (word.Length >= 5 && word.Length <= 6) points += 7;
        else if (word.Length >= 7) points += 10;
        return points;
    }

    private void LoseHP(int amount)
    {
        currentHP -= amount;
        UpdateHPUI();
        if (currentHP <= 0) GameOver();
    }

    private void GameOver()
    {
        Debug.Log("Игра окончена.");
        startOverButton.SetActive(true);
        Time.timeScale = 0f; // пауза
        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (score > best) PlayerPrefs.SetInt("BestScore", score);
    }

    public void StartOver()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateHPUI()
    {
        if (hpText == null) return;
        string hearts = "";
        for (int i = 0; i < currentHP; i++) hearts += "❤️";
        hpText.text = hearts;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }

    private void UpdateCollectedLettersUI()
    {
        if (collectedLettersText != null) collectedLettersText.text = currentCollected;
    }

    private void UpdateUsedWordsUI()
    {
        if (usedWordsText != null) usedWordsText.text = string.Join("\n", usedWords);
    }

    private void UpdateInsightUI()
    {
        if (insightBarFront != null)
        {
            float scaleX = Mathf.Clamp01(insightCharge / insightMax);
            insightBarFront.transform.localScale = new Vector3(scaleX, 1f, 1f);
        }
        if (insightPanelKnob != null)
        {
            if (insightCharge >= insightMax) insightPanelKnob.SetActive(true);
            else insightPanelKnob.SetActive(false);
        }
    }

    private void ActivateInsight()
    {
        List<char> visible = spawner.GetVisibleLetters();
        string reachable = wordManager.FindReachableWord(currentCollected, visible);
        if (reachable != null) spawner.HighlightLettersForWord(reachable);
        Debug.Log($"Прозрение активировано! Цель: {reachable}");
        insightCharge = 0f;
    }

    public string GetCurrentPrefix()
    {
        return currentCollected;
    }
}
