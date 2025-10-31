using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private int currentPage = 0;

    private void Start()
    {
        ShowPage(currentPage);

        // collega i pulsanti
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (prevButton != null) prevButton.onClick.AddListener(PreviousPage);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
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
