using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour
{
    [Header("References")]
    public ObjectSpawner spawner;
    public TMP_Text scoreText;
    public TMP_Text collectedText;
    public TMP_Text usedWordsText;
    public Ult ult;
    public Image imageF;
    public LivesDisplayUI livesUI;
    public Image panelLose; 
    public Image panelPause; 
    public TMP_Text recordText;
    public TMP_Text bestRecordText;
    public AudioManager audioManager;
    private GameManager gameManager = GameManager.Instance;
    



    public Button againButton;
    public Button btnNoPause;
    public Button btnNoLose;
    public Button btnCrossPause;
    public Button btnCrossLose;
    public Button btnYesPause;

    [Header("Settings")]
    public int maxHP = 3;
    public string wordsFilePath = "Assets/words.txt";

    private WordManager wordManager;
    private string currentCollected = "";
    private HashSet<string> usedWords = new HashSet<string>();
    private int hp;
    private int score;
    private bool gameOver = false;
    private bool isPaused = false;
    
    private void Start()
    {
        panelLose.gameObject.SetActive(false);
        panelPause.gameObject.SetActive(false);
        againButton.onClick.AddListener(RestartGame);
        btnNoPause.onClick.AddListener(GoToMenu);
        btnCrossLose.onClick.AddListener(GoToMenu);
        btnCrossPause.onClick.AddListener(Resume);
        btnNoLose.onClick.AddListener(GoToMenu);
        btnYesPause.onClick.AddListener(Resume);


        hp = maxHP;
        score = 0;
        livesUI.InitHearts(hp);
        livesUI.SetLives(hp);

        wordManager = new WordManager();
        wordManager.LoadWords();

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
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            Debug.Log("Space pressed");

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

        if (Input.GetKeyDown(KeyCode.Escape)){
            Pause();
        }

        if(ult.isUltNow && ult.HighlightLetter == null)
        {
            UltHighLight();
        }
    }

    public void OnLetterCollected(Letter letterObj)
    {
        audioManager.PlayBuble();
        char letter = letterObj.letter;
        currentCollected += char.ToUpper(letter);

        Debug.Log($"🔠 Подобрана буква: {letter}, текущее слово: {currentCollected}");

        // Проверяем, есть ли слова с таким префиксом
        if (!wordManager.IsPossibleWord(currentCollected))
        {
            LoseHP("⚠️ Префикс невозможен!");
            audioManager.PlayLetterWrong();
            currentCollected = "";
            ult.UltOff();
        }
        else
        {
            audioManager.PlayLetterCorrect();
        }
        UpdateCollected();
        UltHighLight(); 
        
    }

    public void UltHighLight() 
    { 
        var letters = spawner.letterList.Where(x => wordManager.isChildInPrefix(currentCollected, x.letter));
        ult.HighlightLetter = letters.OrderBy(x=>Random.value).FirstOrDefault();
    }

    public void OnObstacleHit()
    {
        LoseHP("💥 Столкновение с препятствием!" + gameOver);
        audioManager.PlayImpact();
    }

    private void LoseHP(string reason)
    {
        audioManager.PlayDamageTaken();
        if (gameOver) return;

        hp--;
        livesUI.SetLives(hp);
        Debug.Log($"{reason} Осталось W {hp}");

        if (hp <= 0)
        {
            audioManager.PlayLose();
            gameManager.UpdateScore(score);
            Debug.Log("☠️ Игра окончена!");
            gameOver = true;
            panelLose.gameObject.SetActive(true);
            recordText.text = "Текущий результат: " + score.ToString();
            bestRecordText.text = "Лучший результат: " + gameManager.GetScore().ToString();

            againButton.gameObject.SetActive(true);
            Time.timeScale = 0f;
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

    private void UpdateScore()
    {
        scoreText.text = "Очки: " + score.ToString();
    }

    private void UpdateCollected()
    {
        collectedText.text = currentCollected;
    }

    private void UpdateUsedWords()
    {
        usedWordsText.text = string.Join("\n", usedWords);
    }



    public void RestartGame() //кнопка
    {
        Time.timeScale = 1f; // Возвращаем время
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезагружаем текущую сцену
        hp = maxHP;
    }

    public char GetNextLetter()
    {
       return wordManager.GetNextLetter(currentCollected);
       
    }

    public void Pause()
    {
        if (isPaused) return; // чтобы не вызывать несколько раз подряд

        Time.timeScale = 0f; // останавливает игровое время
        isPaused = true;

        if (panelPause != null)
            panelPause.gameObject.SetActive(true);

        if (audioManager != null)
            audioManager.PlayPausedMusic();

        Debug.Log("⏸ Игра поставлена на паузу");
    }

    public void Resume()
    {
        panelPause.gameObject.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (panelLose != null)
            panelLose.gameObject.SetActive(false);

        if (audioManager != null)
            audioManager.PlayMusic();

        Debug.Log("▶ Игра возобновлена");
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
