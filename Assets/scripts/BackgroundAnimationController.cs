using UnityEngine;
using TMPro;
using System.Collections;

public class BackgroundAnimationController : MonoBehaviour
{
    public GameObject[] textPrefabs; // Массив префабов для анимации фона
    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }
        StartCoroutine(SpawnTextObjects());
    }

    IEnumerator SpawnTextObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            SpawnText(); 
        }
    }

    void SpawnText()
    {
        // Выбираем случайный префаб из массива
        GameObject selectedPrefab = textPrefabs[Random.Range(0, textPrefabs.Length)];
        GameObject newTextObject = Instantiate(selectedPrefab, canvas.transform);

        // Активируем объект, если он был отключен в префабе
        newTextObject.SetActive(true);

        // Выравниваем в верхней части экрана и устанавливаем движение вниз
        RectTransform rectTransform = newTextObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(
            Random.Range(-canvas.GetComponent<RectTransform>().sizeDelta.x / 2, canvas.GetComponent<RectTransform>().sizeDelta.x / 2),
            canvas.GetComponent<RectTransform>().sizeDelta.y / 2
        );

        // Устанавливаем случайный цвет текста
        TextMeshProUGUI newText = newTextObject.GetComponent<TextMeshProUGUI>();
        newText.color = new Color(Random.value, Random.value, Random.value);

        // Устанавливаем случайный размер шрифта
        newText.fontSize = Random.Range(20, 60);

        // Запускаем корутину для перемещения и вращения текста
        StartCoroutine(MoveAndRotateText(newTextObject));
    }

    IEnumerator MoveAndRotateText(GameObject textObject)
    {
        float rotationSpeed = Random.Range(10f, 50f);
        float moveSpeed = Random.Range(50f, 200f);

        // Основное направление вниз
        Vector2 direction = Vector2.down;

        // Получаем RectTransform
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        while (textObject != null)
        {
            // Позиция мыши в координатах Canvas
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, Input.mousePosition, canvas.worldCamera, out mousePosition);

            // Позиция объекта
            Vector2 objectPosition = rectTransform.anchoredPosition;

            // Рассчитываем вектор отталкивания
            Vector2 repelVector = objectPosition - mousePosition;
            float distance = repelVector.magnitude;

            // Применяем отталкивание только если расстояние меньше радиуса
            float repelRadius = 300f; // Радиус отталкивания
            if (distance < repelRadius)
            {
                float repelStrength = (repelRadius - distance) / repelRadius; // От 1 до 0
                repelVector = repelVector.normalized * repelStrength * 300f; // Масштабируем силу отталкивания
            }
            else
            {
                repelVector = Vector2.zero; // Если курсор далеко, отталкивания нет
            }

            // Обновляем позицию: движение вниз + отталкивание
            rectTransform.anchoredPosition += (direction * moveSpeed + repelVector) * Time.deltaTime;

            // Вращаем текст
            textObject.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));

            // Удаляем элемент, если он вышел за нижнюю границу
            if (rectTransform.anchoredPosition.y < -canvasRect.sizeDelta.y / 2)
            {
                Destroy(textObject);
            }

            yield return null;
        }
    }
}