
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isPlayerDead = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ogni volta che carichi una scena, resetta lo stato di morte
        isPlayerDead = false;

        // Riattiva il controller del player se presente
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
                controller.enabled = true;
        }
    }
    public void OnPlayerDeath(float delay = 2f)
    {
        if (isPlayerDead) return;

        isPlayerDead = true;
        Debug.Log("[GameManager] Player morto. Attendo " + delay + " secondi...");

        // Blocca input del player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
                controller.enabled = false;
        }

        // Avvia coroutine per caricare il GameOver dopo l’animazione
        StartCoroutine(LoadGameOverDelayed(delay));
    }

    private IEnumerator LoadGameOverDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadGameOver();
    }


    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextScene);
        else
            LoadVictoryScreen();
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void LoadVictoryScreen()
    {
        SceneManager.LoadScene("Victory");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
