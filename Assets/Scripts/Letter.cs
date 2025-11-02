using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Letter : MonoBehaviour
{
    public char letter;
    public bool isTargetLetter = false;

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
}
