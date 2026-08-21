using UnityEngine;

public class HoloSight : MonoBehaviour
{
    public float backSpeed;

    public float intensity;

    public bool changePositions;

    public bool invertX;

    public float blurScale;

    public bool lockRotations;

    public float maxOffset;

    private Vector3 startSize;

    private Vector3 startRot;

    public Color mainColor;

    private float _xSize;

    private float _ySize;

    private static readonly int OffsetX;

    private static readonly int OffsetY;

    private void Awake()
    {
    }

    private void LateUpdate()
    {
    }
}
