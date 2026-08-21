using UnityEngine;

namespace CameraShaking
{
    public class OldRecoilShake : IShakeEffect
    {
        private float _recoil;
        private readonly float _backSpeed;
        private Vector3 _targetRotVec;
        private float _jumpH;
        private float _jumpV;

        public OldRecoilShake(float shockSize, float upSize, float backSpeed, float multiplier)
        {
            _backSpeed = backSpeed / 15f;
            _recoil = shockSize;

            _targetRotVec.x = Random.Range(-70, -50);
            _targetRotVec.y = Random.Range(-20, 20);
            _targetRotVec.z = Random.Range(-20, 20);

            if (backSpeed <= 10)
            {
                _jumpV = 0f;
                _jumpH = 0f;
            }
            else
            {
                _jumpV = upSize * 13f * multiplier;
                _jumpH = (float)Random.Range(-upSize, upSize) * 10f * multiplier;
            }
        }

        public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
        {
            float deltaTime = Time.deltaTime;

            if (_recoil > 0f)
            {
                _recoil -= deltaTime * _backSpeed;
            }

            if (_recoil < 0f)
            {
                _recoil = 0f;
            }

            Quaternion targetRotation = Quaternion.Euler(_targetRotVec * _recoil);

            shakeValues = new ShakeEffectValues(
                targetRotation,
                Quaternion.identity,
                Vector3.zero,
                1f,
                _jumpV * _recoil,
                _jumpH * _recoil
            );

            return _recoil > 0f;
        }
    }
}