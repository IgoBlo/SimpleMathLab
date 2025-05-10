using UnityEngine;
using UnityEngine.EventSystems; // Для работы с системой событий
using TMPro; // Для работы с TextMeshPro

public class MenuButtonHandler : MonoBehaviour, IPointerClickHandler
{
    public NewGameModal newGameModalScript;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (gameObject.name)
        {
            case "Выход":
                QuitGame();
                break;
            case "Новая игра":
                StartNewGame();
                break;
            case "Достижения":
                OpenAchievements();
                break;
        }
    }

    private void QuitGame()
    {
        Debug.Log("Нажато `выход`");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Если в редакторе
        #else
            Application.Quit();
        #endif
    }

    private void StartNewGame()
    {
        Debug.Log("Нажато 'новая игра'.");
        if (newGameModalScript != null)
        {
            // Вызываем метод непосредственно из скрипта NewGameModal
            newGameModalScript.ShowModal();
            Debug.Log("Открыта модалка новой игры. через showmodal");
        }
        else
        {
            Debug.LogError("Скрипт управления модальным окном не найден в сцене!");
        }
    }

    private void OpenAchievements()
    {
        Debug.Log("Нажато 'достижения'.");
    }
}