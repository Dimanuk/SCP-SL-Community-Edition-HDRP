[global::UnityEngine.RequireComponent(typeof(global::UnityEngine.AudioSource))]
[global::UnityEngine.RequireComponent(typeof(global::UnityEngine.AudioReverbFilter))]
[global::UnityEngine.RequireComponent(typeof(global::UnityEngine.AudioLowPassFilter))]
public class AudioProximityEffects : global::UnityEngine.MonoBehaviour
{
    [global::UnityEngine.SerializeField]
    private global::UnityEngine.AnimationCurve _reverbSizeOverDistance;

    [global::UnityEngine.SerializeField]
    private global::UnityEngine.AnimationCurve _reverbDryOverDistance;

    [global::UnityEngine.SerializeField]
    private global::UnityEngine.AnimationCurve _lowpassOverDistance;

    private global::UnityEngine.AudioSource _audioSource;

    private global::UnityEngine.AudioReverbFilter _reverbFilter;

    private global::UnityEngine.AudioLowPassFilter _lowPassFilter;

    private float ProximityLevel
    {
        get
        {
            if (!ReferenceHub.TryGetLocalHub(out var hub))
            {
                return 0f;
            }
            if (!(hub.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole))
            {
                return 0f;
            }
            return global::UnityEngine.Vector3.Distance(base.transform.position, fpcRole.FpcModule.Position) / _audioSource.maxDistance * _audioSource.spatialBlend;
        }
    }

    private void Awake()
    {
        _audioSource = GetComponent<global::UnityEngine.AudioSource>();
        _reverbFilter = GetComponent<global::UnityEngine.AudioReverbFilter>();
        _lowPassFilter = GetComponent<global::UnityEngine.AudioLowPassFilter>();
    }

    private void Update()
    {
        float proximityLevel = ProximityLevel;
        _reverbFilter.dryLevel = _reverbDryOverDistance.Evaluate(proximityLevel);
        _reverbFilter.room = _reverbSizeOverDistance.Evaluate(proximityLevel);
        _lowPassFilter.cutoffFrequency = _lowpassOverDistance.Evaluate(proximityLevel);
    }
}
