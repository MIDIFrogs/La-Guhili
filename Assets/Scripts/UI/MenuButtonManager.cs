using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuButtonManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    


    private void Start()
    {
        playButton.onClick.AddListener(OnPlayPressed);
        exitButton.onClick.AddListener(OnExitPressed);
    }





    private void OnPlayPressed()
    {
        SceneManager.LoadScene("SampleScene");
    }


    private void OnExitPressed()
    {
        Application.Quit();
        Debug.Log("Exiting game");
    }

}







