using System;
using System.Linq;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private Checkpoint startCheckpoint;
    [SerializeField] private bool isCyclical = true;

    private static CheckpointManager _instance;

    public static int TotalLaps { get => GameController.Laps; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        this.SetCheckpointIndex(startCheckpoint);
    }

    private void SetCheckpointIndex(Checkpoint checkpoint, int index = 0)
    {
        checkpoint.Index = index;

        foreach (var child in checkpoint.NextCheckpoints)
        {
            if (child.Index >= 0)
                continue;

            this.SetCheckpointIndex(child, index + 1);
        }
    }

    public static (Checkpoint zeroCheckpoint, Checkpoint firstCheckpoint) GetDefaultCheckpoints() => _instance.GetDefaultCheckpointsLocal();

    private (Checkpoint zeroCheckpoint, Checkpoint firstCheckpoint) GetDefaultCheckpointsLocal() => (startCheckpoint, startCheckpoint.GetNextCheckpointForPlayer());

    public static NextCheckpointDto GetNextCheckpoint(Checkpoint checkpoint, bool forAI) => _instance.GetNextCheckpointLocal(checkpoint, forAI);

    private NextCheckpointDto GetNextCheckpointLocal(Checkpoint checkpoint, bool forAI)
    {
        var nextCheckpoint = forAI ? checkpoint.GetNextCheckpointForAI() : checkpoint.GetNextCheckpointForPlayer();

        if (this.isCyclical && checkpoint.Index == 0)
            return NextCheckpointDto.Lap(nextCheckpoint);
        else if (!this.isCyclical && checkpoint.NextCheckpoints.Max(c => c.Index == 0))
            return NextCheckpointDto.Lap(nextCheckpoint);
        else
            return NextCheckpointDto.Checkpoint(nextCheckpoint);
    }
}
