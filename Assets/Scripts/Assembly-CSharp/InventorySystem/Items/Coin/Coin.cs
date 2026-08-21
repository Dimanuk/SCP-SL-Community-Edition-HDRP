using InventorySystem.GUI;
using InventorySystem.Items.Autosync;
using InventorySystem.Items.Pickups;
using Mirror;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace InventorySystem.Items.Coin
{
    public class Coin : AutosyncItem, IItemDescription, IItemNametag
    {
        [SerializeField] private AudioClip _flipSound;

        private readonly Stopwatch _lastUseSw = Stopwatch.StartNew();
        private const float RateLimit = 0.6f;

        public static readonly Dictionary<ushort, double> FlipTimes = new ();
        private static readonly int IdleHash = Animator.StringToHash("Idle");

        private static readonly ActionName[] ActivationKeys = new ActionName[3]
        {
            ActionName.InspectItem,
            ActionName.Shoot,
            ActionName.Zoom
        };

        private KeyCode[] _activationKeys;
        public string Description { get; set; }
        public string Name { get; set; }

        public static event Action<ushort, bool> OnFlipped;

        public override float Weight => 0.0025f;

        public override void OnAdded(ItemPickupBase pickup)
        {
            if (!IsLocalPlayer) return;

            var reader = new ItemTranslationReader(ItemTypeId);
            Name = reader.Name;
            Description = reader.Description;

            _activationKeys = new KeyCode[ActivationKeys.Length];
            for (int i = 0; i < ActivationKeys.Length; i++)
                _activationKeys[i] = NewInput.GetKey(ActivationKeys[i]);
        }

        public override void EquipUpdate()
        {
            if (!IsLocalPlayer) return;
            if (!InventoryGuiController.ItemsSafeForInteraction) return;
            if (_lastUseSw.Elapsed.TotalSeconds < RateLimit) return;

            // Only allow a new flip once the viewmodel has returned to Idle, so the
            // toss animation/sound can't be re-triggered while one is still playing.
            // The animator state is named "CoinIdle" but carries the tag "Idle" —
            // the original build compares tagHash, not shortNameHash.
            if (ViewModel is AnimatedViewmodelBase animatedViewmodel &&
                animatedViewmodel.GetAnimatorStateInfo(0).tagHash != IdleHash)
                return;

            for (int i = 0; i < _activationKeys.Length; i++)
            {
                if (Input.GetKeyDown(_activationKeys[i]))
                {
                    _lastUseSw.Restart();

                    var cmd = new AutosyncCmd(this);
                    cmd.Send();
                    break;
                }
            }
        }

        internal override void ClientProcessRpcTemplate(NetworkReader reader, ushort serial)
        {
            bool landedHeads = reader.ReadBool();

            OnFlipped?.Invoke(serial, landedHeads);
            FlipTimes[serial] = landedHeads ? NetworkTime.time : -NetworkTime.time;

            if (InventoryExtensions.TryGetHubHoldingSerial(serial, out var hub) && _flipSound != null)
            {
                AudioPooling.AudioSourcePoolManager.PlaySound(
                    _flipSound, hub.transform, 5.5f, 1f, FalloffType.Exponential,
                    AudioPooling.AudioMixerChannelType.DefaultSfx, hub.isLocalPlayer ? 0f : 1f);
            }
        }

        internal override void OnTemplateReloaded(bool wasEverLoaded)
        {
            base.OnTemplateReloaded(wasEverLoaded);

            if (!wasEverLoaded)
                CustomNetworkManager.OnClientReady += FlipTimes.Clear;
        }

        internal override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (!Owner.isLocalPlayer && _lastUseSw.Elapsed.TotalSeconds < RateLimit)
                return;

            bool heads = UnityEngine.Random.value >= 0.5f;
            _lastUseSw.Restart();

            using var rpc = new AutosyncRpc(this, toAll: true, out var writer);
            writer.WriteBool(heads);
        }
    }
}