using DG.Tweening;
using UnityEngine;

public class CameraDolly : MonoBehaviour
{
    [SerializeField] private Vector3 target;
    [SerializeField] private float speed;
    [SerializeField] private Ease easing;

    private void Start()
    {
        this.transform.DOMove(target, speed).SetEase(easing);
    }
}
