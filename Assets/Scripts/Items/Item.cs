using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
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
