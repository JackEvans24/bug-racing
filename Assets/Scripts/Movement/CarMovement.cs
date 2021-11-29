using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Game variables - DO NOT CHANGE")]
    public Checkpoint CurrentCheckpoint;
    public Checkpoint NextCheckpoint;
    public int LapsCompleted = 0;
    public bool IsAi = false;
    public bool CanMove = false;
    public int CurrentPosition;
    public bool IsStunned { get => this.currentStunTime <= this.stunTime; }

    [Header("References")]
    [SerializeField] private Collider bodyCollider;
    [SerializeField] private Raycaster mainGroundCheck;
    [SerializeField] private Raycaster[] groundChecks;
    [SerializeField] private Raycaster[] aerialGroundChecks;
    [SerializeField] private RaceTextController TextController;

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 1f;
    [SerializeField] private float reverseSpeed = 0.6f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private float aerialTurnSpeed = 0.2f;
    [SerializeField] private float rotationSmoothing = 1f;

    [Header("Boost")]
    [SerializeField] private Raycaster[] boostChecks;
    [SerializeField] private float boostSpeed = 1f;
    [SerializeField] private float speedPadBoostTime = 1f;

    [Header("Drag")]
    [SerializeField] private float groundDrag = 4f;
    [SerializeField] private float airDrag = 0.1f;
    [SerializeField] private float gravity = 10f;

    [Header("Speed Modifiers")]
    [SerializeField] private float slowGroundModifier = 0.8f;

    [Header("Checkpoint Reset")]
    [SerializeField] private float checkpointResetTime = 0.2f;

    private Rigidbody rb;
    private HoldingBugController[] holdingBugs;
    private float forwardMovement, turnAmount, velocity;
    private float stunTime, currentStunTime, currentBoostTime, boostTime;
    private bool isGrounded;

    public void SetMovement(float forward, float turnAmount)
    {
        this.forwardMovement = forward;
        this.turnAmount = turnAmount;
    }

    private void Awake()
    {
        this.rb = GetComponentInChildren<Rigidbody>();
        this.rb.transform.parent = null;

        this.holdingBugs = GetComponentsInChildren<HoldingBugController>();

        this.boostTime = this.speedPadBoostTime;
        this.currentStunTime = this.stunTime + 1f;
        this.currentBoostTime = this.boostTime + 1f;
    }

    public void MoveTo(Vector3 position)
    {
        if (this.holdingBugs.Any())
        {
            foreach (var bug in this.holdingBugs)
                bug.transform.position = position + bug.InitialOffset;
        }

        if (this.rb != null)
            this.rb.position = position;
        else
            this.transform.position = position;

        if (this.holdingBugs.Any())
        {
            foreach (var bug in this.holdingBugs)
                bug.ResetWiggle();
        }
    }

    public void Boost(float seconds)
    {
        this.boostTime = Mathf.Max(seconds, this.boostTime - this.currentBoostTime);
        this.currentBoostTime = 0;
    }

    private void Start()
    {
        (this.CurrentCheckpoint, this.NextCheckpoint) = CheckpointManager.GetDefaultCheckpoints();
    }

    public void RaceStart()
    {
        this.CanMove = true;

        if (this.holdingBugs.Any())
        {
            foreach (var bug in this.holdingBugs)
                bug.LetGo();
        }
    }

    private void Update()
    {
        this.UpdateTimeVariables();

        if (!this.CanMove)
            return;

        // Update isGrounded
        this.SetIsGrounded();

        // Set active velocity
        this.velocity = this.GetVelocity();

        // Turn
        var turnVelocity = this.isGrounded ? this.velocity : this.aerialTurnSpeed * 100;
        var newRotation = this.turnAmount * turnVelocity * this.turnSpeed * Time.deltaTime;

        this.transform.Rotate(0, newRotation, 0, Space.Self);

        // Rotate towards ground
        var floorNormal = this.GetFloorNormal();
        var targetRotation = Quaternion.FromToRotation(transform.up, floorNormal) * transform.rotation;
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, rotationSmoothing * 100 * Time.deltaTime);

        // Set collider position/rotation
        this.bodyCollider.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
    }

    private void FixedUpdate()
    {
        if (!this.CanMove)
            return;

        if (this.isGrounded)
        {
            this.rb.AddForce(this.transform.forward * this.velocity, ForceMode.Acceleration);
        }
        else
        {
            var floorNormal = this.GetFloorNormal(includeAerialChecks: true);
            rb.AddForce(floorNormal.normalized * -this.gravity * 100);
        }

        if (RaceManager.TryGetPosition(this, out var position))
            this.CurrentPosition = position;
    }

    private void LateUpdate()
    {
        // Update car position
        this.transform.position = this.rb.position;
    }

    private float GetVelocity()
    {
        // Check stunned
        if (this.currentStunTime <= this.stunTime)
            return 0f;

        // Check boost
        if (this.boostChecks.Any(c => c.Hit.transform != null))
            this.Boost(this.speedPadBoostTime);
        if (this.currentBoostTime <= this.boostTime)
            return this.boostSpeed * 100;

        // Check grounded
        if (!this.isGrounded)
            return 0f;

        var activeSpeed = this.forwardMovement > 0 ? this.forwardSpeed : this.reverseSpeed;

        // Check slow ground
        if (mainGroundCheck.Hit.transform != null && mainGroundCheck.Hit.transform.CompareTag(Tags.SLOW_GROUND))
            activeSpeed *= this.slowGroundModifier;

        return this.forwardMovement * activeSpeed * 100;
    }

    private void SetIsGrounded()
    {
        var grounded = this.groundChecks.Any(g => g.Hit.transform != null);
        if (this.isGrounded != grounded)
        {
            this.isGrounded = grounded;
            this.rb.drag = this.isGrounded ? this.groundDrag : this.airDrag;
        }
    }

    /// <summary>
    /// Get the normal of the floor based on all of this object's ground checks
    /// </summary>
    private Vector3 GetFloorNormal(bool includeAerialChecks = false)
    {
        var result = this.transform.up;

        var checks = !this.isGrounded && includeAerialChecks ? this.aerialGroundChecks : this.groundChecks;
        foreach (var groundCheck in checks)
        {
            if (groundCheck.Hit.transform != null)
                result += groundCheck.Hit.normal;
        }

        Debug.DrawLine(this.transform.position, this.transform.position + result, Color.red);

        return result;
    }

    public void Stun(float forSeconds)
    {
        this.stunTime = Mathf.Max(forSeconds, this.stunTime - this.currentStunTime);
        this.currentStunTime = 0;
    }

    private void UpdateTimeVariables()
    {
        if (this.currentStunTime <= this.stunTime)
            this.currentStunTime += Time.deltaTime;
        if (this.currentBoostTime <= this.boostTime)
            this.currentBoostTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.OUT_OF_BOUNDS))
            StartCoroutine(this.ResetToLastCheckpoint());
        else if (other.CompareTag(Tags.CHECKPOINT))
            this.UpdateCheckpoint(other);
    }

    private IEnumerator ResetToLastCheckpoint()
    {
        this.rb.isKinematic = true;

        this.rb.velocity = Vector3.zero;
        this.rb.position = this.CurrentCheckpoint.transform.position;

        this.transform.position = this.CurrentCheckpoint.transform.position;
        this.transform.rotation = this.CurrentCheckpoint.transform.rotation;

        yield return new WaitForSeconds(this.checkpointResetTime);

        this.rb.isKinematic = false;
    }

    private void UpdateCheckpoint(Collider other)
    {
        var checkpoint = other.GetComponent<Checkpoint>();
        if (checkpoint == null || checkpoint.Index != NextCheckpoint.Index)
            return;

        this.CurrentCheckpoint = checkpoint;

        // Get next checkpoint data
        var checkpointDto = CheckpointManager.GetNextCheckpoint(this.CurrentCheckpoint, this.IsAi);
        this.NextCheckpoint = checkpointDto.NextCheckpoint;

        if (checkpointDto.NewLap)
            this.NewLap();
    }

    private void NewLap()
    {
        this.LapsCompleted++;

        if (this.LapsCompleted == CheckpointManager.TotalLaps)
            this.EndOfRace();
        else if (this.TextController != null)
            StartCoroutine(this.TextController.ShowLap(this.LapsCompleted + 1));
    }

    private void EndOfRace()
    {
        RaceManager.LogFinish(this, out int finalPosition);

        var player = GetComponent<PlayerController>();
        if (player != null)
            player.RaceComplete();

        if (this.TextController != null)
            StartCoroutine(this.TextController.ShowRaceEnd(finalPosition));
    }

    private void OnDestroy()
    {
        if (this.rb != null)
            Destroy(this.rb.gameObject);
    }
}
