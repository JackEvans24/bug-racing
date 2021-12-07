using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ItemParticles : MonoBehaviour
{
    private ParticleSystem particles;

    private void Awake()
    {
        this.particles = GetComponent<ParticleSystem>();
        Destroy(this.gameObject, this.particles.main.duration * 5);
    }
}
