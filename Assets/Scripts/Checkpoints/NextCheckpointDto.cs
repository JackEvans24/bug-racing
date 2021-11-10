public class NextCheckpointDto
{
    private NextCheckpointDto(Checkpoint checkpoint, bool newLap = false)
    {
        this.NextCheckpoint = checkpoint;
        this.NewLap = newLap;
    }

    public Checkpoint NextCheckpoint { get; set; }
    public bool NewLap { get; set; }

    public static NextCheckpointDto Checkpoint(Checkpoint c) => new NextCheckpointDto(c, false);
    public static NextCheckpointDto Lap(Checkpoint c) => new NextCheckpointDto(c, true);
}
