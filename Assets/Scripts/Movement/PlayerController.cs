using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AIController))]
[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(CarItemSystem))]
public class PlayerController : MonoBehaviour
{
    [NonSerialized] public CameraFollow CameraFollow;

    private AIController ai;
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

        var cameraReverse = actions.FindAction(InputButtons.REVERSE_VIEW);
        cameraReverse.started += (_obj) => this.SetCameraReverse(true);
        cameraReverse.canceled += (_obj) => this.SetCameraReverse(false);
    }

    private void Update()
    {
        // Movement
        var currentMovement = actions.FindAction(InputButtons.MOVE).ReadValue<Vector2>();
        this.movement.SetMovement(currentMovement.y, currentMovement.x);
    }

    private void SetCameraReverse(bool reverse) => this.CameraFollow.ReverseView = reverse;

    public void RaceComplete()
    {
        this.ai.enabled = true;
        this.enabled = false;
    }
}
