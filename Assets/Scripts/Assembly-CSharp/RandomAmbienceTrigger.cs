public class RandomAmbienceTrigger : global::UnityEngine.MonoBehaviour
{
    [global::UnityEngine.SerializeField]
    private global::UnityEngine.AudioClip[] _ambientClips;

    [global::UnityEngine.SerializeField]
    private global::UnityEngine.AudioSource _ambientSource;

    private float _timeUntilAmbience;

    [global::UnityEngine.SerializeField]
    private float _minAmbienceTime;

    [global::UnityEngine.SerializeField]
    private float _maxAmbienceTime;

    private void Start()
    {
        Rerandomize();
    }

    private void Update()
    {
        _timeUntilAmbience -= global::UnityEngine.Time.deltaTime;
        if (!(_timeUntilAmbience > 0f))
        {
            _ambientSource.clip = _ambientClips.RandomItem();
            _ambientSource.Play();
            Rerandomize();
        }
    }

    private void Rerandomize()
    {
        _timeUntilAmbience = global::UnityEngine.Random.Range(_minAmbienceTime, _maxAmbienceTime);
    }
}
