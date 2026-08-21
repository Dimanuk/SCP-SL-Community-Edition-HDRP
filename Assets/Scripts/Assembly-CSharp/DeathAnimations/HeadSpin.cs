using PlayerStatsSystem;
using UnityEngine;

namespace DeathAnimations
{
    public class HeadSpin : DeathAnimation
    {
        private const float NeckSpinAngle = 140f;

        private const float NeckSnapSpeed = 30f;

        private float _snapAngle;

        private int _randomSnapDirection;

        private CharacterJoint _joint;

        protected override void OnAnimationStarted()
        {
            if (TargetRagdoll.Info.Handler is Scp049DamageHandler)
            {
                enabled = true;
                _snapAngle = 0f;
                _randomSnapDirection = Random.Range(0, 2) * 2 - 1; // -1 or 1
                _joint = GetComponent<CharacterJoint>();
                _joint.highTwistLimit = new SoftJointLimit { limit = 180f };
                _joint.lowTwistLimit = new SoftJointLimit { limit = -180f };
                _joint.swing1Limit = new SoftJointLimit { limit = 180f };
                _joint.swing2Limit = new SoftJointLimit { limit = 180f };
            }
        }

        private void Update()
        {
            _snapAngle += NeckSnapSpeed * Time.deltaTime * _randomSnapDirection;
            if (Mathf.Abs(_snapAngle) >= NeckSpinAngle)
            {
                _snapAngle = NeckSpinAngle * _randomSnapDirection;
                enabled = false;
            }
            _joint.axis = new Vector3(0f, _snapAngle, 0f);
        }
    }
}
