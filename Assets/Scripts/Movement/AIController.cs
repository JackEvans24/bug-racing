using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(CarItemSystem))]
public class AIController : MonoBehaviour
{
    [Header("Turning")]
    [SerializeField] private float maxTurnAngle = 45f;
    [SerializeField] private float checkpointInSightModifier = 0.3f;
    [Range(0, 1)]
    [SerializeField] private float groundAvoidAmount = 0.8f;
    [Range(0, 1)]
    [SerializeField] private float obstacleAvoidAmount = 0.2f;
    [Range(0, 1)]
    [SerializeField] private float playerAvoidAmount = 0.5f;
    [SerializeField] private float deadZone = 2f;

    [Header("Item usage")]
    [SerializeField] private Vector2 itemUseWaitBounds;

    [Header("Ground Raycasts")]
    [SerializeField] private Raycaster[] leftGroundChecks;
    [SerializeField] private Raycaster[] rightGroundChecks;

    [Header("Obstacle Raycasts")]
    [SerializeField] private Raycaster[] leftObstacleChecks;
    [SerializeField] private Raycaster[] rightObstacleChecks;

    [Header("Item Raycasts")]
    [SerializeField] private Raycaster[] leftItemChecks;
    [SerializeField] private Raycaster[] rightItemChecks;

    [Header("Player Raycasts")]
    [SerializeField] private Raycaster[] frontPlayerChecks;
    [SerializeField] private Raycaster[] leftPlayerChecks;
    [SerializeField] private Raycaster[] rightPlayerChecks;

    private CarMovement movement;
    private CarItemSystem itemSystem;
    private PassDirection passDirection;
    
    private float forwardMovement = 1f;

    private float currentItemWaitTime, currentItemWaitLimit;
    private bool waitingForItemUse;

    private void Awake()
    {
        this.movement = GetComponent<CarMovement>();
        this.movement.IsAi = true;

        this.itemSystem = GetComponent<CarItemSystem>();
    }

    private void Update()
    {
        this.forwardMovement = 1;
        var turnAmount = this.GetTurnAmount();

        this.movement.SetMovement(this.forwardMovement, turnAmount);

        this.HandleItemUsage();
    }

    #region Movement

    private float GetTurnAmount()
    {
        var angleBetween = this.GetMinTurnAngle();
        float turnAngle;

        if (Mathf.Abs(angleBetween) < this.deadZone)
            turnAngle = 0f;
        else if (angleBetween > 0)
            turnAngle = Mathf.Min(angleBetween / this.maxTurnAngle, 1f);
        else
            turnAngle = Mathf.Max(angleBetween / this.maxTurnAngle, -1f);

        if (this.TryAvoidEdges(turnAngle, out var avoidAngle))
            return avoidAngle;
        else if (this.TryAvoidObstacles(turnAngle, out avoidAngle))
            return avoidAngle;
        else if (this.TryHitItems(turnAngle, out avoidAngle))
            return avoidAngle;

        turnAngle = this.AvoidPlayers(turnAngle);

        return turnAngle;
    }

    private float GetMinTurnAngle()
    {
        var checkpoint = this.movement.NextCheckpoint;
        if (checkpoint == null)
            return 0f;

        var minAngle = Vector3.SignedAngle(this.transform.forward, checkpoint.MinimumBoundary - this.transform.position, transform.up);
        var maxAngle = Vector3.SignedAngle(this.transform.forward, checkpoint.MaximumBoundary - this.transform.position, transform.up);
        var modifier = 1f;

        if (Mathf.Sign(minAngle) != Mathf.Sign(maxAngle))
        {
            minAngle = Vector3.SignedAngle(this.transform.forward, checkpoint.MinimumInnerBoundary - this.transform.position, transform.up);
            maxAngle = Vector3.SignedAngle(this.transform.forward, checkpoint.MaximumInnerBoundary - this.transform.position, transform.up);
            modifier = this.checkpointInSightModifier;

            if (Mathf.Sign(minAngle) != Mathf.Sign(maxAngle))
                return 0f;
        }
        
        if (Mathf.Abs(minAngle) < Mathf.Abs(maxAngle))
            return minAngle * modifier;
        else
            return maxAngle * modifier;
    }

    private bool TryAvoidEdges(float turnAngle, out float avoidAngle) =>
        this.TrySteerAccordingToRaycasters(turnAngle, out avoidAngle, this.leftGroundChecks, this.rightGroundChecks, avoid: false);

    private bool TryAvoidObstacles(float turnAngle, out float avoidAngle) =>
        this.TrySteerAccordingToRaycasters(turnAngle, out avoidAngle, this.leftObstacleChecks, this.rightObstacleChecks);

    private bool TryHitItems(float turnAngle, out float avoidAngle) =>
        this.TrySteerAccordingToRaycasters(turnAngle, out avoidAngle, this.leftItemChecks, this.rightItemChecks, avoid: false, editSpeed: false);

    private bool TrySteerAccordingToRaycasters(float turnAngle, out float steerAngle, Raycaster[] leftChecks, Raycaster[] rightChecks, bool avoid = true, bool editSpeed = true)
    {
        steerAngle = turnAngle;

        // Check collisions
        var leftCollisions = leftChecks.Count(c => c.Hit.transform != null);
        var rightCollisions = rightChecks.Count(c => c.Hit.transform != null);

        // If both or neither, don't edit turn value
        if (leftCollisions == rightCollisions)
            return false;

        // Otherwise, steer towards / away from collisions and slow down
        if (editSpeed)
        {
            this.forwardMovement = avoid ?
                (float)((leftChecks.Length + rightChecks.Length) - (rightCollisions + leftCollisions)) / (leftChecks.Length + rightChecks.Length) :
                (float)(rightCollisions + leftCollisions) / (leftChecks.Length + rightChecks.Length);
        }

        steerAngle += this.obstacleAvoidAmount * (leftCollisions - rightCollisions) * (avoid ? 1 : -1);
        return true;
    }

    private float AvoidPlayers(float turnAngle)
    {
        // Check collisions
        var leftCollisions = this.leftPlayerChecks.Count(c => c.Hit.transform != null);
        var rightCollisions = this.rightPlayerChecks.Count(c => c.Hit.transform != null);
        var hasForwardCollision = this.frontPlayerChecks.Any(c => c.Hit.transform != null);

        // Reset pass direction if nothing in front
        if (!hasForwardCollision)
            this.passDirection = PassDirection.None;

        // Return without modification if boxed in
        if (leftCollisions == 0 && rightCollisions == 0 && !hasForwardCollision)
            return turnAngle;
        else if (leftCollisions == rightCollisions)
            return turnAngle;

        // If left or right, modify turn angle and return
        if (leftCollisions > rightCollisions)
            return turnAngle + this.playerAvoidAmount;
        else if (rightCollisions > leftCollisions)
            return turnAngle - this.playerAvoidAmount;

        // Only collision is in front, time to choose a passing direction
        if (this.passDirection == PassDirection.None)
            this.passDirection = (PassDirection)Random.Range(1, 3);

        // Modify angle and return
        if (this.passDirection == PassDirection.Left)
            return turnAngle + this.playerAvoidAmount;
        else if (this.passDirection == PassDirection.Right)
            return turnAngle - this.playerAvoidAmount;

        // Code should never get this far
        return turnAngle;
    }

    #endregion

    #region Item Usage

    private void HandleItemUsage()
    {
        if (this.currentItemWaitTime <= this.currentItemWaitLimit)
            this.currentItemWaitTime += Time.deltaTime;

        if (this.currentItemWaitTime > this.currentItemWaitLimit && this.waitingForItemUse)
        {
            this.itemSystem.UseCurrentItem();
            this.waitingForItemUse = false;
        }
        else if (!this.waitingForItemUse && this.itemSystem.CurrentItems.Count > 0)
        {
            this.currentItemWaitLimit = Random.Range(this.itemUseWaitBounds.x, this.itemUseWaitBounds.y);
            this.currentItemWaitTime = 0f;
            this.waitingForItemUse = true;
        }
    }

    #endregion

    enum PassDirection
    {
        None,
        Left,
        Right
    }
}
