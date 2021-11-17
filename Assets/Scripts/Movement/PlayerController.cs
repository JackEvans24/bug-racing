using System;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(CarItemSystem))]
public class PlayerController : MonoBehaviour
{
    [NonSerialized] public CameraFollow CameraFollow;

    private CarMovement movement;
    private CarItemSystem itemSystem;

    private void Awake()
    {
        this.movement = GetComponent<CarMovement>();
        this.itemSystem = GetComponent<CarItemSystem>();
    }

    private void Update()
    {
        // Movement
        var forwardMovement = Input.GetAxis(InputButtons.VERTICAL);
        var turnAmount = Input.GetAxis(InputButtons.HORIZONTAL);

        this.movement.SetMovement(forwardMovement, turnAmount);

        // Item
        if (Input.GetButtonDown(InputButtons.USE_ITEM))
            this.itemSystem.UseCurrentItem();

        // Camera
        this.CameraFollow.ReverseView = Input.GetButton(InputButtons.REVERSE_VIEW);
    }
}
