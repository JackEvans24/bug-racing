using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BoostColour : MonoBehaviour
{
    [SerializeField, ColorUsage(showAlpha: true)] private Color boostColor;
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private Ease easing = Ease.InOutSine;

    private void Awake()
    {
        var mesh = GetComponent<MeshRenderer>();
        mesh.material.DOColor(this.boostColor, this.fadeTime).SetLoops(-1, LoopType.Yoyo).SetEase(this.easing);
    }
}
