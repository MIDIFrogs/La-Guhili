using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Letter : MonoBehaviour
{
    public char letter;
    public bool isTargetLetter = false;
    public TMP_Text textMeshPro;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameController gc = FindObjectOfType<GameController>();
        if (gc != null)
        {
            gc.OnLetterCollected(this); // передаём сам объект буквы
        }

        Destroy(gameObject);
    }

    public void LightOn()
    {
        textMeshPro.color = Color.yellow;
        Debug.Log("highlight on" + " " + letter);
    }

    public void LightOff() 
    { 
        textMeshPro.color = Color.white;
        Debug.Log("highlight off" + " " + letter);
    }
}
