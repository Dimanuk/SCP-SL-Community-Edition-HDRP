using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using UnityEngine;

namespace InventorySystem.Items.Firearms
{
    public class ClosedBoltFirearmViewmodel : AnimatedFirearmViewmodel
    {
        private enum ClosedBoltLayer
        {
            Grips = 0,
            MainAnims = 1,
            Ads = 2,
            BoltCatch = 3,
            NoMag = 4,
            Trigger = 5,
            FullAuto = 6
        }

        [SerializeField] private float _fullAutoKickSpeed;
        [SerializeField] private bool _isOpenBolt;
        [SerializeField] private bool _boltLockNeedsMag;
        [SerializeField] private float _fullAutoReturnSmoothness = 8f;
        [SerializeField] private float _fullAutoMultiplier = 1f;

        private AutomaticFirearm _firearm;

        private float _fullAutoState;
        private float _triggerSmooth;

        internal override void OnEquipped()
        {
            base.OnEquipped();
            _firearm = ParentItem as AutomaticFirearm;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (_firearm == null)
                return;

            if (_boltLockNeedsMag)
            {
                float boltCatchWeight = (_firearm.Status.Ammo == 0) ? 1f : 0f;
                SetLayerWeight(ClosedBoltLayer.BoltCatch, boltCatchWeight);
            }

            // The NoMag override layer hides the in-gun magazine mesh while no magazine is inserted.
            // But during the reload animation the hand is carrying the fresh magazine, and forcing
            // this layer to full weight would cull that magazine too — leaving it invisible until the
            // MagazineInserted flag flips. Suppress the override while a reload is playing.
            bool noMag = !_firearm.Status.Flags.HasFlagFast(FirearmStatusFlags.MagazineInserted);
            bool reloading = GetAnimatorStateInfo((int)ClosedBoltLayer.MainAnims).tagHash == FirearmAnimatorHashes.Reload;
            SetLayerWeight(ClosedBoltLayer.NoMag, (noMag && !reloading) ? 1f : 0f);

            _triggerSmooth = Mathf.MoveTowards(_triggerSmooth, 0f, Time.deltaTime * 10f);
            SetLayerWeight(ClosedBoltLayer.Trigger, _triggerSmooth);

            if (_fullAutoKickSpeed * _fullAutoReturnSmoothness > 0f)
            {
                float cooldown = 1f / _firearm.FireRate
                    / AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.FireRateMultiplier);
                float estimated = _firearm.RecoilPattern?.GetEstimatedState(cooldown) ?? 1f;
                float target = Mathf.Clamp01(estimated - 1f) * _fullAutoMultiplier;
                float lerpSpeed = (target > _fullAutoState) ? _fullAutoKickSpeed : _fullAutoReturnSmoothness;

                _fullAutoState = Mathf.Lerp(_fullAutoState, target, Time.deltaTime * lerpSpeed);

                SetLayerWeight(ClosedBoltLayer.FullAuto, _fullAutoState);
                AnimatorSetFloat(FirearmAnimatorHashes.RecoilPatternState, _fullAutoState);
            }
        }

        private void SetLayerWeight(ClosedBoltLayer layer, float val)
        {
            AnimatorSetLayerWeight((int)layer, val);
        }

        public ClosedBoltFirearmViewmodel()
        {
            _fullAutoReturnSmoothness = 8f;
            _fullAutoMultiplier = 1f;
        }
    }
}