using UnityEngine;
using UnityEngine.UI; // Добавьте это пространство имен
using TMPro; // Для использования TextMeshPro

using UnityEngine.SceneManagement;

public class NewGameModal : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField; // Поле для ввода имени
    [SerializeField] private Toggle plusToggle; // Переключатель для "+"
    [SerializeField] private Toggle minusToggle; // Переключатель для "-"
    [SerializeField] private Toggle divideToggle; // Переключатель для "\"
    [SerializeField] private Toggle multiplyToggle; // Переключатель для "*"
    [SerializeField] private GameObject modalPanel; // Панель модального окна
    [SerializeField] private TMP_InputField minValueInputField; // Поле для минимального значения
    [SerializeField] private TMP_InputField maxValueInputField; // Поле для максимального значения
    public GameSettings gameSettingsAsset; // Объект с настройками игры 
    [SerializeField] private TMP_Text logText; // Поле для вывода логов в UI

    void Start()
    {

        modalPanel.SetActive(false); // Добавьте эту строку для скрытия панели
        Debug.Log("проставилось скрытие панели");

        // Установите "+" как активированный по умолчанию
        plusToggle.isOn = true;
        minusToggle.isOn = false;
        divideToggle.isOn = false;
        multiplyToggle.isOn = false;
        // isManualInputEnabled.isOn = false;
        // manualInputToggle.isOn = false;

        // Включаем Input Field для минимального значения, если он был выключен
        if (!minValueInputField.gameObject.activeSelf)
        {
            minValueInputField.gameObject.SetActive(true); // Включаем объект, если он выключен
        }

        // Включаем Input Field для минимального значения, если он был выключен
        if (!maxValueInputField.gameObject.activeSelf)
        {
            maxValueInputField.gameObject.SetActive(true); // Включаем объект, если он выключен
        }

    }

    public void ClearLog()
    {
        if (logText != null)
        {
            logText.text = string.Empty;
        }
    }

    public void OnStartClicked()
    {
        ClearLog();

        // Проверка имени
        if (string.IsNullOrWhiteSpace(nameInputField.text))
        {
            Debug.LogWarning("Заполните имя");
            AddLogMessage("Заполните имя");
            return;
        }

        // Проверка операций
        if (!plusToggle.isOn && !minusToggle.isOn && !divideToggle.isOn && !multiplyToggle.isOn)
        {
            Debug.LogWarning("Выберите хотя бы одну операцию");
            AddLogMessage("Выберите хотя бы одну операцию");
            return;
        }

        // Проверка минимального и максимального значения
        if (string.IsNullOrWhiteSpace(minValueInputField.text) || string.IsNullOrWhiteSpace(maxValueInputField.text))
        {
            Debug.LogWarning("Введите минимальное и максимальное значения");
            AddLogMessage("Введите минимальное и максимальное значения");
            return;
        }

        if (!int.TryParse(minValueInputField.text, out int minValue) ||
            !int.TryParse(maxValueInputField.text, out int maxValue))
        {
            Debug.LogWarning("Минимальное и максимальное значения должны быть числами");
            AddLogMessage("Мин. и макс. значения должны быть числами");

            return;
        }

        if (minValue < 1 || maxValue > 999)
        {
            Debug.LogWarning("Минимальное значение должно быть >= 1, а максимальное <= 999");
            AddLogMessage("Мин. значение должно быть >= 1, макс. <= 999");
            return;
        }

        if (minValue >= maxValue)
        {
            Debug.LogWarning("Минимальное значение должно быть меньше максимального");
            AddLogMessage("Минимальное значение должно быть меньше максимального");
            return;
        }

        // Сохраняем значения в настройки
        gameSettingsAsset.playerName = nameInputField.text;
        gameSettingsAsset.addition = plusToggle.isOn;
        gameSettingsAsset.subtraction = minusToggle.isOn;
        gameSettingsAsset.multiplication = multiplyToggle.isOn;
        gameSettingsAsset.division = divideToggle.isOn;
        gameSettingsAsset.minValue = minValue;
        gameSettingsAsset.maxValue = maxValue;
        gameSettingsAsset.score = 0;
        SessionManager.Instance.CurrentPlayerName = nameInputField.text; 
        SceneManager.LoadScene("game");
    }


    // Вызывается при нажатии кнопки "К меню"
    public void OnBackToMenuClicked()
    {
        Debug.Log("Нажатие на возврат в главное меню");
        HideModal();
        Debug.Log("Возврат в главное меню");
    }

    // Метод для обновления состояния переключателей операций
    public void OnOperationToggleChanged()
    {
        // Убедитесь, что хотя бы один переключатель выбран
        if (!plusToggle.isOn && !minusToggle.isOn && !divideToggle.isOn && !multiplyToggle.isOn)
        {
            plusToggle.isOn = true; // Вернуть "+" по умолчанию, если все выключены
        }
    }
    public void ShowModal()
    {
        modalPanel.SetActive(true);
        var canvasGroup = modalPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
    }
    // Информирование о проблемах валидации
    public void AddLogMessage(string message)
    {
        if (logText != null)
        {
            logText.text += message + "\n";
        }
    }

    public void HideModal()
    {
        modalPanel.SetActive(false);
        var canvasGroup = modalPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }
}