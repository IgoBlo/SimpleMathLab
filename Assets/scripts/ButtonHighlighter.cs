using UnityEngine;
using UnityEngine.EventSystems; // Для работы с системой событий
using TMPro; // Для работы с TextMeshPro

public class ButtonHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color defaultColor = Color.white; // Стандартный цвет текста
    public Color highlightColor = Color.yellow; // Цвет подсветки при наведении
    private TextMeshProUGUI textComponent; // Компонент TextMeshProUGUI

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent == null)
        {
            Debug.LogError("ButtonHighlighter не может найти компонент TextMeshProUGUI на объекте.");
        }
        else
        {
            textComponent.color = defaultColor; // Установить начальный цвет текста
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textComponent != null)
        {
            textComponent.color = highlightColor; // Изменить цвет при наведении курсора
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (textComponent != null)
        {
            textComponent.color = defaultColor; // Вернуть стандартный цвет
        }
    }
}
