using UnityEngine;
using TMPro; // если используешь TextMeshPro

public class WordDisplayUI : MonoBehaviour
{
    [Header("UI-компонент")]
    public TMP_Text wordText; 

    /// <summary>
    /// Обновляет отображение собираемого слова
    /// </summary>
    public void SetWord(string currentWord)
    {
        wordText.text = currentWord;
    }

    /// <summary>
    /// Очистить отображение
    /// </summary>
    public void Clear()
    {
        wordText.text = "";
    }
}
