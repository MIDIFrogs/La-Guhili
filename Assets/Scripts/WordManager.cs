using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WordManager : MonoBehaviour
{

    void Start()
    {
        LoadWordsFromFile("Assets/words.txt");
    }

    private TrieNode root = new TrieNode();
    private List<string> allWords = new List<string>();

    public void LoadWordsFromFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Words file not found: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);
        foreach (string line in lines)
        {
            string word = line.Trim().ToUpper();
            if (string.IsNullOrEmpty(word)) continue;
            allWords.Add(word);
            AddWordToTrie(word);
        }

        Debug.Log("Loaded words: " + allWords.Count);
    }

    private void AddWordToTrie(string word)
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

    public string GetRandomWord()
    {
        if (allWords.Count == 0) return null;
        int index = Random.Range(0, allWords.Count);
        return allWords[index];
    }

    public bool IsPossibleWord(string prefix)
    {
        TrieNode node = root;
        foreach (char c in prefix)
        {
            if (!node.children.ContainsKey(c))
                return false;
            node = node.children[c];
        }
        return true;
    }

    public bool IsExactWord(string word)
    {
        TrieNode node = root;
        foreach (char c in word)
        {
            if (!node.children.ContainsKey(c))
                return false;
            node = node.children[c];
        }
        return node.isWord;
    }
}

public class TrieNode
{
    public Dictionary<char, TrieNode> children = new Dictionary<char, TrieNode>();
    public bool isWord = false;
}
