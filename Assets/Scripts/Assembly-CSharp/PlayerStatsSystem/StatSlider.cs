
using UnityEngine;
using UnityEngine.UI;

namespace PlayerStatsSystem
{
	public class StatSlider : MonoBehaviour
	{
		private enum DisplayExactMode
		{
			PreferenceBased = 0,
			AlwaysExact = 1,
			AlwaysPercent = 2,
			ValueHidden = 3
		}

		[SerializeField]
		public int _displayedStatId;

		[SerializeField]
		public float _lerpSpeed;

		[SerializeField]
		public float _snapValueSkip;

		[SerializeField]
		private DisplayExactMode _displayExactMode;

		[SerializeField]
		public string _preferenceKey;

		[SerializeField]
		public string _suffix;

		[SerializeField]
		public Text _targetText;

		[SerializeField]
		public Slider _targetSlider;

		[SerializeField]
		public int _roundingAccuracy;

		private float _currentValue;

		private string _originalSuffix;

		private const float AbsoluteMoveRatio = 0.04f;

        public string CustomSuffix
        {
            get
            {
                return _suffix;
            }
            set
            {
                _suffix = (string.IsNullOrEmpty(value) ? _originalSuffix : value);
            }
        }

        public int StatId => _displayedStatId;

        public void ForceUpdate()
        {
            if (TryGetModule(out var sb))
            {
                _currentValue = sb.CurValue;
            }
            _targetSlider.value = _currentValue;
        }

        private bool TryGetModule(out global::PlayerStatsSystem.StatBase sb)
        {
            sb = null;
            if (!ReferenceHub.TryGetLocalHub(out var hub))
            {
                return false;
            }
            if (hub.roleManager.CurrentRole is not global::PlayerRoles.IHealthbarRole healthbarRole)
            {
                return false;
            }
            if (healthbarRole.TargetStats == null)
            {
                return false;
            }
            sb = healthbarRole.TargetStats.StatModules[_displayedStatId];
            return true;
        }

        private void Awake()
        {
            _originalSuffix = _suffix;
            if (_displayExactMode == global::PlayerStatsSystem.StatSlider.DisplayExactMode.PreferenceBased)
            {
                _displayExactMode = (PlayerPrefsSl.Get(_preferenceKey, defaultValue: true) ? global::PlayerStatsSystem.StatSlider.DisplayExactMode.AlwaysExact : global::PlayerStatsSystem.StatSlider.DisplayExactMode.AlwaysPercent);
            }
        }

        private void Update()
        {
            if (TryGetModule(out var sb))
            {
                UpdateSlider(sb);
                UpdateText(sb);
            }
        }

        private void UpdateSlider(global::PlayerStatsSystem.StatBase stat)
        {
            _targetSlider.minValue = stat.MinValue;
            _targetSlider.maxValue = stat.MaxValue;
            float num = global::UnityEngine.Mathf.Abs(stat.CurValue - _currentValue);
            if (num > _snapValueSkip)
            {
                _currentValue = stat.CurValue;
            }
            else
            {
                float num2 = global::UnityEngine.Mathf.Max(_lerpSpeed * num, (stat.MaxValue - stat.MinValue) * 0.04f);
                _currentValue = global::UnityEngine.Mathf.MoveTowards(_currentValue, stat.CurValue, num2 * global::UnityEngine.Time.deltaTime);
            }
            _targetSlider.value = _currentValue;
        }

        private void UpdateText(global::PlayerStatsSystem.StatBase stat)
        {
            if (_displayExactMode != global::PlayerStatsSystem.StatSlider.DisplayExactMode.ValueHidden)
            {
                bool flag = _displayExactMode == global::PlayerStatsSystem.StatSlider.DisplayExactMode.AlwaysExact;
                float num = (flag ? stat.CurValue : ((float)global::UnityEngine.Mathf.CeilToInt(stat.NormalizedValue * 100f)));
                _targetText.text = global::UnityEngine.Mathf.CeilToInt(num * (float)_roundingAccuracy) / _roundingAccuracy + (flag ? _suffix : "%");
            }
        }
    }
}
