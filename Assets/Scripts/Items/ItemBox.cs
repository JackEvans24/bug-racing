using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class ItemBox : MonoBehaviour
{
    [SerializeField] private float downTime;

    private float currentDownTime;
    private bool isDown;

    private Collider boxCollider;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        this.boxCollider = this.GetComponent<Collider>();
        this.meshRenderer = GetComponent<MeshRenderer>();

        this.currentDownTime = downTime + 1;
    }

    private void Update()
    {
        if (this.currentDownTime <= this.downTime)
            this.currentDownTime += Time.deltaTime;
        else if (this.isDown)
            this.ToggleActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
            this.ToggleActive(false);
    }

    private void ToggleActive(bool active)
    {
        if (this.isDown == !active)
            return;

        if (!active)
            this.currentDownTime = 0f;

        this.isDown = !active;
        this.boxCollider.enabled = active;
        this.meshRenderer.enabled = active;
    }
}
