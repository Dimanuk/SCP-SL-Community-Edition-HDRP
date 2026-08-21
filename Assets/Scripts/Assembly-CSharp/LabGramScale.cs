using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class LabGramScale : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private int _weightOffset;

    [SerializeField] private AudioSource _beep;

    private float _detectedWeight;
    private float _prevDetected;
    private float _weightRandom;
    private bool _idle;

    private const float AdjustmentLerp = 1.4f;
    private const int WeightLimitGrams = 4500;
    private const int SaladLimit = 14000;
    private const float DisableTime = 5f;
    private const float ConversionRate = 945f;

    private readonly Stopwatch _lastWeightSw = new Stopwatch();
    private readonly HashSet<Rigidbody> _detectedRbs = new HashSet<Rigidbody>();

    private bool Calibrated => Mathf.Abs(_weightRandom) < 0.001f;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null)
            return;

        if (_detectedRbs.Add(rb))
        {
            _idle = false;
            _detectedWeight += rb.mass;

            if (Mathf.Approximately(_detectedWeight, 0f))
                _detectedWeight = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (_idle)
        {
            if (_text != null)
                _text.text = string.Empty;

            return;
        }

        if (Mathf.Approximately(_detectedWeight, _prevDetected))
        {
            _weightRandom = Random.Range(-0.005f, 0.001f);
        }
        else
        {
            float sign = Mathf.Sign(_detectedWeight - _prevDetected);
            _weightRandom = Random.Range(0.01f, 0.04f) * sign;
        }

        _prevDetected = _detectedWeight;

        float displayedWeight = (_detectedWeight * ConversionRate) + _weightOffset + _weightRandom;
        int grams = Mathf.RoundToInt(displayedWeight);

        if (Calibrated)
        {
            if (_beep != null && _lastWeightSw.ElapsedMilliseconds > 100)
                _beep.Play();

            _lastWeightSw.Restart();
        }

        if (_text != null)
        {
            if (grams > SaladLimit)
                _text.text = "Next time eat a salad.";
            else if (grams > WeightLimitGrams)
                _text.text = grams + " g";
            else
                _text.text = grams + " g";
        }
        
        if (_lastWeightSw.Elapsed.TotalSeconds > DisableTime)
        {
            _idle = true;
            _detectedWeight = 0f;
            _detectedRbs.Clear();
        }
    }

    public LabGramScale()
    {
        _lastWeightSw.Start();
    }
}