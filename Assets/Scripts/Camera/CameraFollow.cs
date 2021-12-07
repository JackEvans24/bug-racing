using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [NonSerialized] public bool ReverseView;

    [Header("Forward camera")]
    [SerializeField] private float smoothing = 0.125f;
    [SerializeField] private float distance = 5;
    [SerializeField] private float height = 3;
    [SerializeField] private float lookHeight = 1;

    [Header("Reverse camera")]
    [SerializeField] private float reverseSmoothing = 0.125f;
    [SerializeField] private float reverseDistance = 5;
    [SerializeField] private float reverseHeight = 3;

    private Vector3 currentDefaultPosition, currentRearPosition;
    private Vector3 currentDefaultVelocity, currentRearVelocity, currentLookAt, currentLookAtVelocity;

    private void FixedUpdate()
    {
        var defaultTargetPosition = this.target.position + (this.distance * -this.target.forward) + (this.height * this.target.up);
        var rearTargetPosition = this.target.position + (this.reverseDistance * this.target.forward) + (this.reverseHeight * this.target.up);

        this.currentDefaultPosition = Vector3.SmoothDamp(this.currentDefaultPosition, defaultTargetPosition, ref currentDefaultVelocity, this.smoothing);
        this.currentRearPosition = Vector3.SmoothDamp(this.currentRearPosition, rearTargetPosition, ref currentRearVelocity, this.reverseSmoothing);

        this.transform.position = this.ReverseView ? this.currentRearPosition : this.currentDefaultPosition;

        currentLookAt = Vector3.SmoothDamp(this.currentLookAt, this.target.position + (this.target.up * lookHeight), ref this.currentLookAtVelocity, this.smoothing);
        this.transform.LookAt(currentLookAt, this.target.up);
    }
}
