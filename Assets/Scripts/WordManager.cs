using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    private TrieNode root;
    private HashSet<string> allWords = new HashSet<string>();
    private System.Random rnd = new System.Random();

    public Dictionary<char, float> letterWeights = new Dictionary<char, float>
    {
        {'а',0.062f},{'б',0.014f},{'в',0.045f},{'г',0.017f},{'д',0.025f},{'е',0.072f},
        {'ё',0.0004f},{'ж',0.009f},{'з',0.016f},{'и',0.062f},{'й',0.010f},{'к',0.028f},
        {'л',0.035f},{'м',0.026f},{'н',0.053f},{'о',0.09f},{'п',0.023f},{'р',0.041f},
        {'с',0.045f},{'т',0.053f},{'у',0.021f},{'ф',0.002f},{'х',0.009f},{'ц',0.004f},
        {'ч',0.012f},{'ш',0.007f},{'щ',0.003f},{'ъ',0.0004f},{'ы',0.018f},{'ь',0.017f},
        {'э',0.003f},{'ю',0.006f},{'я',0.021f}
    };

    private void Awake()
    {
        LoadWords();
    }

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

    public bool isChildInPrefix(string prefix, char letter)
    {
        TrieNode node = root;
        foreach (char c in prefix)
        {
            if (!node.children.ContainsKey(c))
                return false;
            node = node.children[c];
        }
        return node.children.ContainsKey(letter);
    }

    public char GetNextLetter(string prefix)
    {
        prefix = prefix.ToLower();
        TrieNode node = root;
        foreach (char c in prefix)
        {
            if (!node.children.ContainsKey(c)) return GetRandomLetterByFrequency(); // мусор
            node = node.children[c];
        }

        List<char> options = node.children.Keys.ToList();
        if (options.Count == 0) return GetRandomLetterByFrequency();
        // рандомно среди возможных букв
        char chosen = options[UnityEngine.Random.Range(0, options.Count)];
        Debug.Log($"WordManager: следующая буква для префикса '{prefix}' -> '{chosen}'");
        return chosen;
    }

    public char GetRandomLetterByFrequency()
    {
        float r = UnityEngine.Random.value;
        float cumulative = 0f;
        foreach (var kvp in letterWeights)
        {
            cumulative += kvp.Value;
            if (r <= cumulative) return kvp.Key;
        }
        return 'а';
    }

    // Вложенный класс узла дерева
    private class TrieNode
    {
        public Dictionary<char, TrieNode> children = new Dictionary<char, TrieNode>();
        public bool isWord = false;
    }
}
