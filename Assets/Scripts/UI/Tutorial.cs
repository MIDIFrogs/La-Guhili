using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] Sprite photo;
    [SerializeField] Image targetImage;

    private int rightClickCount = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // ÏÊÌ
        {
            rightClickCount++;

            if (targetImage != null && photo != null)
            {
                targetImage.sprite = photo;
            }

            if (rightClickCount >= 2)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}