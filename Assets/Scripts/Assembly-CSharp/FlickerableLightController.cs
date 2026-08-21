using MapGeneration;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FlickerableLightController : NetworkBehaviour
{
    private List<FlickerableLight> _cachedLights;
    private List<ReflectionProbe> _cachedReflectionProbes;
    private bool _cacheSet;

    public const float HeightRadius = 100f;

    public static bool WarheadEnabled;

    private float _flickerDuration;

    [SyncVar(hook = nameof(LightsHook))]
    public bool LightsEnabled;

    public static readonly Color DefaultWarheadColor = new Color(1f, 0.2f, 0.2f);

    public static readonly List<FlickerableLightController> Instances = new List<FlickerableLightController>();

    private Light[] _allLights;
    private float[] _allLightDefaultIntensity;
    private bool _initializedLightIntensity;

    [SyncVar(hook = nameof(UpdateLightsIntensity))]
    private float _lightIntensityMultiplier = 1f;

    [SyncVar(hook = nameof(UpdateWarheadLight))]
    private Color _warheadLightColor = DefaultWarheadColor;

    [SyncVar(hook = nameof(UpdateWarheadLightOverride))]
    private bool _warheadLightOverride;

    private List<FlickerableLight> LightsInRoom
    {
        get
        {
            PrepareCache();
            return _cachedLights;
        }
    }

    private List<ReflectionProbe> ReflectionProbes
    {
        get
        {
            PrepareCache();
            return _cachedReflectionProbes;
        }
    }

    public RoomIdentifier Room { get; private set; }

    public float LightIntensityMultiplier
    {
        get => _lightIntensityMultiplier;
        set
        {
            if (!NetworkServer.active)
                throw new System.InvalidOperationException("Tried changing light intensity on client.");

            _lightIntensityMultiplier = value;
        }
    }

    public Color WarheadLightColor
    {
        get => _warheadLightColor;
        set
        {
            if (!NetworkServer.active)
                throw new System.Runtime.Remoting.ServerException("Tried changing warhead light color on client.");

            _warheadLightColor = value;
        }
    }

    public bool WarheadLightOverride
    {
        get => _warheadLightOverride;
        set
        {
            if (!NetworkServer.active)
                throw new System.Runtime.Remoting.ServerException("Tried changing warhead light override on client.");

            _warheadLightOverride = value;
        }
    }

    private void Awake()
    {
        WarheadEnabled = false;
    }

    private void Start()
    {
        if (NetworkServer.active)
        {
            LightsEnabled = true;
        }
    }

    private void OnEnable()
    {
        Instances.Add(this);
    }

    private void OnDisable()
    {
        Instances.Remove(this);
    }

    [Server]
    public static void ServerSendDataToClient(NetworkConnection conn)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'System.Void FlickerableLightController::ServerSendDataToClient(Mirror.NetworkConnection)' called when server was not active");
            return;
        }

        foreach (var instance in Instances)
        {
            if (instance == null) continue;

            if (instance._warheadLightOverride)
                instance.TargetRpcUpdateWarheadOverride(conn, instance._warheadLightOverride);

            if (instance._warheadLightColor != DefaultWarheadColor)
                instance.TargetRpcUpdateWarheadLight(conn, instance._warheadLightColor);
        }
    }

    [TargetRpc]
    private void TargetRpcUpdateWarheadLight(NetworkConnection conn, Color color)
    {
        UpdateWarheadLight(default, color);
    }

    [TargetRpc]
    private void TargetRpcUpdateWarheadOverride(NetworkConnection conn, bool state)
    {
        UpdateWarheadLightOverride(default, state);
    }

    private void PrepareCache()
    {
        if (_cacheSet) return;

        Transform parent = transform.parent;

        _cachedLights = new List<FlickerableLight>();
        _cachedReflectionProbes = new List<ReflectionProbe>();

        var lights = parent.GetComponentsInChildren<FlickerableLight>(true);
        foreach (var light in lights)
        {
            _cachedLights.Add(light);
        }

        var probes = parent.GetComponentsInChildren<ReflectionProbe>(true);
        foreach (var probe in probes)
        {
            if (probe.mode != ReflectionProbeMode.Realtime)
                continue;

            probe.shadowDistance = probe.intensity;
            _cachedReflectionProbes.Add(probe);
        }

        _cacheSet = true;
    }

    private void FixedUpdate()
    {
        if (NetworkServer.active && _flickerDuration > 0f)
        {
            _flickerDuration -= Time.fixedDeltaTime;

            if (_flickerDuration <= 0f)
            {
                SetLights(true);
            }
        }
    }

    private void SetLights(bool state)
    {
        if (NetworkServer.active)
            LightsEnabled = state;

        SetProbeIntensity(state);

        PrepareCache();
        foreach (var light in _cachedLights)
        {
            if (light != null)
                light.SetFlickering(!state);
        }
    }

    private void SetProbeIntensity(bool state)
    {
        PrepareCache();

        foreach (var probe in ReflectionProbes)
        {
            if (probe != null)
            {
                probe.intensity = state ? probe.shadowDistance : 0f;
            }
        }
    }

    private void LightsHook(bool oldValue, bool newValue)
    {
        SetLights(newValue);
    }

    [Server]
    public void ServerFlickerLights(float dur)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'System.Void FlickerableLightController::ServerFlickerLights(System.Single)' called when server was not active");
            return;
        }

        if (dur <= 0f)
        {
            _flickerDuration = 0f;
            SetLights(true);
        }
        else
        {
            _flickerDuration = dur;
            SetLights(false);
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        SeedSynchronizer.OnMapGenerated += () =>
        {
            foreach (var instance in Instances)
            {
                if (instance != null)
                    instance.Room = RoomIdUtils.RoomAtPosition(instance.transform.position);
            }
        };
    }

    public static bool IsInDarkenedRoom(Vector3 positionToCheck)
    {
        RoomIdentifier room = RoomIdUtils.RoomAtPosition(positionToCheck);
        if (room == null)
            room = RoomIdUtils.RoomAtPositionRaycasts(positionToCheck);

        foreach (var controller in Instances)
        {
            if (controller == null || controller.LightsEnabled || controller.Room != room)
                continue;

            if (Mathf.Abs(controller.transform.position.y - positionToCheck.y) > HeightRadius)
                continue;

            return true;
        }

        return false;
    }

    private void UpdateLightsIntensity(float oldValue, float newValue)
    {
        if (!_initializedLightIntensity)
        {
            RoomIdentifier room = GetComponentInParent<RoomIdentifier>();
            if (room != null)
            {
                _allLights = room.GetComponentsInChildren<Light>();
                if (_allLights != null && _allLights.Length > 0)
                {
                    _allLightDefaultIntensity = new float[_allLights.Length];
                    for (int i = 0; i < _allLights.Length; i++)
                    {
                        if (_allLights[i] != null)
                            _allLightDefaultIntensity[i] = _allLights[i].intensity;
                    }
                }
            }
            _initializedLightIntensity = true;
        }

        if (_allLights == null) return;

        float multiplier = Mathf.Clamp(newValue, 0f, 2f);

        for (int i = 0; i < _allLights.Length; i++)
        {
            if (_allLights[i] != null)
                _allLights[i].intensity = _allLightDefaultIntensity[i] * multiplier;
        }
    }

    private void UpdateWarheadLight(Color oldColor, Color newColor)
    {
        PrepareCache();
        foreach (var light in LightsInRoom)
        {
            if (light != null)
            {
                light.WarheadColor = newColor;
                light.isColorChanged = true;
            }
        }
    }

    private void UpdateWarheadLightOverride(bool oldState, bool newState)
    {
        PrepareCache();
        foreach (var light in LightsInRoom)
        {
            if (light != null)
                light.WarheadLightOverride = newState;
        }
    }
}
