using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : Item
{
    [SerializeField] private float stunTime = 1f;

    [Header("Fly off")]
    [SerializeField] private Vector3 upDistance;
    [SerializeField] private float upDuration = 1f;
    [SerializeField] private Ease upEasing = Ease.InSine;

    [Header("Appear")]
    [SerializeField] private Vector3 appearOffset;
    [SerializeField] private float appearDuration = 1.5f;
    [SerializeField] private Ease appearEasing = Ease.OutSine;
    [SerializeField] private AudioClip appearNoise;

    [Header("Pause")]
    [SerializeField] private float pauseDuration = 1.5f;

    [Header("Attack")]
    [SerializeField] private float attackDuration = 0.1f;
    [SerializeField] private Ease attackEasing = Ease.InSine;
    [SerializeField] private AudioClip attackNoise;

    private Transform target;

    private void Start()
    {
        StartCoroutine(this.Fire());
    }

    private void Update()
    {
        if (this.target != null)
            this.transform.LookAt(this.target, this.target.up);
    }

    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(this.scaleTime);

        // Fly away from current car
        var flyTo = this.GetOffset(this.upDistance, this.transform);
        this.transform.DOMove(flyTo, this.upDuration).SetEase(this.upEasing);
        this.transform.DOScale(0.01f, this.upDuration).SetEase(this.upEasing);

        yield return new WaitForSeconds(this.upDuration);

        // Get first place car
        RaceManager.TryGetRacer(1, out var car);
        while (car == null)
        {
            yield return new WaitForEndOfFrame();
            RaceManager.TryGetRacer(1, out car);
        }
        this.target = car.transform;

        // Appear above 1st place
        this.transform.parent = this.target;
        this.transform.position = this.GetOffset(this.appearOffset, this.target);
        this.transform.DOScale(this.finalScale, this.appearDuration).SetEase(this.appearEasing);

        GameController.PlaySound(this.appearNoise);

        yield return new WaitForSeconds(this.appearDuration + this.pauseDuration);

        this.transform.DOLocalMove(Vector3.zero, this.attackDuration).SetLoops(5, LoopType.Yoyo).SetEase(this.attackEasing);

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(this.attackDuration);

            GameController.PlaySound(this.attackNoise);

            if (i == 0)
                car.Stun(this.stunTime);

            yield return new WaitForSeconds(this.attackDuration);
        }

        this.DestroyWithSound();
    }

    private Vector3 GetOffset(Vector3 offset, Transform relativeTo) =>
        relativeTo.position +
        (offset.x * relativeTo.right) +
        (offset.y * relativeTo.up) +
        (offset.z * relativeTo.forward);
}
