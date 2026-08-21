namespace InventorySystem.Items.Usables
{
    public class RegenerationProcess
    {
        private readonly global::UnityEngine.AnimationCurve _regenCurve;

        private readonly float _maxTime;

        private readonly float _speedMultip;

        private readonly float _hpMultip;

        private float _healValue;

        private float _elapsed;

        public RegenerationProcess(global::UnityEngine.AnimationCurve regenCurve, float speedMultiplier, float healthPointsMultiplier)
        {
            _regenCurve = regenCurve;
            _maxTime = regenCurve.keys[regenCurve.length - 1].time;
            _speedMultip = speedMultiplier;
            _hpMultip = healthPointsMultiplier * _speedMultip;
            _healValue = 0f;
            _elapsed = 0f;
        }

        public void GetValue(out bool isDone, out int value)
        {
            _elapsed += global::UnityEngine.Time.deltaTime * _speedMultip;
            _healValue += _regenCurve.Evaluate(_elapsed) * global::UnityEngine.Time.deltaTime * _hpMultip;
            value = (int)_healValue;
            _healValue -= value;
            isDone = _elapsed >= _maxTime;
        }
    }
}
