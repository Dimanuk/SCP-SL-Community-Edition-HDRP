using Mirror;
using PlayerRoles.Spectating;
using PostProcessing;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BloodEffectsSystem : MonoBehaviour
{
    public static BloodEffectsSystem LocalPlayerSingleton;

    [Space]
    public float HealthLerpSpeed = 0.1f;
    public float PulseBloodDeductSpeed = 1f;
    public float ScrapeBloodDeductSpeed = 0.1f;

    public AnimationCurve grayscaleAnimationCurve;
    public AnimationCurve vignetteAnimationCurve;

    private BloodHit _bloodHit;
    private ColorGrading _bloodGrayScale;

    private float _dyingRatio;
    private bool _instantUpdate;
    private int _fullScapeRoundRobin;

    public bool DisableHurtEffect { get; set; }

    private void Awake()
    {
        LocalPlayerSingleton = this;

        PostProcessVolume volume = GetComponent<PostProcessVolume>();
        if (volume != null)
        {
            volume.sharedProfile.TryGetSettings(out _bloodGrayScale);
            volume.sharedProfile.TryGetSettings(out _bloodHit);
        }

        _fullScapeRoundRobin = Random.Range(0, 3);

        SpectatorTargetTracker.OnTargetChanged += delegate { _instantUpdate = true; };
    }

    [Client]
    public void AddPulseBlood(Vector3 hitDir, float opacityMultiplier = 1f)
    {
        if (!NetworkClient.active) return;

        if (hitDir.x > 0f)
        {
            _bloodHit.Hit_Right.value = opacityMultiplier;
        }
        else if (hitDir.x < 0f)
        {
            _bloodHit.Hit_Left.value = opacityMultiplier;
        }

        if (hitDir.z > 0f)
        {
            _bloodHit.Hit_Up.value = opacityMultiplier;
        }
        else if (hitDir.z < 0f)
        {
            _bloodHit.Hit_Down.value = opacityMultiplier;
        }
    }

    [Client]
    public void AddScrapeBlood(Vector3 hitDir, float opacityMultiplier = 1f)
    {
        if (!NetworkClient.active) return;

        if (hitDir.x > 0f)
        {
            _bloodHit.Blood_Hit_Right.value = opacityMultiplier;
        }
        else if (hitDir.x < 0f)
        {
            _bloodHit.Blood_Hit_Left.value = opacityMultiplier;
        }

        if (hitDir.z > 0f)
        {
            _bloodHit.Blood_Hit_Up.value = opacityMultiplier;
        }
        else if (hitDir.z < 0f)
        {
            _bloodHit.Blood_Hit_Down.value = opacityMultiplier;
        }
    }

    [Client]
    public void AddScrapes(float opacity)
    {
        if (!NetworkClient.active) return;

        switch (_fullScapeRoundRobin)
        {
            case 0:
                _bloodHit.Blood_Hit_Full_1.value = opacity;
                break;
            case 1:
                _bloodHit.Blood_Hit_Full_2.value = opacity;
                break;
            case 2:
                _bloodHit.Blood_Hit_Full_3.value = opacity;
                break;
        }

        _fullScapeRoundRobin++;
        if (_fullScapeRoundRobin >= 3)
        {
            _fullScapeRoundRobin = 0;
        }
    }

    [Client]
    public void ResetScrapes()
    {
        if (!NetworkClient.active) return;

        _bloodHit.Blood_Hit_Right.value = 0f;
        _bloodHit.Blood_Hit_Left.value = 0f;
        _bloodHit.Blood_Hit_Up.value = 0f;
        _bloodHit.Blood_Hit_Down.value = 0f;
        _bloodHit.Blood_Hit_Full_1.value = 0f;
        _bloodHit.Blood_Hit_Full_2.value = 0f;
        _bloodHit.Blood_Hit_Full_3.value = 0f;
    }

    private void Update()
    {
        // Dying ratio target: 0 = Full Health, 1 = Dead. Falls back to 0 (no
        // healthbar target, e.g. free-cam spectating) so the ratio always
        // drifts back to "clean" instead of freezing at its last value.
        float targetDyingRatio = 0f;

        if (!DisableHurtEffect
            && ReferenceHub.TryGetLocalHub(out ReferenceHub hub)
            && hub.roleManager.CurrentRole is PlayerRoles.IHealthbarRole healthbarRole)
        {
            PlayerStatsSystem.PlayerStats targetStats = healthbarRole.TargetStats;
            if (targetStats != null && targetStats.TryGetModule(out PlayerStatsSystem.HealthStat healthStat))
            {
                targetDyingRatio = 1f - healthStat.NormalizedValue;
            }
        }

        if (_instantUpdate)
        {
            _dyingRatio = targetDyingRatio;
            _instantUpdate = false;
        }
        else
        {
            _dyingRatio = Mathf.Lerp(_dyingRatio, targetDyingRatio, HealthLerpSpeed);
        }

        if (float.IsNaN(_dyingRatio))
            _dyingRatio = 0f;

        if (!NetworkClient.active)
        {
            _bloodHit.Hit_Right.value = 0f;
            _bloodHit.Hit_Left.value = 0f;
            _bloodHit.Hit_Up.value = 0f;
            _bloodHit.Hit_Down.value = 0f;
        }

        float saturation = grayscaleAnimationCurve.Evaluate(_dyingRatio) * -100f;
        _bloodGrayScale.saturation.value = saturation;

        float vignette = vignetteAnimationCurve.Evaluate(_dyingRatio);
        _bloodHit.Hit_Full.value = vignette;

        _bloodHit.Hit_Right.value = Mathf.Lerp(_bloodHit.Hit_Right.value, 0f, PulseBloodDeductSpeed);
        _bloodHit.Hit_Left.value = Mathf.Lerp(_bloodHit.Hit_Left.value, 0f, PulseBloodDeductSpeed);
        _bloodHit.Hit_Up.value = Mathf.Lerp(_bloodHit.Hit_Up.value, 0f, PulseBloodDeductSpeed);
        _bloodHit.Hit_Down.value = Mathf.Lerp(_bloodHit.Hit_Down.value, 0f, PulseBloodDeductSpeed);

        _bloodHit.Blood_Hit_Right.value = Mathf.Lerp(_bloodHit.Blood_Hit_Right.value, 0f, ScrapeBloodDeductSpeed);
        _bloodHit.Blood_Hit_Left.value = Mathf.Lerp(_bloodHit.Blood_Hit_Left.value, 0f, ScrapeBloodDeductSpeed);
        _bloodHit.Blood_Hit_Up.value = Mathf.Lerp(_bloodHit.Blood_Hit_Up.value, 0f, ScrapeBloodDeductSpeed);
        _bloodHit.Blood_Hit_Down.value = Mathf.Lerp(_bloodHit.Blood_Hit_Down.value, 0f, ScrapeBloodDeductSpeed);
        _bloodHit.Blood_Hit_Full_1.value = Mathf.Lerp(_bloodHit.Blood_Hit_Full_1.value, 0f, ScrapeBloodDeductSpeed);
        _bloodHit.Blood_Hit_Full_2.value = Mathf.Lerp(_bloodHit.Blood_Hit_Full_2.value, 0f, ScrapeBloodDeductSpeed);
        _bloodHit.Blood_Hit_Full_3.value = Mathf.Lerp(_bloodHit.Blood_Hit_Full_3.value, 0f, ScrapeBloodDeductSpeed);
    }
}