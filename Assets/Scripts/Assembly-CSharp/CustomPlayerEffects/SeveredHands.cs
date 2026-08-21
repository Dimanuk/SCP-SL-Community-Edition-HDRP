using System.Collections.Generic;
using Footprinting;
using InventorySystem.Items;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerStatsSystem;
using UnityEngine;
using AudioPooling;
using InventorySystem;

namespace CustomPlayerEffects
{
    public class SeveredHands : TickingEffectBase, IInteractionBlocker
    {
        private static readonly int HashSeveredHands = Animator.StringToHash("SeveredHands");
        public static readonly List<Footprint> AllSeveredHands = new();

        [SerializeField]
        private AudioClip _severClip;

        [SerializeField]
        private float _severSoundDistance;

        [SerializeField]
        private GameObject _severedHandsPrefab;

        [SerializeField]
        private int _overrideLayerIndex;

        [SerializeField]
        private float _tickDamage;
        public bool CanBeCleared => !base.IsEnabled;
        public BlockedInteraction BlockedInteractions => BlockedInteraction.All;

        protected override void Enabled()
        {
            base.Enabled();
            base.Hub.interCoordinator.AddBlocker(this);

            AllSeveredHands.Add(new Footprint(base.Hub));

            if (_severedHandsPrefab != null)
            {
                Transform hubTransform = base.Hub.transform;
                Object.Instantiate(_severedHandsPrefab, hubTransform.position, hubTransform.rotation);
            }

            if (_severClip != null)
            {
                AudioSourcePoolManager.PlaySound(
                    _severClip,
                    base.Hub.transform,
                    _severSoundDistance,
                    1f,
                    FalloffType.Linear,
                    AudioMixerChannelType.DefaultSfx,
                    0f,
                    false
                );
            }

            if (base.Hub.TryGetComponent<BloodDrawer>(out var bloodDrawer))
            {
                bloodDrawer.PlaceUnderneath(base.Hub.transform.position, _overrideLayerIndex);
            }

            ChangeHandsState(true);
        }

        protected override void Disabled()
        {
            base.Disabled();
            ChangeHandsState(false);
        }

        protected override void OnTick()
        {
            if (NetworkServer.active)
            {
                InventoryExtensions.ServerDropItem(base.Hub.inventory, base.Hub.inventory.CurItem.SerialNumber);
                base.Hub.playerStats.DealDamage(
                    new UniversalDamageHandler(_tickDamage, DeathTranslations.SeveredHands)
                );
            }
        }

        private void ChangeHandsState(bool handsCut)
        {
            if (base.Hub.roleManager.CurrentRole is HumanRole humanRole
                && humanRole.FpcModule.CharacterModelInstance is HumanCharacterModel humanCharacterModel)
            {
                humanCharacterModel.Animator.SetBool(HashSeveredHands, handsCut);
            }
        }
    }
}