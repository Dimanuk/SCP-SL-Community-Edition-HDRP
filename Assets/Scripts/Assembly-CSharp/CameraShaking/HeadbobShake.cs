using PlayerRoles.FirstPersonControl.Thirdperson;
using System;
using UnityEngine;

namespace CameraShaking
{
    public class HeadbobShake : IShakeEffect
    {
        [NonSerialized]
        public readonly AnimatedCharacterModel _model;

        public static HeadbobShake _mostCurrent;

        public static readonly string HeadBobSetting = "HeadBob";

        public static bool IsEnabled
        {
            get
            {
                return PlayerPrefsSl.Get(HeadBobSetting, true);
            }
        }

        public HeadbobShake(AnimatedCharacterModel model)
        {
            _model = model;
            _mostCurrent = this;
        }

        public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
        {
            if (_model == null || _model.Pooled || !_model.IsTracked || this != _mostCurrent)
            {
                shakeValues = ShakeEffectValues.None;
                return false;
            }

            Vector3 headBobOffset = IsEnabled
                ? ply.transform.TransformDirection(_model.HeadBobPosition)
                : Vector3.zero;

            shakeValues = new ShakeEffectValues(null, null, headBobOffset);
            return true;
        }
    }
}