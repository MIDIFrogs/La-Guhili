using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuButtonManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button oButton;
    [SerializeField] private Button exitButton;
    


    private void Start()
    {
        playButton.onClick.AddListener(OnPlayPressed);
        exitButton.onClick.AddListener(OnExitPressed);
        oButton.onClick.AddListener(OnOButtonPressed);
    }





    private void OnPlayPressed()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnOButtonPressed()
    {
        SceneManager.LoadScene("ObScene");
    }


    private void OnExitPressed()
    {
        Application.Quit();
        Debug.Log("Exiting game");
    }

}







