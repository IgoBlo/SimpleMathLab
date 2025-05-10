using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    public string CurrentPlayerName { get; set; }
    public List<GameManager.Result> Results { get; private set; } = new List<GameManager.Result>();

    private const string ResultsKey = "GameResults"; // Ключ для сохранения данных

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadResults(); // Загружаем результаты при запуске игры
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddResult(GameManager.Result result)
    {
        Results.Add(result);
        Results = Results.OrderByDescending(r => r.percentage).ToList(); // Сортировка по убыванию

        if (Results.Count > 10)
        {
            Results = Results.Take(10).ToList(); // Храним только 10 лучших результатов
        }

        SaveResults(); // Сохраняем результаты
    }

    private void SaveResults()
    {
        string json = JsonUtility.ToJson(new ResultsWrapper(Results));
        PlayerPrefs.SetString(ResultsKey, json);
        PlayerPrefs.Save();
        Debug.Log("Результаты сохранены.");
    }

    private void LoadResults()
{
    if (PlayerPrefs.HasKey(ResultsKey))
    {
        string json = PlayerPrefs.GetString(ResultsKey);
        ResultsWrapper wrapper = JsonUtility.FromJson<ResultsWrapper>(json);
        if (wrapper != null && wrapper.results != null)
        {
            Results = wrapper.results;
        }
        else
        {
            Results = new List<GameManager.Result>(); // Создать новый список
        }
    }
    else
    {
        Results = new List<GameManager.Result>(); // Создать новый список
    }
}

    [System.Serializable]
    private class ResultsWrapper
    {
        public List<GameManager.Result> results;

        public ResultsWrapper(List<GameManager.Result> results)
        {
            this.results = results;
        }
    }
}
