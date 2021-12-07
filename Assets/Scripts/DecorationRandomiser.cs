using System.Linq;
using UnityEngine;

public class DecorationRandomiser : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private Vector2 rotationBounds;
    [SerializeField] private Vector2 scaleBounds;

    [Header("Colours")]
    [SerializeField] private MeshRenderer[] meshes;
    [SerializeField] private Material[] materials;

    void Awake()
    {
        if (rotationBounds.x != rotationBounds.y)
            this.transform.Rotate(Vector3.up * Random.Range(rotationBounds.x, rotationBounds.y));

        if (scaleBounds.x != scaleBounds.y)
            this.transform.localScale = Vector3.one * Random.Range(scaleBounds.x, scaleBounds.y);

        if (this.meshes.Any() && this.materials.Any())
        {
            var mat = this.materials[Random.Range(0, this.materials.Length)];
            foreach (var mesh in this.meshes)
                mesh.material = mat;
        }
    }
}
