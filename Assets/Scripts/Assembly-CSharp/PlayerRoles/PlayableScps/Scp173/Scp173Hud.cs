using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.PlayableScps.Subroutines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp173
{
    public class Scp173Hud : ScpHudBase
    {
        [SerializeField]
        private Animator _hudAnimator;

        [SerializeField]
        private Image _eyeIndicator;

        [SerializeField]
        private Image _tantrumCooldown;

        [SerializeField]
        private Image _breakneckSpeedsCooldown;

        [SerializeField]
        private TextMeshProUGUI _timer;

        [SerializeField]
        private Sprite _bloodshotEye;

        [SerializeField]
        private Sprite _openEye;

        private Scp173ObserversTracker _observersTracker;
        private Scp173BlinkTimer _blinkAbility;
        private Scp173TantrumAbility _tantrumAbility;
        private Scp173BreakneckSpeedsAbility _breakneckSpeedsAbility;

        private const float RotateSpeedFirst = 0f;
        private const float RotateSpeedLast = 0f;

        private static readonly int AnimatorHudShownHash;
        private static readonly int AnimatorHudReadyHash;

        static Scp173Hud()
        {
            AnimatorHudShownHash = Animator.StringToHash("Shown");
            AnimatorHudReadyHash = Animator.StringToHash("Ready");
        }

        internal override void Init(ReferenceHub hub)
        {
            base.Init(hub);

            if (hub.roleManager.CurrentRole is Scp173Role scp173Role)
            {
                scp173Role.SubroutineModule.TryGetSubroutine(out _observersTracker);
                scp173Role.SubroutineModule.TryGetSubroutine(out _blinkAbility);
                scp173Role.SubroutineModule.TryGetSubroutine(out _tantrumAbility);
                scp173Role.SubroutineModule.TryGetSubroutine(out _breakneckSpeedsAbility);
            }
        }

        protected override void Update()
        {
            base.Update();

            bool isObserved = _observersTracker.IsObserved;
            float remainingSustainPercent = _blinkAbility.RemainingSustainPercent;
            float remainingBlinkCooldown = _blinkAbility.RemainingBlinkCooldown;

            bool showHud = isObserved || remainingSustainPercent > 0f;

            _hudAnimator.SetBool(AnimatorHudShownHash, showHud);
            
            _hudAnimator.SetBool(AnimatorHudReadyHash, remainingBlinkCooldown <= 0f);

            _eyeIndicator.fillAmount = remainingSustainPercent;

            _timer.text = showHud ? $"{remainingBlinkCooldown:F1}s" : string.Empty;
            
            _timer.color = Color.Lerp(Color.clear, Color.white, remainingSustainPercent);

            UpdateCooldown(_tantrumCooldown, _tantrumAbility.Cooldown);
            UpdateCooldown(_breakneckSpeedsCooldown, _breakneckSpeedsAbility.Cooldown);
            _eyeIndicator.sprite = (remainingBlinkCooldown <= 0f) ? _bloodshotEye : _openEye;
        }

        private void UpdateCooldown(Image target, AbilityCooldown cooldown)
        {
            GameObject cooldownGo = target.transform.parent.gameObject;

            if (cooldown.IsReady)
            {
                cooldownGo.SetActive(false);
                return;
            }

            float readiness = cooldown.Readiness;

            float rotationSpeed = Mathf.Lerp(RotateSpeedFirst, RotateSpeedLast, readiness);

            target.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.back);
            
            target.fillAmount = readiness;
            cooldownGo.SetActive(true);
        }
    }
}