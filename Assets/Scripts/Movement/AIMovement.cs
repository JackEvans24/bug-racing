using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
public class AIMovement : MonoBehaviour
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

    [Header("Ground Raycasts")]
    [SerializeField] private Raycaster[] leftGroundChecks;
    [SerializeField] private Raycaster[] rightGroundChecks;

    [Header("Obstacle Raycasts")]
    [SerializeField] private Raycaster[] leftObstacleChecks;
    [SerializeField] private Raycaster[] rightObstacleChecks;

    [Header("Player Raycasts")]
    [SerializeField] private Raycaster[] frontPlayerChecks;
    [SerializeField] private Raycaster[] leftPlayerChecks;
    [SerializeField] private Raycaster[] rightPlayerChecks;

    private CarMovement movement;
    private PassDirection passDirection;
    
    private float forwardMovement = 1f;

    private void Awake()
    {
        this.movement = GetComponent<CarMovement>();
        this.movement.IsAi = true;
    }

    private void Update()
    {
        this.forwardMovement = 1;
        var turnAmount = this.GetTurnAmount();

        this.movement.SetMovement(this.forwardMovement, turnAmount);
    }

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

        turnAngle = this.AvoidPlayers(turnAngle);

        return turnAngle;
    }

    private float GetMinTurnAngle()
    {
        var checkpoint = this.movement.NextCheckpoint;

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

    private bool TryAvoidEdges(float turnAngle, out float avoidAngle)
    {
        avoidAngle = turnAngle;

        // Check collisions
        var leftCollisions = this.leftGroundChecks.Count(c => c.Hit.transform != null);
        var rightCollisions = this.rightGroundChecks.Count(c => c.Hit.transform != null);

        // If both have ground, or neither have ground, don't edit turn value
        if (leftCollisions == rightCollisions)
            return false;

        // Otherwise, steer away from edge and slow down
        this.forwardMovement = (float)(rightCollisions + leftCollisions) / (float)(this.rightGroundChecks.Length + this.leftGroundChecks.Length);
        avoidAngle += this.groundAvoidAmount * (rightCollisions - leftCollisions);
        return true;
    }

    private bool TryAvoidObstacles(float turnAngle, out float avoidAngle)
    {
        avoidAngle = turnAngle;

        // Check collisions
        var leftCollisions = this.leftObstacleChecks.Count(c => c.Hit.transform != null);
        var rightCollisions = this.rightObstacleChecks.Count(c => c.Hit.transform != null);

        // If both or neither, don't edit turn value
        if (leftCollisions == rightCollisions)
            return false;

        // Otherwise, steer away from obstacle and slow down
        this.forwardMovement = (float)((this.rightGroundChecks.Length + this.leftGroundChecks.Length) - (rightCollisions + leftCollisions)) / (float)(this.rightGroundChecks.Length + this.leftGroundChecks.Length);
        avoidAngle += this.obstacleAvoidAmount * (leftCollisions - rightCollisions);
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

    enum PassDirection
    {
        None,
        Left,
        Right
    }
}
