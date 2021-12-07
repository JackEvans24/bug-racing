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
    [SerializeField] private CanvasGroup controlsView;
    [SerializeField] private CanvasGroup player2Hint;
    [SerializeField] private MusicTrack menuMusic;

    [Header("Track selection")]
    [SerializeField] private TrackSelectMenuController trackSelection;

    [Header("Multiplayer")]
    [SerializeField] private float rejoinTimeout = 0.5f;
    private List<PlayerInput> currentInputs;
    private bool menuIsActive;

    private Dictionary<int, PlayerSelection> selectedPlayers;

    private void Awake()
    {
        this.menuIsActive = true;
        this.currentInputs = new List<PlayerInput>();
        this.selectedPlayers = new Dictionary<int, PlayerSelection>();
    }

    private void Start()
    {
        GameController.UpdateMusic(menuMusic);
    }

    public void PlayerJoined(PlayerInput input)
    {
        this.currentInputs.Add(input);

        if (menuIsActive)
        {
            this.mainMenu.SetActive(false);
            this.mainCamera.gameObject.SetActive(false);

            this.controlsView.alpha = 1;
            this.player2Hint.alpha = 1;

            menuIsActive = false;
        }

        if (this.currentInputs.Count == this.inputManager.maxPlayerCount)
            this.player2Hint.alpha = 0;
    }

    public void PlayerLeft(PlayerInput input)
    {
        this.currentInputs.Remove(input);

        if (!menuIsActive && this.currentInputs.Count <= 0)
        {
            this.mainMenu.SetActive(true);
            if (this.mainCamera != null)
                this.mainCamera.gameObject.SetActive(true);

            this.controlsView.alpha = 0;
            this.player2Hint.alpha = 0;

            menuIsActive = true;
        }
        else
            this.player2Hint.alpha = 1;

        StartCoroutine(this.ReenableJoining());
    }

    private IEnumerator ReenableJoining()
    {
        this.inputManager.DisableJoining();

        yield return new WaitForSeconds(this.rejoinTimeout);
        this.inputManager.EnableJoining();
    }

    public void Ready(int playerNumber, PlayerSelection selection)
    {
        this.selectedPlayers.Add(playerNumber, selection);

        if (this.selectedPlayers.Count == this.currentInputs.Count)
            this.ShowTrackSelectionMenu();
    }

    public void UnreadyAll()
    {
        foreach (var selection in GameObject.FindGameObjectsWithTag(Tags.PLAYER_SELECTION))
        {
            var menu = selection.GetComponent<SelectionMenuController>();
            if (menu != null)
                menu.UnReady();
        }
    }

    public void Unready(InputDevice device)
    {
        this.selectedPlayers = this.selectedPlayers.Where(kvp => kvp.Value.Device != device).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public void BackToMainMenu()
    {
        foreach (var selection in GameObject.FindGameObjectsWithTag(Tags.PLAYER_SELECTION))
            Destroy(selection);

        this.currentInputs.Clear();
        this.selectedPlayers.Clear();
    }

    private void ShowTrackSelectionMenu()
    {
        this.inputManager.DisableJoining();
        this.player2Hint.alpha = 0;

        var selectionMenus = FindObjectsOfType<SelectionMenuController>();
        foreach (var menu in selectionMenus)
            menu.AllPlayersReady();

        this.trackSelection.Show();
    }

    public void BackToSelectionMenu()
    {
        this.UnreadyAll();

        this.inputManager.EnableJoining();

        if (this.currentInputs.Count < this.inputManager.maxPlayerCount)
            this.player2Hint.alpha = 1;
    }

    public void StartRace(Scenes scene)
    {
        var players = this.selectedPlayers
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => kvp.Value)
            .ToList();

        GameController.SetPlayersAndPlay(players, scene);
    }

    public void Quit()
    {
        Debug.Log("Quit application");
    }
}
