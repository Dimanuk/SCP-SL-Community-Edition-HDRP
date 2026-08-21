using UnityEngine;
using UnityEngine.UI;

public class ImageBasedRealisticLoadingBar : MonoBehaviour
{
    private RealisticLoadingBar _bar;

    [SerializeField] private float _targetTime;
    [SerializeField] private float _stepVar;
    [SerializeField] private float _tickVar;
    [SerializeField] private int _minNumOfSteps;
    [SerializeField] private int _maxNumOfSteps;
    [SerializeField] private Image _targetImage;
    [SerializeField] private float _smoothing;

    private void OnEnable()
    {
        _targetImage.fillAmount = 0f;

        int steps = Random.Range(_minNumOfSteps, _maxNumOfSteps + 1);

        _bar = new RealisticLoadingBar(_targetTime, steps, _stepVar, _tickVar);
    }

    private void Update()
    {
        if (_targetImage == null || _bar == null)
            return;

        float current = _targetImage.fillAmount;
        float progress = _bar.Progress;
        float delta = Time.deltaTime;
        _targetImage.fillAmount = Mathf.Lerp(current, progress, delta * _smoothing);
    }
}