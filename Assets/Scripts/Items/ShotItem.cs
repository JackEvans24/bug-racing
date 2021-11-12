using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShotItem : Item
{
    [SerializeField] private Raycaster[] groundChecks;
    [SerializeField] private Raycaster[] forwardChecks;
    [SerializeField] private float velocity = 3f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float gravity = 0.6f;
    [SerializeField] private float stunTime = 1f;

    private Rigidbody rb;
    private bool isGrounded;

    private void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
        Destroy(this.gameObject, this.lifetime);
    }

    private void Update()
    {
        //var floorNormal = this.GetFloorNormal();
        //this.transform.rotation = Quaternion.FromToRotation(transform.up, floorNormal) * transform.rotation;

        this.Bounce();
    }

    private void FixedUpdate()
    {
        if (!this.groundChecks.Any(g => g.Hit.transform != null))
        {
            //var floorNormal = this.GetFloorNormal();
            //this.rb.AddForce(floorNormal.normalized * this.gravity * -100);
            this.rb.AddForce(this.transform.up * this.gravity * -100);
        }

        this.rb.velocity = this.transform.forward * this.velocity;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
        {
            var car = other.GetComponent<CarMovement>();
            if (car != null)
                car.Stun(this.stunTime);
        }

        base.OnTriggerEnter(other);
    }

    private Vector3 GetFloorNormal()
    {
        var result = this.transform.up;

        foreach (var groundCheck in this.groundChecks)
        {
            if (groundCheck.Hit.transform != null)
                result += groundCheck.Hit.normal;
        }

        Debug.DrawLine(this.transform.position, this.transform.position + result, Color.red);

        return result;
    }

    private void Bounce()
    {
        var maxBounce = this.transform.forward;

        foreach (var check in this.forwardChecks)
        {
            if (check.Hit.transform == null)
                continue;

            var bounce = Vector3.Reflect(this.transform.forward, check.Hit.normal);
            if (Vector3.Dot(this.transform.forward, bounce) < Vector3.Dot(this.transform.forward, maxBounce))
                maxBounce = bounce;
        }

        this.transform.forward = maxBounce;
    }
}
