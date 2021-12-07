using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(TMP_Text))]
public class RaceTextController : MonoBehaviour
{
    private CanvasGroup canvas;
    private TMP_Text textField;

    [SerializeField] private float readyFadeTime = 1f;
    [SerializeField] private float readyGrowSize = 1.5f;
    [SerializeField] private float goFadeTime = 0.6f;
    [SerializeField] private float lapFadeTime = 0.5f;
    [SerializeField] private float lapSustainTime = 1.2f;
    [SerializeField] private float finishFadeTime = 0.5f;
    [SerializeField] private float finishSustainTime = 1.2f;

    private void Awake()
    {
        this.canvas = GetComponent<CanvasGroup>();
        this.textField = GetComponent<TMP_Text>();
    }

    public IEnumerator ShowRaceReady()
    {
        this.canvas.alpha = 0;
        this.textField.text = "Ready";
        var originalScale = this.textField.transform.localScale;

        var scalingTween = this.textField.transform.DOScale(this.textField.transform.localScale * this.readyGrowSize, this.readyFadeTime * 3);

        var fadeTween = this.canvas.DOFade(1f, this.readyFadeTime);

        yield return fadeTween.WaitForCompletion();

        yield return new WaitForSeconds(this.readyFadeTime);

        fadeTween = this.canvas.DOFade(0f, this.readyFadeTime);

        yield return fadeTween.WaitForCompletion();
        yield return scalingTween.WaitForCompletion();

        this.textField.transform.localScale = originalScale;
    }

    public IEnumerator ShowRaceStart()
    {
        this.canvas.alpha = 0;
        this.textField.text = "Go";

        yield return new WaitForEndOfFrame();
        this.canvas.alpha = 1;

        yield return new WaitForSeconds(this.goFadeTime);

        var tween = this.canvas.DOFade(0f, this.goFadeTime);
        yield return tween.WaitForCompletion();
    }

    public IEnumerator ShowLap(int lap)
    {
        this.canvas.alpha = 0;
        this.textField.text = $"Lap {lap}";

        var tween = this.canvas.DOFade(1f, this.lapFadeTime);
        yield return tween.WaitForCompletion();

        yield return new WaitForSeconds(this.lapSustainTime);

        tween = this.canvas.DOFade(0f, this.lapFadeTime);
        yield return tween.WaitForCompletion();
    }

    public IEnumerator ShowRaceEnd(int position)
    {
        this.canvas.alpha = 0;
        var positionText = PositionHelpers.GetPositionText(position);
        this.textField.text = $"Finished!\r\n{positionText}";

        var tween = this.canvas.DOFade(1f, this.finishFadeTime);
        yield return tween.WaitForCompletion();
    }
}
