using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isPlayerDead = false;

    private SceneChanger sceneChanger;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            CursorManager.UnlockCursor();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset stato di morte
        isPlayerDead = false;

        // Trova lo SceneChanger nella scena
        sceneChanger = FindObjectOfType<SceneChanger>();

        // Gestione AI
        if (scene.name.Contains("Level"))
        {
            if (AIManager.Instance != null)
                AIManager.Instance.activeTracking = true;
        }
        else
        {
            if (AIManager.Instance != null)
                AIManager.Instance.activeTracking = false;
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

        StartCoroutine(LoadGameOverDelayed(delay));
    }

    private IEnumerator LoadGameOverDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (sceneChanger != null)
            sceneChanger.LoadGameOver();
        else
            Debug.LogWarning("[GameManager] Nessuno SceneChanger trovato nella scena!");
    }

    public void LoadMainMenu() => sceneChanger?.LoadMainMenu();
    public void LoadLevel(int levelIndex) => sceneChanger?.LoadLevel(levelIndex);
    public void LoadNextLevel() => sceneChanger?.LoadNextLevel();
    public void LoadVictoryScreen() => sceneChanger?.LoadVictoryScreen();
    public void LoadHowToPlay() => sceneChanger?.LoadHowToPlay();
    public void RestartLevel() => sceneChanger?.RestartLevel();
    public void QuitGame() => sceneChanger?.QuitGame();
}
