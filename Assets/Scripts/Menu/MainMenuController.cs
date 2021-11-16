using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private GameObject defaultButton;

    private void OnEnable()
    {
        StartCoroutine(this.SetSelectedButton());
    }

    private IEnumerator SetSelectedButton()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(this.defaultButton);
    }

    public void StartSinglePlayer()
    {
        this.selectionMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void Quit()
    {
        Debug.Log("Quit application");
    }
}
