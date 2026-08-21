using System.Diagnostics;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using UnityEngine;

namespace CameraShaking
{
    public class RecoilShake : IShakeEffect
    {
        private readonly Stopwatch _removeStopwatch;
        private readonly RecoilSettings _settings;
        private readonly Quaternion _startQuaternion;
        private readonly float _scale;
        private bool _firstFrame = true;

        public RecoilShake(RecoilSettings settings, Firearm firearm = null, float scale = 1f)
        {
            _settings = settings;
            _scale = scale;

            if (firearm != null)
            {
                _scale *= AttachmentsUtils.AttachmentsValue(firearm, AttachmentParam.OverallRecoilMultiplier);

                if (firearm.AdsModule != null && firearm.AdsModule.ClientAdsAmount >= 1f)
                    _scale *= AttachmentsUtils.AttachmentsValue(firearm, AttachmentParam.AdsRecoilMultiplier);
            }

            _startQuaternion = Quaternion.Euler(0f, 0f, settings.ZAxis * (Random.value - 0.5f) * _scale);
            _removeStopwatch = Stopwatch.StartNew();
        }

        public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
        {
            float progress = Mathf.Clamp01((float)_removeStopwatch.Elapsed.TotalSeconds / _settings.AnimationTime);

            float vLook;
            float hLook;

            if (_firstFrame)
            {
                vLook = _scale * _settings.UpKick;
                hLook = _scale * _settings.SideKick;
                _firstFrame = false;
            }
            else
            {
                vLook = -_settings.UpKick * _scale * Time.deltaTime;
                hLook = -_settings.SideKick * _scale * Time.deltaTime;
            }

            Quaternion currentRot = Quaternion.Slerp(_startQuaternion, Quaternion.identity, progress);
            float currentFov = Mathf.SmoothStep((_settings.FovKick - 1f) * _scale + 1f, 1f, progress);

            shakeValues = new ShakeEffectValues(currentRot, null, null, currentFov, vLook, hLook);

            return progress < 1f;
        }
    }
}
