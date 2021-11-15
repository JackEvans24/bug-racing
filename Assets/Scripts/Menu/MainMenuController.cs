using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartSinglePlayer()
    {
        LoadRace(Scenes.OakHighway);
    }

    public void Quit()
    {
        Debug.Log("Quit application");
    }

    private void LoadRace(Scenes scene)
    {
        SceneManager.LoadScene((int)scene);
    }
}
