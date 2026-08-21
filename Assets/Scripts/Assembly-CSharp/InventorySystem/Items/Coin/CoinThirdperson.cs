using InventorySystem.Items.Thirdperson;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace InventorySystem.Items.Coin
{
    public class CoinThirdperson : IdleThirdpersonItem
    {
        [SerializeField] private AnimationClip _headsOverride;
        [SerializeField] private AnimationClip _tailsOverride;

        private const ThirdpersonItemAnimationName TargetLayer = ThirdpersonItemAnimationName.Override0;

        private static readonly int TriggerHash = Animator.StringToHash("OverrideTrigger");

        internal override void Initialize(HumanCharacterModel subcontroller, ItemIdentifier id)
        {
            base.Initialize(subcontroller, id);
            Coin.OnFlipped += OnCoinflip;
        }

        public override void ResetObject()
        {
            base.ResetObject();
            Coin.OnFlipped -= OnCoinflip;
        }

        private void OnCoinflip(ushort serial, bool isTails)
        {
            if (serial != base.Identifier.SerialNumber)
                return;

            AnimationClip clipToPlay = isTails ? _tailsOverride : _headsOverride;

            ThirdpersonItemAnimationManager.SetAnimation(TargetModel, TargetLayer, clipToPlay);

            if (TargetModel != null && TargetModel.Animator != null)
            {
                TargetModel.Animator.Play(TriggerHash);
            }
        }
    }
}