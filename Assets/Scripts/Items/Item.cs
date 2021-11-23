using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private float finalScale = 3f;
    [SerializeField] private float scaleTime = 0.2f;

    protected void Awake()
    {
        this.transform.DOScale(this.finalScale, this.scaleTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
            Destroy(this.gameObject);
        else if (other.CompareTag(Tags.OUT_OF_BOUNDS))
            Destroy(this.gameObject);
        else if (other.CompareTag(Tags.HAZARD))
            Destroy(this.gameObject);
    }
}
