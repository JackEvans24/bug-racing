using DG.Tweening;
using System;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [Header("Initial scaling")]
    [SerializeField] protected float finalScale = 3f;
    [SerializeField] protected float scaleTime = 0.2f;

    [Header("SFX")]
    [SerializeField] private AudioClip awakeClip;
    [SerializeField] private AudioClip destroyClip;

    [Header("FX")]
    [SerializeField] protected float effectTime;
    [SerializeField] private GameObject particles;

    private Guid startNoiseId = Guid.Empty;

    protected void Awake()
    {
        this.startNoiseId = GameController.PlaySound(this.awakeClip);
        this.transform.DOScale(this.finalScale, this.scaleTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
            this.DestroyWithSound();
        else if (other.CompareTag(Tags.HAZARD))
            this.DestroyWithSound();
        else if (other.CompareTag(Tags.OUT_OF_BOUNDS))
            this.DestroyWithSound();
    }

    protected void DestroyWithSound()
    {
        if (this.startNoiseId != Guid.Empty)
            GameController.StopSound(this.startNoiseId);
        
        GameController.PlaySound(this.destroyClip);

        Destroy(this.gameObject);
    }

    protected void StartParticles(Transform parent = null)
    {
        if (this.particles == null)
            return;

        var target = parent == null ? this.transform : parent;
        var particles = Instantiate(this.particles, target.position, target.rotation);

        if (parent != null)
            particles.transform.parent = parent;

        particles.transform.localPosition = Vector3.zero;
    }
}
