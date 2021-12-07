using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BoostColour : MonoBehaviour
{
    [SerializeField, ColorUsage(showAlpha: true)] private Color boostColor;
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private Ease easing = Ease.InOutSine;
    [SerializeField] private bool addDelay = false;

    private void Awake()
    {
        var mesh = GetComponent<MeshRenderer>();
        var delay = this.addDelay ? Random.Range(0, this.fadeTime) : 0;
        mesh.material.DOColor(this.boostColor, this.fadeTime).SetDelay(delay).SetLoops(-1, LoopType.Yoyo).SetEase(this.easing);
    }
}
