using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";
    public string gameOverScene = "GameOver";
    public string victoryScene = "Victory";
    public string howToPlayScene = "HowToPlay";

    public void LoadMainMenu()
    {
        CursorManager.UnlockCursor();
        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadLevel(int levelIndex)
    {
        CursorManager.LockCursor();
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            CursorManager.LockCursor();
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            LoadVictoryScreen();
        }
    }

    public void LoadGameOver()
    {
        CursorManager.UnlockCursor();
        SceneManager.LoadScene(gameOverScene);
    }

    public void LoadVictoryScreen()
    {
        CursorManager.UnlockCursor();
        SceneManager.LoadScene(victoryScene);
    }

    public void LoadHowToPlay()
    {
        CursorManager.UnlockCursor();
        SceneManager.LoadScene(howToPlayScene);
    }

    public void RestartLevel()
    {
        CursorManager.LockCursor();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
