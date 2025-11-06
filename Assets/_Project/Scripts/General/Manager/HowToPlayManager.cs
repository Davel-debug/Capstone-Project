using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HowToPlayManager : MonoBehaviour
{
    [Header("Pages")]
    [Tooltip("Lista delle pagine (ogni GameObject è una pagina del tutorial).")]
    public GameObject[] pages;

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button prevButton;

    [Header("UI Text (optional)")]
    public TextMeshProUGUI pageCounterText;

    [Header("Settings")]
    [Tooltip("Tempo minimo tra due click consecutivi.")]
    public float clickCooldown = 0.3f;

    private int currentPage = 0;
    private bool canClick = true;

    private void Start()
    {
        ShowPage(currentPage);

        // collega i pulsanti
        if (nextButton != null) nextButton.onClick.AddListener(() => TryChangePage(1));
        if (prevButton != null) prevButton.onClick.AddListener(() => TryChangePage(-1));
    }

    private void TryChangePage(int direction)
    {
        if (!canClick) return;
        StartCoroutine(ClickCooldownRoutine());

        int newPage = currentPage + direction;
        if (newPage >= 0 && newPage < pages.Length)
        {
            currentPage = newPage;
            ShowPage(currentPage);
        }
    }

    private IEnumerator ClickCooldownRoutine()
    {
        canClick = false;
        yield return new WaitForSeconds(clickCooldown);
        canClick = true;
    }

    private void ShowPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }

        // aggiorna pulsanti
        if (prevButton != null) prevButton.interactable = index > 0;
        if (nextButton != null) nextButton.interactable = index < pages.Length - 1;

        // aggiorna contatore testo
        if (pageCounterText != null)
            pageCounterText.text = $"{index + 1} / {pages.Length}";
    }
}
