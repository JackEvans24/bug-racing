using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [SerializeField] private float smoothing = 0.125f;
    [SerializeField] private float distance = 5;
    [SerializeField] private float height = 3;
    [SerializeField] private float lookHeight = 1;

    private Vector3 currentVelocity, currentLookAt, currentLookAtVelocity;

    private void FixedUpdate()
    {
        var targetPosition = this.target.position + (this.distance * -this.target.forward) + (this.height * this.target.up);

        this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPosition, ref currentVelocity, this.smoothing);

        currentLookAt = Vector3.SmoothDamp(this.currentLookAt, this.target.position + (this.target.up * lookHeight), ref this.currentLookAtVelocity, this.smoothing);
        this.transform.LookAt(currentLookAt, this.target.up);
    }
}
