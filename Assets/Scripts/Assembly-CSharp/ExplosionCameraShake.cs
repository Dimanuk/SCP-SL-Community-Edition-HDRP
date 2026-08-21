using PostProcessing;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ExplosionCameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float force;
    public float deductSpeed = 2f;

    private CameraShake _cameraShake;
    public static ExplosionCameraShake singleton;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        if (TryGetComponent(out PostProcessVolume volume))
        {
            if (volume.sharedProfile != null)
            {
                _cameraShake = volume.sharedProfile.GetSetting <CameraShake>();
            }
        }
    }

    private void Update()
    {
        if (_cameraShake == null)
            return;

        force -= Time.deltaTime / deductSpeed;
        force = Mathf.Clamp01(force);

        if (_cameraShake.scanLineJitter != null)
            _cameraShake.scanLineJitter.value = force;

        if (_cameraShake.horizontalShake != null)
            _cameraShake.horizontalShake.value = force;

        if (_cameraShake.colorDrift != null)
            _cameraShake.colorDrift.value = force;
    }

    public void Shake(float explosionForce)
    {
        if (explosionForce > force)
            force = explosionForce;
    }

    private void OnDestroy()
    {
        if (singleton == this)
            singleton = null;
    }
}