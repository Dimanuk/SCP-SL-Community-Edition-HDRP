using System.Diagnostics;
using UnityEngine;

namespace InventorySystem.Items.Firearms.FunctionalParts
{
    public class GlowingBarrel : FunctionalFirearmPart
    {
        [SerializeField]
        private float _currentTemperature;

        [SerializeField]
        private float _temperaturePerShot;

        [SerializeField]
        private float _temperatureDropPerMinute;

        [SerializeField]
        private float _colorLerpSpeed;

        [SerializeField]
        private Gradient _colorOverTemperature;

        [SerializeField]
        private Material[] _targetMaterials;

        [SerializeField]
        private string _emissionColorName;

        private int _emissionColorHash;
        private bool _emissionHashSet;

        private readonly Stopwatch _stopwatch = new();

        private int EmissionColorHash
        {
            get
            {
                if (!_emissionHashSet)
                {
                    _emissionColorHash = Shader.PropertyToID(_emissionColorName);
                    _emissionHashSet = true;
                    return _emissionColorHash;
                }

                _emissionHashSet = true;
                return _emissionColorHash;
            }
        }

        private void Start()
        {
            Firearm.OnShotCalled += OnShot;
        }

        private void OnDestroy()
        {
            Firearm fa = Firearm;
            if (fa != null)
                fa.OnShotCalled -= OnShot;
        }

        private void OnEnable()
        {
            Refresh(forceChange: true);
        }

        private void Update()
        {
            Refresh(forceChange: false);
        }

        private void OnShot()
        {
            _currentTemperature += _temperaturePerShot;
        }

        private void Refresh(bool forceChange)
        {
            float elapsedMinutes = (float)_stopwatch.Elapsed.TotalSeconds / 60f;
            _currentTemperature -= _temperatureDropPerMinute * elapsedMinutes;

            float clamped = Mathf.Clamp01(_currentTemperature);
            _currentTemperature = clamped;

            Color targetColor = _colorOverTemperature.Evaluate(clamped);

            Color finalColor;
            if (!forceChange)
            {
                Color currentColor = _targetMaterials[0].GetColor(EmissionColorHash);
                finalColor = Color.Lerp(currentColor, targetColor, _colorLerpSpeed * Time.deltaTime);
            }
            else
            {
                finalColor = targetColor;
            }

            int hash = EmissionColorHash;
            for (int i = 0; i < _targetMaterials.Length; i++)
                _targetMaterials[i].SetColor(hash, finalColor);

            _stopwatch.Restart();
        }
    }
}