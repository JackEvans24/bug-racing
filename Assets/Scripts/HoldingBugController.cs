using DG.Tweening;
using UnityEngine;

public class HoldingBugController : WiggleBug
{
    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] LineRenderer line;

    [Header("Positioning")]
    public Vector3 InitialOffset;

    [Header("Fly off")]
    [SerializeField] private Vector3 letGoDistance;
    [SerializeField] private float letGoDuration;
    [SerializeField] private Ease letGoEasing = Ease.InSine;

    private bool holding = true;

    private void Update()
    {
        if (!this.holding)
            return;

        this.line.SetPositions(new[] { this.transform.position, this.player.position });
    }

    public void LetGo()
    {
        if (!this.holding)
            return;

        this.holding = false;

        this.line.enabled = false;

        this.transform.DOMove(this.transform.position + this.letGoDistance, this.letGoDuration).SetEase(this.letGoEasing);
        this.transform.DOScale(0.01f, this.letGoDuration).SetEase(this.letGoEasing);
    }
}
