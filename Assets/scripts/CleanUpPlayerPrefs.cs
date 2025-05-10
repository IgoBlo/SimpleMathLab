using UnityEngine;

public class ClearResults : MonoBehaviour
{
    [ContextMenu("Clear Results")] // Добавляет пункт в контекстное меню
    private void ClearAllResults()
    {
        PlayerPrefs.DeleteKey("GameResults"); // Удаляем ключ из PlayerPrefs
        PlayerPrefs.Save();
        Debug.Log("Результаты очищены!");
    }
}