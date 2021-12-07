using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Music Track")]
public class MusicTrack : ScriptableObject
{
    public AudioClip Clip;
    public int BPM;
    public int TimeSignature;
    public int BarsLength;

    public float LoopPoint { get => (60f * this.BarsLength * this.TimeSignature) / this.BPM; }
}
