using UnityEngine;
using MapGeneration;

public class LightBlink : MonoBehaviour
{
    public float noshadowIntensMultiplier = 1f;
    public float minFlickerTimeRange = 0.1f;
    public float maxFlickerTimeRange = 1.5f;
    public float turnOnSpeed = 0.3f;
    public float maxIntensityDecreaseMultiplier = 0.5f;

    public Light[] lightGroup;

    private float[] _startLightIntensity;
    private Light _defaultLight;
    private float _frequency;
    private FlickerableLightController _controller;
    private bool _hasParentController;

    private void Awake()
    {
        var room = GetComponentInParent <RoomIdentifier > ();
        if (room != null)
            _controller = room.GetComponentInChildren<FlickerableLightController>();

        _hasParentController = _controller != null;

        if (lightGroup == null || lightGroup.Length == 0)
        {
            var selfLight = GetComponent <Light>();
            lightGroup = new Light[] { selfLight };
        }

        _startLightIntensity = new float[lightGroup.Length];
        for (int i = 0; i < lightGroup.Length; i++)
        {
            if (lightGroup[i] != null)
                _startLightIntensity[i] = lightGroup[i].intensity;
        }

        if (lightGroup.Length > 0)
            _defaultLight = lightGroup[0];
        else
            enabled = false; 
    }

    private void Start()
    {
        if (QualitySettings.shadows != ShadowQuality.Disable)
            noshadowIntensMultiplier = 1f;
    }

    private void Update()
    {
        if (_defaultLight == null || !_defaultLight.enabled)
            return;

        _frequency -= Time.deltaTime;
        if (_frequency <= 0f)
        {
            _frequency = Random.Range(minFlickerTimeRange, maxFlickerTimeRange);

            for (int i = 0; i < lightGroup.Length; i++)
            {
                var light = lightGroup[i];
                if (light == null)
                    continue;

                float controllerMultiplier = _hasParentController && _controller != null
                    ? _controller.LightIntensityMultiplier
                    : 1f;

                float maxDip = _startLightIntensity[i] * controllerMultiplier * maxIntensityDecreaseMultiplier;
                float dip = Random.Range(0f, maxDip);

                light.intensity = Mathf.Max(light.intensity - dip, 0f);
            }
        }

        for (int i = 0; i < lightGroup.Length; i++)
        {
            var light = lightGroup[i];
            if (light == null)
            {
                Destroy(this);
                return;
            }

            float controllerMultiplier = _hasParentController && _controller != null
                ? _controller.LightIntensityMultiplier
                : 1f;

            float targetIntensity = _startLightIntensity[i] * controllerMultiplier;

            light.intensity = Mathf.Lerp(light.intensity, targetIntensity, turnOnSpeed);
        }
    }
}