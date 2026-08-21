using UnityEngine;

namespace PostProcessing
{
    public class FogSetting : MonoBehaviour
    {
        public FogType FogType;
        public int Priority;
        public float EndDistance;
        public Color Color;

        private bool _isDirty;
        private bool _state;

        public bool IsEnabled
        {
            get => _state;
            internal set
            {
                if (_state == value) return;
                _state = value;
                _isDirty = true;
            }
        }

        public float BlendTime { get; internal set; }
        public float Weight { get; private set; }

        public void UpdateWeight()
        {
            if (!_isDirty) return;

            float blendTime = BlendTime;
            if (blendTime <= 0f)
            {
                Weight = _state ? 1f : 0f;
                _isDirty = false;
                return;
            }

            float delta = Time.deltaTime / blendTime;
            if (!_state) delta = -delta;

            float newWeight = Weight + delta;
            Weight = newWeight;

            if (newWeight < 0f || newWeight > 1f)
            {
                Weight = Mathf.Clamp01(newWeight);
                _isDirty = false;
            }
        }
    }
}