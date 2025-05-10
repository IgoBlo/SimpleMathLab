using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro
using UnityEngine.UI; // Для работы с UI

public class GameManager : MonoBehaviour
{
    public GameSettings gameSettingsAsset;
    public TextMeshProUGUI exampleText;
    public GameObject answerButtonPrefab; // Префаб кнопки ответа
    public Transform answerButtonParent; // Родительский объект для кнопок ответов
    public TextMeshProUGUI correctAnswersText; // Для отображения количества правильных ответов
    public TextMeshProUGUI wrongAnswersText; // Для отображения количества неправильных ответов
    public int numberOfAnswers = 4; // Количество ответов (включая правильный)
    private List<Result> results = new List<Result>();
    public Transform resultTableParent; // Родитель для строк таблицы
    public GameObject resultRowPrefab; // Префаб строки таблицы
    private int correctAnswer;
    private int correctAnswerCount = 0; // Счетчик правильных ответов
    private int wrongAnswerCount = 0;   // Счетчик неправильных ответов
    public TMP_InputField answerInputField; // Поле для ввода
    public Button submitButton; // Кнопка "Ответить"

    [System.Serializable] public class Result
    {
        public string playerName;
        public int correctAnswers;
        public int wrongAnswers;
        public float percentage;

        public Result(string playerName, int correctAnswers, int wrongAnswers)
        {
            this.playerName = playerName;
            this.correctAnswers = correctAnswers;
            this.wrongAnswers = wrongAnswers;
            this.percentage = (correctAnswers + wrongAnswers) > 0 ?
                (correctAnswers / (float)(correctAnswers + wrongAnswers)) * 100 : 0f;
        }
    }


    void Start()
    {
        if (gameSettingsAsset != null)
        {
            Debug.Log("Имя игрока: " + gameSettingsAsset.playerName);
            GenerateRandomExample();
            SetRandomAnswers(correctAnswer);
            UpdateAnswerCounts(); // Обновляем отображение счетчиков
        }
        else
        {
            Debug.LogError("Asset GameSettings не назначен в инспекторе");
        }
    }

    void GenerateRandomExample()
{
    int number1 = Random.Range(gameSettingsAsset.minValue, gameSettingsAsset.maxValue + 1);
    int number2 = Random.Range(gameSettingsAsset.minValue, gameSettingsAsset.maxValue + 1);

    List<string> availableOperations = new List<string>();
    if (gameSettingsAsset.addition) availableOperations.Add("+");
    if (gameSettingsAsset.subtraction) availableOperations.Add("-");
    if (gameSettingsAsset.multiplication) availableOperations.Add("*");
    if (gameSettingsAsset.division) availableOperations.Add("/");

    if (availableOperations.Count == 0)
    {
        Debug.LogError("Нет выбранных операций для генерации примера.");
        return;
    }

    string operation = availableOperations[Random.Range(0, availableOperations.Count)];

    if (operation == "/")
    {
        // Убедимся, что деление имеет целый результат
        number2 = Random.Range(gameSettingsAsset.minValue, gameSettingsAsset.maxValue + 1);
        number1 = number2 * Random.Range(gameSettingsAsset.minValue, gameSettingsAsset.maxValue / number2 + 1);
    }

    string example = number1 + " " + operation + " " + number2;
    correctAnswer = CalculateCorrectAnswer(number1, number2, operation);

    if (exampleText != null)
    {
        exampleText.gameObject.SetActive(true);
        exampleText.text = example + " = ?";
    }
    else
    {
        Debug.LogError("TextMeshProUGUI для отображения примера не назначен!");
    }
}


    int CalculateCorrectAnswer(int num1, int num2, string op)
    {
        switch (op)
        {
            case "+": return num1 + num2;
            case "-": return num1 - num2;
            case "*": return num1 * num2;
            case "/": return num2 != 0 ? num1 / num2 : 0;
        }
        return 0;
    }

    void SetRandomAnswers(int correctAnswer)
    {
        List<int> answers = new List<int> { correctAnswer };

        while (answers.Count < numberOfAnswers)
        {
            int falseAnswer;

            // Условие для положительного правильного ответа
            if (correctAnswer > 0)
            {
                falseAnswer = Random.Range(1, correctAnswer + 11); // Только положительные ответы
            }
            else
            {
                falseAnswer = Random.Range(correctAnswer - 10, correctAnswer + 11); // Возможны как положительные, так и отрицательные
            }

            if (!answers.Contains(falseAnswer))
            {
                answers.Add(falseAnswer);
            }
        }

        // Перемешиваем ответы
        for (int i = 0; i < answers.Count; i++)
        {
            int randomIndex = Random.Range(0, answers.Count);
            int temp = answers[i];
            answers[i] = answers[randomIndex];
            answers[randomIndex] = temp;
        }

        // Создаем кнопки или обновляем существующие
        for (int i = 0; i < numberOfAnswers; i++)
        {
            GameObject button;

            if (i < answerButtonParent.childCount)
            {
                button = answerButtonParent.GetChild(i).gameObject;
                button.SetActive(true);
            }
            else
            {
                button = Instantiate(answerButtonPrefab, answerButtonParent);
            }

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = answers[i].ToString();
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on the button prefab!");
            }

            Button buttonComponent = button.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.RemoveAllListeners();
                int answer = answers[i];
                buttonComponent.onClick.AddListener(() => CheckAnswer(answer));
            }
            else
            {
                Debug.LogError("Button component not found on the prefab!");
            }
        }

        // Деактивируем лишние кнопки, если они есть
        for (int i = numberOfAnswers; i < answerButtonParent.childCount; i++)
        {
            answerButtonParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SaveResultAndGoToStart()
    {
        // Сохраняем текущий результат
        Result newResult = new Result(
            SessionManager.Instance.CurrentPlayerName,
            correctAnswerCount,
            wrongAnswerCount
        );

        // Добавляем результат в SessionManager
        SessionManager.Instance.AddResult(newResult);

        // Логируем для отладки
        Debug.Log("Результат сохранен: " + newResult.playerName + ", процент: " + newResult.percentage);

        // Переходим на сцену "start"
        UnityEngine.SceneManagement.SceneManager.LoadScene("start");
    }


    void CheckAnswer(int chosenAnswer)
    {
        if (chosenAnswer == correctAnswer)
        {
            Debug.Log("Верный ответ!");
            correctAnswerCount++;
            Debug.Log("ok:" + correctAnswerCount + "errors:" + wrongAnswerCount);
        }
        else
        {
            Debug.Log("Неверный ответ.");
            wrongAnswerCount++;
            Debug.Log("ok:" + correctAnswerCount + "errors:" + wrongAnswerCount);
        }

        UpdateAnswerCounts(); // Обновляем отображение счетчиков
        GenerateRandomExample(); // Генерируем новый пример
        SetRandomAnswers(correctAnswer); // Перегенерируем ответы
    }

    void UpdateAnswerCounts()
    {
        if (correctAnswersText != null)
        {
            correctAnswersText.text = "Правильных ответов: " + correctAnswerCount;
        }

        if (wrongAnswersText != null)
        {
            wrongAnswersText.text = "Неправильных ответов: " + wrongAnswerCount;
        }
    }

    void EndSession()
    {
        Result newResult = new Result(
            SessionManager.Instance.CurrentPlayerName,
            correctAnswerCount,
            wrongAnswerCount
        );

        SessionManager.Instance.AddResult(newResult);
        Debug.Log("Результат сохранен: " + newResult.playerName + ", процент: " + newResult.percentage);

        UnityEngine.SceneManagement.SceneManager.LoadScene("start");
    }

    public void UpdateResultTable()
    {
        // Удаляем старые строки таблицы
        foreach (Transform child in resultTableParent)
        {
            Destroy(child.gameObject);
        }

        // Создаем строки для новых результатов
        foreach (var result in results)
        {
            GameObject row = Instantiate(resultRowPrefab, resultTableParent);
            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 4)
            {
                texts[0].text = result.playerName;
                texts[1].text = result.correctAnswers.ToString();
                texts[2].text = result.wrongAnswers.ToString();
                texts[3].text = result.percentage.ToString("F2") + "%";
            }
        }
    }
    void OnApplicationQuit()
    {
        EndSession(); // Сохраняем сессию при выходе
    }
}