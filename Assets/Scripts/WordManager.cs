using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    private TrieNode root;
    private List<string> allWords = new List<string>();
    private System.Random rnd = new System.Random();

    private void Awake()
    {
        LoadWords();
    }

    /// <summary>
    /// Загружает слова из файла words.txt (в папке Resources)
    /// </summary>
    public void LoadWords()
    {
        root = new TrieNode();

        TextAsset textAsset = Resources.Load<TextAsset>("words");
        if (textAsset == null)
        {
            Debug.LogError("❌ Не найден файл words.txt в папке Resources!");
            return;
        }

        string[] lines = textAsset.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            string word = line.Trim().ToUpper();
            if (word.Length == 0) continue;

            InsertWord(word);
            allWords.Add(word);
        }

        Debug.Log($"📚 Загружено {allWords.Count} слов");
    }

    /// <summary>
    /// Вставка слова в префиксное дерево
    /// </summary>
    private void InsertWord(string word)
    {
        TrieNode node = root;
        foreach (char c in word)
        {
            if (!node.children.ContainsKey(c))
                node.children[c] = new TrieNode();

            node = node.children[c];
        }
        node.isWord = true;
    }

    /// <summary>
    /// Проверяет, может ли текущее собрание букв быть началом хотя бы одного слова
    /// </summary>
    public bool IsPossibleWord(string prefix)
    {
        if (string.IsNullOrEmpty(prefix)) return true;

        TrieNode node = root;
        foreach (char c in prefix)
        {
            if (!node.children.ContainsKey(c))
                return false;
            node = node.children[c];
        }

        return true;
    }

    /// <summary>
    /// Проверяет, существует ли слово целиком
    /// </summary>
    public bool IsFullWord(string word)
    {
        if (string.IsNullOrEmpty(word)) return false;

        TrieNode node = root;
        foreach (char c in word)
        {
            if (!node.children.ContainsKey(c))
                return false;
            node = node.children[c];
        }

        return node.isWord;
    }

    /// <summary>
    /// Возвращает случайное слово из списка
    /// </summary>
    public string GetRandomWord()
    {
        if (allWords.Count == 0) return null;
        return allWords[rnd.Next(allWords.Count)];
    }

    /// <summary>
    /// Возвращает слово, которое начинается с данного префикса (если есть)
    /// </summary>
    public string GetWordStartingWith(string prefix)
    {
        foreach (string w in allWords)
        {
            if (w.StartsWith(prefix))
                return w;
        }
        return null;
    }

    // Вложенный класс узла дерева
    private class TrieNode
    {
        public Dictionary<char, TrieNode> children = new Dictionary<char, TrieNode>();
        public bool isWord = false;
    }
}
