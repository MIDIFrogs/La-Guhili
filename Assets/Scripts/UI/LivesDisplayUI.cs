using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.ComponentModel;


public class LivesDisplayUI : MonoBehaviour
{
    [Header("Настройки сердечек")]
    public GameObject heartPrefab;
    public Transform displayContainer;
    public Sprite fullHeart;


    private List<Image> heartImages = new List<Image>();

    /// <summary>
    /// Инициализирует сердечки по количеству жизней
    /// </summary>
    public void InitHearts(int maxLives)
    {
        ClearOldHearts();
        
        CreateNewHearts(maxLives);

    }



    public void SetLives(int lives)
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            heartImages[i].enabled = i < lives;
        }
    }






    private void ClearOldHearts()
    {
        foreach (Transform child in displayContainer)
        {
            Destroy(child);
        }
        heartImages.Clear();
    }

    private void CreateNewHearts(int maxLives)
    {
        for (int i = 0; i < maxLives; i++)
        {
            GameObject heart = Instantiate(heartPrefab, displayContainer);
            Image img = heart.GetComponent<Image>();
            img.sprite = fullHeart;
            img.enabled = true;
            heartImages.Add(img);
        }
    }

}
