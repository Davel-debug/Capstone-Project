using UnityEngine;
using TMPro;
using System;

public class TimerCountdown : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Durata del timer in secondi.")]
    public float startTime = 120f;

    [Tooltip("Testo TMP che mostra il tempo rimanente.")]
    public TextMeshProUGUI timerText;

    [Tooltip("Avvia automaticamente all'avvio della scena.")]
    public bool autoStart = true;

    [Tooltip("Nascondi il timer quando raggiunge 0.")]
    public bool hideOnZero = true;

    private float currentTime;
    private bool isRunning = false;

    public bool IsFinished { get; private set; } = false;

    // Evento chiamato quando il timer finisce
    public static event Action OnTimerFinished;

    private void Start()
    {
        currentTime = startTime;
        if (autoStart)
            StartTimer();
    }

    private void Update()
    {
        if (!isRunning || IsFinished)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            IsFinished = true;
            isRunning = false;

            if (hideOnZero && timerText != null)
                timerText.gameObject.SetActive(false);

            Debug.Log("[TimerCountdown] Timer terminato.");

            // Scatena evento globale
            OnTimerFinished?.Invoke();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;

    public void ResetTimer(float newTime = -1)
    {
        currentTime = newTime > 0 ? newTime : startTime;
        IsFinished = false;
        UpdateUI();
    }
}
