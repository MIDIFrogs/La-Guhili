using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score = 0; // текущий счёт

    private const string ScoreKey = "score"; // ключ для PlayerPrefs

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Загружаем рекорд при старте
        score = PlayerPrefs.GetInt(ScoreKey, 0);
    }

    /// <summary>
    /// Получить текущий счёт
    /// </summary>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// Обновляет рекорд: если новый результат больше текущего, сохраняет его
    /// </summary>
    public void UpdateScore(int newScore)
    {
        if (newScore > score)
        {
            score = newScore;
            PlayerPrefs.SetInt(ScoreKey, score);
            PlayerPrefs.Save();
            Debug.Log($"🏆 Новый рекорд: {score}");
        }
        else
        {
            Debug.Log($"🎮 Результат {newScore} не превысил рекорд {score}");
        }
    }

    /// <summary>
    /// Сбросить текущий рекорд (если нужно)
    /// </summary>
    public void ResetScore()
    {
        score = 0;
        PlayerPrefs.DeleteKey(ScoreKey);
    }
}
