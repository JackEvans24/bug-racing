using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicLoop : MonoBehaviour
{
    private AudioSource[] sources;
    [SerializeField] private MusicTrack track;
    [SerializeField] private bool playOnAwake;

    private bool loopStarted;
    private int sourceIndex;
    private double nextPlayTime;

    private void Awake()
    {
        this.sources = GetComponents<AudioSource>();

        foreach (var source in this.sources)
            source.clip = this.track.Clip;

        if (playOnAwake)
            this.Play();
    }

    public void Play()
    {
        this.sources[this.sourceIndex].Play();
        this.sourceIndex++;
        this.nextPlayTime = AudioSettings.dspTime;
        this.loopStarted = true;
    }

    private void Update()
    {
        if (!this.loopStarted)
            return;

        if (!this.sources[this.sourceIndex].isPlaying)
        {
            this.nextPlayTime += this.track.LoopPoint;
            sources[this.sourceIndex].PlayScheduled(this.nextPlayTime);
            this.sourceIndex = 1 - this.sourceIndex;
        }
    }
}
