using System;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    [SerializeField] private CastDirection direction;
    [SerializeField] private float distance;
    [SerializeField] private LayerMask layers;
    [SerializeField] private LayerMask blockingLayers;

    [NonSerialized] public RaycastHit Hit;

    private void FixedUpdate()
    {
        Physics.Raycast(this.transform.position, this.transform.GetCastDirection(direction), out var hit, this.distance, this.layers + this.blockingLayers);

        if (hit.transform == null || (this.layers & (1 << hit.transform.gameObject.layer)) == 0)
            this.Hit = default;
        else
            this.Hit = hit;
    }

    private void OnDrawGizmosSelected()
    {
        var origin = this.transform.position;
        var dir = this.transform.GetCastDirection(direction);
        var destination = origin + (dir * this.distance);

        Gizmos.DrawLine(origin, destination);
    }
}
