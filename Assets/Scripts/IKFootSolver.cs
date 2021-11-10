using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform body;
    [SerializeField] private IKFootSolver[] otherFeet;

    [Header("Step variables")]
    [SerializeField] private Vector3 footOffset;
    [SerializeField] float speed = 1;
    [SerializeField] float stepDistance = 4;
    [SerializeField] float stepLength = 4;
    [SerializeField] float stepHeight = 1;
    [SerializeField] private float rayDistance = 4;
    [SerializeField] private LayerMask groundLayer;

    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

    public bool IsMoving { get => this.lerp < 1; }
    private bool OtherFeetMoving { get => this.otherFeet.Any(f => f.IsMoving); }

    private void Start()
    {
        this.currentPosition = this.newPosition = this.oldPosition = this.transform.position;
        this.currentNormal = this.newNormal = this.oldNormal = this.transform.up;
        this.lerp = 1;
    }

    void Update()
    {
        this.transform.position = this.currentPosition;
        this.transform.up = this.currentNormal;

        var ray = new Ray(this.body.position + (this.footOffset.x * this.body.right) + (this.footOffset.z * this.body.forward), -this.body.up);
        if (Physics.Raycast(ray, out var info, this.rayDistance, this.groundLayer))
        {
            if (Vector3.Distance(this.newPosition, info.point) > this.stepDistance && !this.OtherFeetMoving && !this.IsMoving)
            {
                this.lerp = 0;
                int direction = this.body.InverseTransformPoint(info.point).z > this.body.InverseTransformPoint(this.newPosition).z ? 1 : -1;
                this.newPosition = info.point + (this.body.forward * this.stepLength * direction) + (this.footOffset.x * this.body.right) + (this.footOffset.z * this.body.forward);
                this.newNormal = info.normal;
            }
        }

        if (this.IsMoving)
        {
            Vector3 tempPosition = Vector3.Lerp(this.oldPosition, this.newPosition, this.lerp);
            tempPosition.y += Mathf.Sin(this.lerp + Mathf.PI) * this.stepHeight;

            this.currentPosition = tempPosition;
            this.currentNormal = Vector3.Lerp(this.oldNormal, this.newNormal, this.lerp);
            this.lerp += Time.deltaTime * this.speed;
        }
        else
        {
            this.oldPosition = this.newPosition;
            this.oldNormal = this.newNormal;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        var origin = this.body.position + (this.body.right * this.footOffset.x) + (this.body.forward * this.footOffset.z);
        Gizmos.DrawWireSphere(origin, 0.5f);
        Gizmos.DrawLine(origin, origin + (this.body.up * -this.rayDistance));
    }
}
