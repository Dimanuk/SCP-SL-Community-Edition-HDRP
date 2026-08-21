using Knife.DeferredDecals;
using UnityEngine;

public class BloodController : MonoBehaviour
{
    public float decayTimeSeconds = 600f;

    public Color startColor;
    public Color endColor;

    public int gridSize;
    public int numberOfDecals;

    private bool _isDying;
    private Decal _decalScript;

    public float BloodAgeTimer { get; set; }

    private void Awake()
    {
        _decalScript = GetComponent<Decal>();

        if (_decalScript != null)
            _decalScript.UVTiling = Vector2.one / gridSize;
    }

    private void OnEnable()
    {
        if (_isDying)
            return;

        BloodAgeTimer = 0f;

        // Pick a random cell of the blood atlas (grid is addressed top row first).
        int index = Random.Range(0, numberOfDecals);
        int cellX = index % gridSize;
        int cellY = gridSize - index / gridSize - 1;

        if (_decalScript != null)
            _decalScript.UVOffset = new Vector2(cellX, cellY) / gridSize;
    }

    private void Update()
    {
        BloodAgeTimer += Time.deltaTime;

        if (_decalScript != null)
            _decalScript.InstancedColor = Color.Lerp(startColor, endColor, BloodAgeTimer / decayTimeSeconds);

        if (BloodAgeTimer >= decayTimeSeconds)
        {
            if (_isDying)
                Destroy(gameObject);
            else
                Destroy(this);
        }
    }

    public void DestroyDecal()
    {
        _isDying = true;
        BloodAgeTimer = 0f;
        decayTimeSeconds = 5f;

        if (_decalScript != null)
        {
            startColor = _decalScript.InstancedColor;
            endColor = Color.clear;
        }

        enabled = true;
    }
}
