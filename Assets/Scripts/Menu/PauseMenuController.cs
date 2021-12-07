using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public bool Paused;

    [SerializeField] private GameObject resumeButton;

    private CanvasGroup canvas;

    private void Awake()
    {
        this.canvas = GetComponent<CanvasGroup>();
    }

    public void Pause()
    {
        Time.timeScale = 0f;

        this.canvas.interactable = true;

        GameController.PauseAll();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton);

        this.canvas.alpha = 1f;
        this.Paused = true;
    }

    public void Resume()
    {
        this.canvas.alpha = 0f;
        this.canvas.interactable = false;

        Time.timeScale = 1f;

        GameController.ResumeAll();

        this.Paused = false;
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        GameController.LoadScene(Scenes.Menu);
    }
}
