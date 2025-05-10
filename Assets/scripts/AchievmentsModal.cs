using UnityEngine;
using TMPro;
using System.Linq;

public class AchievementsModalController : MonoBehaviour
{
    [SerializeField] private GameObject modalPanel;
    [SerializeField] private Transform resultTableParent;
    [SerializeField] private GameObject resultRowPrefab;

    public TextMeshProUGUI[] tableHeaders; // Заголовки таблицы

    void Start()
    {
        if (modalPanel != null)
        {
            modalPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("ModalPanel для AchievementsModal не привязан в инспекторе!");
        }
    }

    public void ShowModal()
    {
        if (modalPanel != null)
        {
            modalPanel.SetActive(true);
            ShowResults();
        }
    }

    public void ShowResults()
{
    // Удаляем старые строки из таблицы (кроме заголовков)
    foreach (Transform child in resultTableParent)
    {
        if (child.gameObject.name != "Заголовок")
        {
            child.gameObject.SetActive(false);
        }
    }

    ShowTableHeaders(); // Показываем заголовки таблицы

    var results = SessionManager.Instance.Results;

    if (results == null || results.Count == 0)
    {
        Debug.Log("Нет сохраненных результатов для отображения.");
        return;
    }

    // Ограничиваем количество отображаемых результатов до 5
    int maxResultsToShow = 5;
    var topResults = results.Take(maxResultsToShow).ToList(); // Берем первые 5 результатов

    foreach (var result in topResults)
    {
        if (resultRowPrefab == null)
        {
            Debug.LogError("ResultRowPrefab не привязан в инспекторе или был уничтожен!");
            return;
        }

        // Создаем новую строку таблицы из префаба
        GameObject row = Instantiate(resultRowPrefab, resultTableParent);

        // Включаем все компоненты TextMeshProUGUI в строке
        TextMeshProUGUI[] textComponents = row.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var textComponent in textComponents)
        {
            if (!textComponent.enabled)
            {
                textComponent.enabled = true; // Включаем компонент TextMeshProUGUI
            }
        }

        // Заполняем текстовые значения
        TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length >= 4)
        {
            texts[0].text = result.playerName;                    // Имя игрока
            texts[1].text = result.correctAnswers.ToString();     // Количество правильных ответов
            texts[2].text = result.wrongAnswers.ToString();       // Количество неправильных ответов
            texts[3].text = result.percentage.ToString("F2") + "%"; // Процент правильных ответов
        }
        else
        {
            Debug.LogError("RowPrefab не содержит достаточно TextMeshProUGUI компонентов.");
        }

        // Активируем строку на случай, если она не активна
        row.SetActive(true);
    }
}

    private void ShowTableHeaders()
    {
        if (tableHeaders == null || tableHeaders.Length == 0)
        {
            Debug.LogError("TableHeaders не инициализированы или пусты.");
            return;
        }

        foreach (var header in tableHeaders)
        {
            if (header != null)
            {
                header.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Один из элементов tableHeaders равен null.");
            }
        }
    }
    public void HideModal()
    {
        if (modalPanel != null)
        {
            modalPanel.SetActive(false);
        }
    }
}