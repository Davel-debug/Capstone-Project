using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    public Button nextLevelButton;
    public Button menuButton;

    void Start()
    {
       
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        // Se ci sono altri livelli, mostra il pulsante "Prossimo livello"
        if (currentIndex + 1 < totalScenes)
        {
            nextLevelButton.gameObject.SetActive(true);
            nextLevelButton.onClick.AddListener(LoadNextLevel);
        }
        else
        {
            nextLevelButton.gameObject.SetActive(false); // Nascondi se sei all'ultimo
        }

        menuButton.onClick.AddListener(ReturnToMenu);
    }

    void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
