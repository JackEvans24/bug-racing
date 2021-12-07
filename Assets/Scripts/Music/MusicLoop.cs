using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicLoop : MonoBehaviour
{
    private AudioSource[] sources;
    private MusicTrack track;

    private bool loopStarted;
    private int sourceIndex;
    private double nextPlayTime;

    private void Awake()
    {
        this.sources = GetComponents<AudioSource>();
    }

    public void UpdateTrack(MusicTrack track, bool play = true)
    {
        if (this.track == track)
            return;

        this.loopStarted = false;
        this.sourceIndex = 0;
        this.nextPlayTime = 0;

        this.track = track;

        foreach (var source in this.sources)
        {
            source.Stop();
            source.clip = this.track.Clip;
        }

        if (play)
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
