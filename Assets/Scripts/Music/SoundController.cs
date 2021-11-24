using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float volume = 0.2f;

    private AudioSource[] sources;
    private Dictionary<Guid, AudioSource> playingSources;

    private void Awake()
    {
        this.sources = GetComponents<AudioSource>();
        this.playingSources = new Dictionary<Guid, AudioSource>();

        foreach (var source in this.sources)
            source.volume = this.volume;
    }

    public Guid PlayClip(AudioClip clip)
    {
        var availableSource = this.sources.FirstOrDefault(s => !s.isPlaying);
        if (availableSource == null)
            return Guid.Empty;

        var id = Guid.NewGuid();
        this.playingSources.Add(id, availableSource);

        availableSource.clip = clip;
        availableSource.Play();

        StartCoroutine(this.StopAfter(id, clip.length));

        return id;
    }

    private IEnumerator StopAfter(Guid id, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.StopSound(id);
    }

    public void StopSound(Guid sourceId)
    {
        if (!this.playingSources.TryGetValue(sourceId, out var source))
            return;

        source.Stop();
        this.playingSources.Remove(sourceId);
    }
}
