using UnityEngine;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startTime = 60f; // tempo in secondi

    private float timeLeft;
    private UIController uiController;
    private LifeController lifeController;
    private bool isRunning = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset timer e riferimenti ogni volta che cambia scena
        timeLeft = startTime;
        isRunning = true;

        uiController = FindObjectOfType<UIController>();
        lifeController = FindObjectOfType<LifeController>();
    }

    void Update()
    {
        if (!isRunning) return;

        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Max(timeLeft, 0);

        if (uiController != null)
            uiController.UpdateTimer(timeLeft);

        if (timeLeft <= 0 && lifeController != null)
        {
            isRunning = false;
            lifeController.Die();
        }
    }
}
