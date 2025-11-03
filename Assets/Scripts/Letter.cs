using UnityEngine;

public class Letter : MonoBehaviour
{
    private char currentChar;
    private TextMesh text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMesh>();
    }

    public void SetLetter(char c)
    {
        currentChar = c;
        if (text != null) text.text = c.ToString();
    }

    public char GetChar() => currentChar;

    public void Highlight(bool on)
    {
        var r = GetComponentInChildren<MeshRenderer>();
        if (r != null) r.material.color = on ? Color.yellow : Color.white;
    }

    private void OnTriggerEnter(Collider other)
    {
        var gc = other.GetComponent<GameController>();
        if (gc != null)
        {
            gc.OnLetterCollected(currentChar);
            Destroy(gameObject);
        }
    }
}
