using System;
using System.Diagnostics;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.HUDs
{
    [Serializable]
    public class AbilityHud
    {
        [SerializeField]
        private GameObject _parent;

        [SerializeField]
        private CanvasGroup _fader;

        [SerializeField]
        private Image _durationCircle;

        [SerializeField]
        private Image _cooldownCircle;

        [SerializeField]
        private bool _inverseDuration;

        [SerializeField]
        private bool _inverseCooldown;

        [SerializeField]
        private bool _showDurationAtCooldown;

        [SerializeField]
        private Vector3 _startScale = Vector3.one;

        private AbilityCooldown _cooldown;

        private AbilityCooldown _duration;

        private RectTransform _rt;

        private bool _hasDuration;

        private bool _hasFader;

        private readonly Stopwatch _fullStopwatch = new Stopwatch();

        private const float MinFullTime = 0.3f;

        private const float FadeSpeed = 8.5f;

        public void Setup(AbilityCooldown cd, AbilityCooldown duration)
        {
            _cooldown = cd;
            _duration = duration;
            _rt = _cooldownCircle.rectTransform;
            _hasDuration = _duration != null && _durationCircle != null;
            _hasFader = _fader != null;
            bool flag = UpdateVisibility();
            if (_hasFader)
            {
                _fader.alpha = flag ? 1f : 0f;
            }
            else
            {
                _parent.SetActive(flag);
            }
        }

        public void Update(bool forceHidden = false)
        {
            bool flag = !forceHidden && UpdateVisibility();
            if (_hasFader)
            {
                _fader.alpha = Mathf.Clamp01(_fader.alpha + Time.deltaTime * (flag ? FadeSpeed : -FadeSpeed));
            }
            else
            {
                _parent.SetActive(flag);
            }
        }

        private bool UpdateVisibility()
        {
            bool result = UpdateCooldown();
            if (_hasDuration && UpdateDuration())
            {
                result = true;
            }
            return result;
        }

        private bool UpdateCooldown()
        {
            float t = FillCircle(_cooldown, _cooldownCircle, _inverseCooldown);
            _rt.localScale = Vector3.Lerp(_startScale, Vector3.one, t);
            return !_cooldown.IsReady;
        }

        private bool UpdateDuration()
        {
            if (_duration.IsReady)
            {
                _durationCircle.enabled = false;
                if (_fullStopwatch.IsRunning)
                {
                    return _fullStopwatch.Elapsed.TotalSeconds < MinFullTime;
                }
                return false;
            }
            _durationCircle.enabled = _showDurationAtCooldown || _cooldown.IsReady;
            FillCircle(_duration, _durationCircle, !_inverseDuration);
            _fullStopwatch.Restart();
            return true;
        }

        private float FillCircle(AbilityCooldown cd, Image circle, bool inverse)
        {
            float num = cd.Readiness;
            if (inverse)
            {
                num = 1f - num;
            }
            circle.fillAmount = num;
            return num;
        }
    }
}
