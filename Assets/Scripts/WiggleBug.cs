using DG.Tweening;
using UnityEngine;

public class WiggleBug : MonoBehaviour
{
    [Header("Wiggle")]
    [SerializeField] private Vector3 wiggleDistance;
    [SerializeField] private Vector2 wiggleSpeedBounds;
    [SerializeField] private Ease wiggleEasing = Ease.InOutSine;
    private Tween[] wiggleTweens = new Tween[0];

    protected void Start()
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
}
