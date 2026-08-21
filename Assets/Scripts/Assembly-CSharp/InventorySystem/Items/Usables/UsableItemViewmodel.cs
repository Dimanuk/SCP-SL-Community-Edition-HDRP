using UnityEngine;

namespace InventorySystem.Items.Usables
{
    public class UsableItemViewmodel : StandardAnimatedViemodel
    {
        private static readonly int UseAnimHash = Animator.StringToHash("IsUsing");
        public static readonly int SpeedModifierHash = Animator.StringToHash("SpeedModifier");

        public bool Equipped;

        [SerializeField]
        private AudioSource _equipSoundSource;

        public AudioSource EquipSoundSource => _equipSoundSource;

        protected float _originalPitch = 1f;

        public override void InitAny()
        {
            base.InitAny();

            if (_equipSoundSource != null)
            {
                _originalPitch = _equipSoundSource.pitch;
            }
        }

        public override void InitSpectator(ReferenceHub ply, ItemIdentifier id, bool wasEquipped)
        {
            base.InitSpectator(ply, id, wasEquipped);
            OnEquipped();

            UsableItemsController.OnClientStatusReceived += HandleMessage;

            if (!wasEquipped)
                return;

            _equipSoundSource?.Stop();

            if (UsableItemsController.StartTimes.TryGetValue(id.SerialNumber, out float startTime))
            {
                // Target is mid-use: enter the use animation and fast-forward it by the
                // time already elapsed so the spectator view catches up.
                AnimatorSetBool(UseAnimHash, true);
                AnimatorForceUpdate(Time.timeSinceLevelLoad - startTime, false);
            }
            else
            {
                // Item merely equipped: skip only the equip animation.
                AnimatorForceUpdate(SkipEquipTime, true);
            }
        }

        public virtual void OnUsingCancelled()
        {
            AnimatorSetBool(UseAnimHash, false);
             Equipped =false;
        }

        public virtual void OnUsingStarted()
        {
            _equipSoundSource?.Stop();
            AnimatorSetBool(UseAnimHash, true);
            Equipped =true;
        }

        internal override void OnEquipped()
        {
            base.OnEquipped();

            float speedMultiplier = CustomPlayerEffects.UsableItemModifierEffectExtensions.GetSpeedMultiplier(
                ItemId.TypeId, Hub);

            AnimatorSetFloat(SpeedModifierHash, speedMultiplier);

            if (_equipSoundSource != null)
            {
                _equipSoundSource.pitch = speedMultiplier * _originalPitch;
            }
        }

        private void HandleMessage(StatusMessage msg)
        {
            if (ItemId.SerialNumber != msg.ItemSerial)
                return;

            bool isUsing = msg.Status == StatusMessage.StatusType.Start;
            AnimatorSetBool(UseAnimHash, isUsing);
        }

        private void OnDestroy()
        {
            UsableItemsController.OnClientStatusReceived -= HandleMessage;
        }
    }
}