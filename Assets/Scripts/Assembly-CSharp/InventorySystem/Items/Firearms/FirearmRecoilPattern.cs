using System;
using UnityEngine;
using CameraShaking;

namespace InventorySystem.Items.Firearms
{
    [Serializable]
    public class FirearmRecoilPattern
    {
        private float _currentBulletsShot;

        private float _lastReading;

        private float _totalCutoff;

        public float SingleShotTolerance = 1.1f;

        public AnimationCurve DropOverTime;

        public AnimationCurve ZAxisScale;

        public AnimationCurve FovKickScale;

        public AnimationCurve HorizontalKickScale;

        public AnimationCurve VerticalKickScale;

        public AnimationCurve InaccuracyOverShots;

        public float GetEstimatedState(float timeBetweenShots)
        {
            if (_totalCutoff == 0f)
            {
                Keyframe[] keys = DropOverTime.keys;
                _totalCutoff = keys[keys.Length - 1].time;
            }

            float timePassed = Time.timeSinceLevelLoad - _lastReading;
            float toleranceThreshold = timeBetweenShots * SingleShotTolerance;
            float bullets = _currentBulletsShot;

            if (timePassed > toleranceThreshold)
            {
                float idleTime = timePassed - toleranceThreshold;
                bullets = (idleTime > _totalCutoff) ? 0f : Mathf.Max(0f, bullets - DropOverTime.Evaluate(idleTime));
            }

            return Mathf.Max(1f, bullets + 1f);
        }

        public void ApplyShot(float timeBetweenShots)
        {
            float timeSinceLevelLoad = Time.timeSinceLevelLoad;
            if (_lastReading != timeSinceLevelLoad)
            {
                _currentBulletsShot = GetEstimatedState(timeBetweenShots);
                _lastReading = timeSinceLevelLoad;
            }
        }

        public RecoilSettings GetRecoil(RecoilSettings startRecoil)
        {
            float bullets = _currentBulletsShot;

            return new RecoilSettings(
                startRecoil.AnimationTime,
                startRecoil.ZAxis * ZAxisScale.Evaluate(bullets),
                (startRecoil.FovKick - 1f) * FovKickScale.Evaluate(bullets) + 1f,
                startRecoil.UpKick * VerticalKickScale.Evaluate(bullets),
                startRecoil.SideKick * HorizontalKickScale.Evaluate(bullets));
        }

        public float GetInaccuracy()
        {
            return InaccuracyOverShots.Evaluate(_currentBulletsShot);
        }
    }
}
