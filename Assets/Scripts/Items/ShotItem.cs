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

    private new void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
        Destroy(this.gameObject, this.lifetime);

        base.Awake();
    }

    private void Update()
    {
        this.Bounce();
    }

    private void FixedUpdate()
    {
        if (this.groundChecks.All(g => g.Hit.transform == null))
            this.rb.AddForce(this.transform.up * this.gravity * -100);

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
