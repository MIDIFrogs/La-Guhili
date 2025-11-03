using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    [Header("Словарь")]
    public TextAsset dictionaryFile;

    [Header("Частотность букв (русские)")]
    public Dictionary<char, float> letterWeights = new Dictionary<char, float>
    {
        {'а',0.062f},{'б',0.014f},{'в',0.045f},{'г',0.017f},{'д',0.025f},{'е',0.072f},
        {'ё',0.0004f},{'ж',0.009f},{'з',0.016f},{'и',0.062f},{'й',0.010f},{'к',0.028f},
        {'л',0.035f},{'м',0.026f},{'н',0.053f},{'о',0.09f},{'п',0.023f},{'р',0.041f},
        {'с',0.045f},{'т',0.053f},{'у',0.021f},{'ф',0.002f},{'х',0.009f},{'ц',0.004f},
        {'ч',0.012f},{'ш',0.007f},{'щ',0.003f},{'ъ',0.0004f},{'ы',0.018f},{'ь',0.017f},
        {'э',0.003f},{'ю',0.006f},{'я',0.021f}
    };

    private HashSet<string> words = new HashSet<string>(); 
    private TrieNode root = new TrieNode(); 


    private class TrieNode
    {
        public Dictionary<char, TrieNode> children = new Dictionary<char, TrieNode>();
        public bool isWord = false;
    }


    public void LoadWords()
    {
        if (dictionaryFile == null)
        {
            Debug.LogError("WordManager: словарь не назначен!");
            return;
        }

        string[] lines = dictionaryFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int count = 0;
        foreach (string line in lines)
        {
            string w = line.Trim().ToLower();
            if (string.IsNullOrEmpty(w)) continue;
            words.Add(w);
            AddWordToTrie(w);
            count++;
        }
        Debug.Log($"WordManager: загружено {count} слов, построено дерево префиксов.");
    }

    private void AddWordToTrie(string word)
    {
        TrieNode node = root;
        foreach (char c in word)
        {
            if (!node.children.ContainsKey(c)) node.children[c] = new TrieNode();
            node = node.children[c];
        }
        node.isWord = true;
    }

    public bool IsWord(string word)
    {
        word = word.ToLower();
        TrieNode node = root;
        foreach (char c in word)
        {
            if (!node.children.ContainsKey(c)) return false;
            node = node.children[c];
        }
        return node.isWord;
    }

    public bool IsPossiblePrefix(string prefix)
    {
        prefix = prefix.ToLower();
        TrieNode node = root;
        foreach (char c in prefix)
        {
            if (!node.children.ContainsKey(c)) return false;
            node = node.children[c];
        }
        return true;
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

    public string FindReachableWord(string currentCollected, List<char> availableLetters)
    {
        currentCollected = currentCollected.ToLower();
        List<char> pool = new List<char>(availableLetters);
        pool.AddRange(currentCollected);

        List<string> candidates = words.Where(w => {
            var temp = new List<char>(pool);
            foreach (char c in w)
            {
                if (temp.Contains(c)) temp.Remove(c);
                else return false;
            }
            return true;
        }).ToList();

        if (candidates.Count == 0) return null;

        string chosen = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        Debug.Log($"WordManager: найдено достижимое слово для прозрения -> {chosen}");
        return chosen;
    }
}
