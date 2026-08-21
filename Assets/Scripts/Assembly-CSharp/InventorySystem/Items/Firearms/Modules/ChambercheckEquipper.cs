using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using System.Diagnostics;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class ChambercheckEquipper : IEquipperModule, IFirearmModuleBase
    {
        private int _equips;

        private bool _allowInteraction;

        private float _targetTime;

        private readonly int _pickupParamHash;

        private readonly float _normalTime;

        private readonly float _pickupTime;

        private readonly Firearm _firearm;

        private readonly Stopwatch _stopwatch;

        private readonly int _randomParamHash;

        public bool Standby
        {
            get
            {
                if (!_allowInteraction && _stopwatch.Elapsed.TotalSeconds >= (double)_targetTime)
                {
                    _stopwatch.Stop();
                    _allowInteraction = true;
                }
                return _allowInteraction;
            }
        }

        public ChambercheckEquipper(Firearm firearm, string pickupParamName, float normalAnimationTime, float pickupAnimationTime)
        {
            _firearm = firearm;
            _pickupParamHash = Animator.StringToHash(pickupParamName);
            _randomParamHash = FirearmAnimatorHashes.Random;
            _normalTime = normalAnimationTime;
            _pickupTime = pickupAnimationTime;
            _stopwatch = new Stopwatch();
        }

        public void OnEquipped()
        {
            _equips++;
            _allowInteraction = false;

            if (_equips == 1)
            {
                _targetTime = _pickupTime;
            }
            else
            {
                float drawTimeModifier = AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.DrawTimeModifier);
                float drawSpeedMultiplier = AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.DrawSpeedMultiplier);
                _targetTime = (_normalTime + drawTimeModifier) / drawSpeedMultiplier;
            }

            if (!_firearm.IsLocalPlayer)
            {
                if (_firearm.Status.Flags.HasFlagFast(FirearmStatusFlags.Cocked))
                {
                    _targetTime = Mathf.Max(0f, _targetTime - 0.1f);
                }
            }

            _stopwatch.Restart();

            if (_firearm.IsLocalPlayer)
            {
                var viewmodel = _firearm.ClientViewmodel;
                if (viewmodel != null)
                {
                    viewmodel.AnimatorSetFloat(_randomParamHash, Random.value);
                    viewmodel.AnimatorSetBool(_pickupParamHash, _equips == 1);
                }
            }
        }
    }
}