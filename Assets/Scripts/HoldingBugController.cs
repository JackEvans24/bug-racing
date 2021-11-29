using DG.Tweening;
using UnityEngine;

public class HoldingBugController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] LineRenderer line;

    [Header("Positioning")]
    public Vector3 InitialOffset;

    [Header("Wiggle")]
    [SerializeField] private Vector3 wiggleDistance;
    [SerializeField] private Vector2 wiggleSpeedBounds;
    [SerializeField] private Ease wiggleEasing = Ease.InOutSine;

    [Header("Fly off")]
    [SerializeField] private Vector3 letGoDistance;
    [SerializeField] private float letGoDuration;
    [SerializeField] private Ease letGoEasing = Ease.InSine;

    private bool holding = true;
    private Tween[] wiggleTweens = new Tween[0];

    private void Start()
    {
        this.ResetWiggle();
    }

    public void ResetWiggle()
    {
        foreach (var tween in this.wiggleTweens)
            tween.Kill();

        var xTween = this.transform
            .DOMoveX(this.transform.position.x + this.wiggleDistance.x, Random.Range(this.wiggleSpeedBounds.x, this.wiggleSpeedBounds.y))
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(this.wiggleEasing);
        var yTween = this.transform
            .DOMoveY(this.transform.position.y + this.wiggleDistance.y, Random.Range(this.wiggleSpeedBounds.x, this.wiggleSpeedBounds.y))
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(this.wiggleEasing);
        var zTween = this.transform
            .DOMoveZ(this.transform.position.z + this.wiggleDistance.z, Random.Range(this.wiggleSpeedBounds.x, this.wiggleSpeedBounds.y))
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(this.wiggleEasing);

        this.wiggleTweens = new[] { xTween, yTween, zTween };
    }

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
