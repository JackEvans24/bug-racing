using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    [Header("Menu references")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputManager inputManager;

    [Header("Multiplayer")]
    [SerializeField] private int maxPlayers = 2;
    [SerializeField] private float rejoinTimeout = 2;
    private List<PlayerInput> currentInputs = new List<PlayerInput>();
    private bool menuIsActive = true;

    private List<PlayerSelection> selectedPlayers = new List<PlayerSelection>();

    public void PlayerJoined(PlayerInput input)
    {
        this.currentInputs.Add(input);

        if (menuIsActive)
        {
            this.mainMenu.SetActive(false);
            this.mainCamera.gameObject.SetActive(false);

            //var menu = input.GetComponent<SelectionMenuController>();
            //menu.SetBackButtonEnabled(false);

            menuIsActive = false;
        }
    }

    public void PlayerLeft(PlayerInput input)
    {
        this.currentInputs.Remove(input);

        if (!menuIsActive && this.currentInputs.Count <= 0)
        {
            this.mainMenu.SetActive(true);
            if (this.mainCamera != null)
                this.mainCamera.gameObject.SetActive(true);

            //var menu = input.GetComponent<SelectionMenuController>();
            //menu.SetBackButtonEnabled(true);

            menuIsActive = true;
        }

        StartCoroutine(this.ReenableJoining());
    }

    private IEnumerator ReenableJoining()
    {
        this.inputManager.DisableJoining();

        yield return new WaitForSeconds(this.rejoinTimeout);
        this.inputManager.EnableJoining();
    }

    public void Ready(PlayerSelection selection)
    {
        this.selectedPlayers.Add(selection);

        if (this.selectedPlayers.Count == this.currentInputs.Count)
            this.StartRace();
    }

    public void Unready(InputDevice device)
    {
        this.selectedPlayers = this.selectedPlayers.Where(p => p.Device != device).ToList();
    }

    public void Back()
    {
        foreach (var selection in GameObject.FindGameObjectsWithTag(Tags.PLAYER_SELECTION))
            Destroy(selection);

        this.currentInputs.Clear();
        this.selectedPlayers.Clear();
    }

    public void StartRace()
    {
        //var menu = this.currentInputs[0].GetComponent<SelectionMenuController>();
        //menu.SetBackButtonEnabled(true);

        GameController.SetPlayersAndPlay(this.selectedPlayers, Scenes.OakHighway);
    }

    public void Quit()
    {
        Debug.Log("Quit application");
    }
}
