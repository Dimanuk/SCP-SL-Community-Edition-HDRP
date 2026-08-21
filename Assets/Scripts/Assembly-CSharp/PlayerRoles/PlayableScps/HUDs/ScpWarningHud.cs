using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace PlayerRoles.PlayableScps.HUDs
{
    public class ScpWarningHud : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        private const float FadeSpeed = 8f;
        private const float DefaultTime = 3.8f;

        private float _duration;
        private string _targetText;
        private float _alpha;
        private bool _dirty;
        private readonly Stopwatch _elapsed = new Stopwatch();

        private float Alpha
        {
            get => _alpha;
            set
            {
                value = Mathf.Clamp01(value);
                if (_alpha != value)
                {
                    _alpha = value;
                    _text.alpha = value;
                }
            }
        }

        private void Awake()
        {
            _text.alpha = Alpha;
        }

        private void Update()
        {
            if (_elapsed.Elapsed.TotalSeconds > _duration || _dirty)
            {
                Alpha -= Time.deltaTime * FadeSpeed;

                if (Alpha <= 0f && _dirty)
                {
                    _text.text = _targetText;
                    _dirty = false;
                }
            }
            else
            {
                Alpha += Time.deltaTime * FadeSpeed;
            }
        }

        public void SetText(string text, float duration = DefaultTime)
        {
            _dirty |= _targetText != text;
            _targetText = text;
            _duration = duration;
            _elapsed.Restart();
        }
    }
}