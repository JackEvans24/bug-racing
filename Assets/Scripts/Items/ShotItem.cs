using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShotItem : Item
{
    [Header("Shoot variables")]
    [SerializeField] private Raycaster[] forwardChecks;
    [SerializeField] private float velocity = 3f;
    [SerializeField] private float lifetime = 3f;

    private Rigidbody rb;
    private bool isGrounded;

    private new void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
        StartCoroutine(this.DestroyAfterLifetime());

        base.Awake();
    }

    private void Update()
    {
        this.Bounce();
    }

    private void FixedUpdate()
    {
        this.rb.velocity = this.transform.forward * this.velocity;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
        {
            var car = other.GetComponent<CarMovement>();
            if (car != null)
            {
                this.StartParticles(car.transform);
                car.Stun(this.effectTime);
            }
        }

        base.OnTriggerEnter(other);
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(this.lifetime);
        this.DestroyWithSound();
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
