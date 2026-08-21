using Knife.DeferredDecals;
using UnityEngine;

public class BulletHoleController : MonoBehaviour
{
    [SerializeField]
    private Material[] _materials;

    [SerializeField]
    private float _minScale = 0.8f;

    [SerializeField]
    private float _maxScale = 1.2f;

    private Decal _decal;
    private Transform _transform;

    private void Awake()
    {
        _decal = GetComponent<Decal>();
        _transform = transform;
    }

    public void SetupDecal(RaycastHit hit)
    {
        Decal.SetTransformAlongSurface(_transform, hit);

        float scale = Random.Range(_minScale, _maxScale);
        Vector3 ls = _transform.localScale;

        _transform.localScale = new Vector3(ls.x * scale, ls.y, ls.z * scale);
        if (_materials != null && _materials.Length > 0)
        {
            _decal.DecalMaterial = _materials[Random.Range(0, _materials.Length)];
        }
        _transform.Rotate(-90f, 0f, 0f, Space.Self);
        _transform.Rotate(0f, Random.Range(0f, 360f), 0f, Space.Self);
        _transform.SetParent(hit.transform, true);
    }
}