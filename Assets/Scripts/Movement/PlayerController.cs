using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AIController))]
[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(CarItemSystem))]
public class PlayerController : MonoBehaviour
{
    public CameraFollow CameraFollow;
    [SerializeField] private PauseMenuController pauseMenu;

    private AIController ai;
    [SerializeField] private GameObject[] aiObjects;

    private CarMovement movement;
    private CarItemSystem itemSystem;

    private PlayerInput input;
    private InputActionMap actions;

    private void Awake()
    {
        this.ai = GetComponent<AIController>();
        this.movement = GetComponent<CarMovement>();
        this.itemSystem = GetComponent<CarItemSystem>();
        this.input = GetComponent<PlayerInput>();

        this.SetUpInputEvents();
    }

    private void SetUpInputEvents()
    {
        actions = this.input.actions.FindActionMap(InputMaps.RACING);

        var item = actions.FindAction(InputButtons.USE_ITEM);
        item.started += (_obj) => this.itemSystem.UseCurrentItem();

        var pause = actions.FindAction(InputButtons.PAUSE);
        pause.started += (_obj) => this.TogglePause();

        var cameraReverse = actions.FindAction(InputButtons.REVERSE_VIEW);
        cameraReverse.started += (_obj) => this.SetCameraReverse(true);
        cameraReverse.canceled += (_obj) => this.SetCameraReverse(false);
    }

    private void Update()
    {
        // Movement
        var horizontalMovement = actions.FindAction(InputButtons.HORIZONTAL).ReadValue<float>();
        var verticalMovement = actions.FindAction(InputButtons.VERTICAL).ReadValue<float>();

        this.movement.SetMovement(verticalMovement, horizontalMovement);
    }

    private void SetCameraReverse(bool reverse) => this.CameraFollow.ReverseView = reverse;

    public void RaceComplete()
    {
        this.ai.enabled = true;

        foreach (var obj in this.aiObjects)
            obj.SetActive(true);

        this.enabled = false;
    }

    public void TogglePause()
    {
        if (Time.timeScale != 1 && !this.pauseMenu.Paused)
            return;

        if (this.pauseMenu.Paused)
        {
            this.pauseMenu.Resume();
        }
        else
        {
            this.pauseMenu.Pause();
        }
    }
}
