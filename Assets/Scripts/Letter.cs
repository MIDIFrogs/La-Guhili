using UnityEngine;

public class Letter : MonoBehaviour
{
    public char letter; // задаётся при спавне через TMP

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameController gc = FindObjectOfType<GameController>();
            if (gc != null)
            {
                gc.OnLetterCollected(this); // передаём сам Letter
            }

            Destroy(gameObject); // удаляем букву после сбора
        }
    }
}