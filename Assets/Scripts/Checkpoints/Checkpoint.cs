using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Do not change")]
    public int Index = -1;

    [Header("Order")]
    [SerializeField] private Checkpoint PrimaryNextCheckpoint;
    [SerializeField] private Checkpoint[] OtherNextCheckpoints;
    public Checkpoint[] NextCheckpoints
    {
        get
        {
            var checkpoints = new[] { PrimaryNextCheckpoint };
            if (OtherNextCheckpoints.Any())
                checkpoints = checkpoints.Concat(OtherNextCheckpoints).ToArray();
            return checkpoints;
        }
    }

    [Header("Position")]
    public float Offset;
    [Min(0)] public float HardBoundary;
    [Min(0)] public float SoftBoundary;

    public Vector3 CheckpointCenter => this.transform.position + (this.transform.right * this.Offset);
    public Vector3 MinimumBoundary => this.CheckpointCenter + (this.transform.right * -this.HardBoundary);
    public Vector3 MaximumBoundary => this.CheckpointCenter + (this.transform.right * this.HardBoundary);
    public Vector3 MinimumInnerBoundary => this.CheckpointCenter + (this.transform.right * -this.SoftBoundary);
    public Vector3 MaximumInnerBoundary => this.CheckpointCenter + (this.transform.right * this.SoftBoundary);

    public Checkpoint GetNextCheckpointForPlayer() => this.PrimaryNextCheckpoint;
    public Checkpoint GetNextCheckpointForAI() => this.NextCheckpoints[Random.Range(0, this.NextCheckpoints.Length)];

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(CheckpointCenter, 2f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(MinimumBoundary, MaximumBoundary);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(MinimumInnerBoundary + (transform.up * 5), MaximumInnerBoundary + (transform.up * 5));
    }
}
