using TMPro;
using UnityEngine;

public class Score: MonoBehaviour 
{
    public GameManager gameManager;
    public TMP_Text text;

    private void Update()
    {
        text.text = gameManager.GetScore().ToString();
    }
}
