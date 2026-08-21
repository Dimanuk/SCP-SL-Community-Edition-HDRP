
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Cameras
{
	public abstract class CameraAxisBase
	{
		private float _val;

		private bool _wasEverSet;

		private bool _wasMoving;

		private const float LocalPlayerLerpMultiplier = 1f;

		private const float LocalPlayerPitchMultiplier = 1f;

		private const float LocalPlayerVolumeMultiplier = 0.4f;

		[SerializeField]
		private float _soundLerpSpeed;

		[SerializeField]
		private float _soundStopSpeed;

		[SerializeField]
		private float _localPlayerDiffLimiter;

		[SerializeField]
		private Vector2 _constraints;

		[SerializeField]
		protected AudioSource SoundEffectSource;

		[SerializeField]
		protected AnimationCurve SpeedCurve;

		[SerializeField]
		protected AnimationCurve VolumeCurve;

		[SerializeField]
		protected AnimationCurve PitchCurve;

		private bool IsFirstperson => Scp079Role.LocalInstanceActive;

        private bool IsSpectating
        {
            get
            {
                if (!global::PlayerRoles.Spectating.SpectatorTargetTracker.TrackerSet)
                {
                    return false;
                }
                if (global::PlayerRoles.Spectating.SpectatorTargetTracker.CurrentTarget is global::PlayerRoles.PlayableScps.Scp079.Scp079SpectatableModule scp079SpectatableModule)
                {
                    return scp079SpectatableModule != null;
                }
                return false;
            }
        }

        protected virtual float SpectatorLerpMultiplier => 0f;

		public float CurValue { get; internal set; }

        public float MinValue => _constraints.x;
        public float MaxValue => _constraints.y;

        public float TargetValue
        {
            get
            {
                return _val;
            }
            set
            {
                _val = global::UnityEngine.Mathf.Clamp(value % 360f, _constraints.x, _constraints.y);
                if (!_wasEverSet)
                {
                    CurValue = _val;
                    _wasEverSet = true;
                    _wasMoving = true;
                    OnValueChanged(_val, null);
                }
            }
        }


        public ushort Value16BitCompression
        {
            get
            {
                return (ushort)Compress(_constraints, TargetValue, 65535);
            }
            set
            {
                TargetValue = Uncompress(_constraints, (int)value, 65535);
            }
        }

        public byte Value8BitCompression
        {
            get
            {
                return (byte)Compress(_constraints, TargetValue, 255);
            }
            set
            {
                TargetValue = Uncompress(_constraints, (int)value, 255);
            }
        }

        internal virtual void Update(global::PlayerRoles.PlayableScps.Scp079.Cameras.Scp079Camera cam)
        {
            if (CurValue == TargetValue)
            {
                if (_wasMoving)
                {
                    float num = SoundEffectSource.volume - _soundStopSpeed * global::UnityEngine.Time.deltaTime;
                    if (num <= 0f)
                    {
                        SoundEffectSource.Stop();
                        _wasMoving = false;
                    }
                    SoundEffectSource.volume = global::UnityEngine.Mathf.Clamp01(num);
                }
                return;
            }
            float time = global::UnityEngine.Mathf.Abs(CurValue - TargetValue);
            bool isFirstperson = IsFirstperson;
            bool isSpectating = IsSpectating;
            float num2 = SpeedCurve.Evaluate(time);
            float num3 = VolumeCurve.Evaluate(time);
            float num4 = PitchCurve.Evaluate(time);
            float num5 = ((_soundLerpSpeed == 0f) ? 1f : (_soundLerpSpeed * global::UnityEngine.Time.deltaTime));
            if (isFirstperson || isSpectating)
            {
                num3 *= 0.4f;
                num4 *= 1f;
                num5 *= 1f;
            }
            if (!_wasMoving)
            {
                SoundEffectSource.Play();
                _wasMoving = true;
            }
            if (isFirstperson)
            {
                CurValue = global::UnityEngine.Mathf.Clamp(CurValue, TargetValue - _localPlayerDiffLimiter, TargetValue + _localPlayerDiffLimiter);
            }
            else if (isSpectating)
            {
                CurValue = global::UnityEngine.Mathf.Lerp(CurValue, TargetValue, global::UnityEngine.Time.deltaTime * SpectatorLerpMultiplier);
            }
            num2 *= global::UnityEngine.Time.deltaTime;
            CurValue = ((_constraints.x == -360f || _constraints.y == 360f) ? global::UnityEngine.Mathf.MoveTowardsAngle(CurValue, TargetValue, num2) : global::UnityEngine.Mathf.MoveTowards(CurValue, TargetValue, num2));
            SoundEffectSource.volume = global::UnityEngine.Mathf.Lerp(SoundEffectSource.volume, num3, num5);
            SoundEffectSource.pitch = global::UnityEngine.Mathf.Lerp(SoundEffectSource.pitch, num4, num5);
            OnValueChanged(CurValue, cam);
        }

        internal virtual void Awake(global::PlayerRoles.PlayableScps.Scp079.Cameras.Scp079Camera cam)
        {
            _wasEverSet = false;
        }

        protected abstract void OnValueChanged(float newValue, Scp079Camera cam);

        private int Compress(global::UnityEngine.Vector2 constraints, float val, int maxVal)
        {
            return global::UnityEngine.Mathf.RoundToInt(global::UnityEngine.Mathf.InverseLerp(constraints.x, constraints.y, val) * (float)maxVal);
        }

        private float Uncompress(global::UnityEngine.Vector2 constraints, float val, int maxVal)
        {
            return global::UnityEngine.Mathf.Lerp(constraints.x, constraints.y, val / (float)maxVal);
        }
    }
}
