using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraDolly : MonoBehaviour
{
    [SerializeField] private Vector3 target;
    [SerializeField] private float speed;
    [SerializeField] private float wait;
    [SerializeField] private Ease easing;

    private Camera cam;

    private void Awake()
    {
        this.cam = GetComponent<Camera>();
    }

    public void Prewarm() => this.cam.enabled = true;

    public IEnumerator PlayDolly()
    {
        this.cam.enabled = true;

        this.transform.DOMove(this.target, this.speed).SetEase(this.easing);

        yield return new WaitForSeconds(this.speed + this.wait);

        this.cam.enabled = false;
    }
}
