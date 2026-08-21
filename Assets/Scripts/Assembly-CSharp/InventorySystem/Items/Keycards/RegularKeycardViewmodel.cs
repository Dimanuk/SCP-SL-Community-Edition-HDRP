using System;
using System.Diagnostics;
using Interactables;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.SwayControllers;
using MapGeneration.Distributors;
using UnityEngine;
using Mirror;

namespace InventorySystem.Items.Keycards
{
    public class RegularKeycardViewmodel : AnimatedViewmodelBase
    {
        internal static readonly int AnimHash = Animator.StringToHash("Use");

        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Transform _swayPivot;

        private GoopSway _goopSway;
        private const float UseCooldown = 0.5f;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private static RegularKeycardViewmodel _lastTarget;
        public override IItemSwayController SwayController => _goopSway;
        public override float ViewmodelCameraFOV => 40f;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += RegisterUseMessageHandler;
        }

        public override void InitAny()
        {
            base.InitAny();
            var settings = new GoopSway.GoopSwaySettings(
                targetTransform: _swayPivot,
                swayIntensity: 0.65f,
                translationIntensity: 0.0035f,
                zAxisIntensity: 0.04f,
                swaySmoothness: 7f,
                translationSmoothness: 6.5f,
                bobIntensity: 0.03f,
                centrifugalIntensity: 1.6f
            );
            _goopSway = new GoopSway(settings, base.Hub);
        }

        public override void InitLocal(ItemBase ib)
        {
            base.InitLocal(ib);
            _lastTarget = this;

            InteractionCoordinator.OnClientInteracted += OnInteracted;

            SetupKeycard(ib.ItemTypeId);
        }

        public override void InitSpectator(ReferenceHub ply, ItemIdentifier id, bool wasEquipped)
        {
            base.InitSpectator(ply, id, wasEquipped);
            _lastTarget = this;
            SetupKeycard(id.TypeId);

            if (wasEquipped)
            {
                base.AnimatorForceUpdate(base.SkipEquipTime, true);
                if (TryGetComponent<AudioSource>(out var audioSource))
                {
                    audioSource.mute = true;
                }
            }
        }

        private void SetupKeycard(ItemType keycardType)
        {
            if (InventoryItemLoader.TryGetItem<KeycardItem>(keycardType, out KeycardItem item))
            {
                Material material = null;

                if (item.PickupDropModel is KeycardPickup keycardPickup)
                {
                    material = keycardPickup.GetMaterialFromKeycardId(keycardType);
                }

                if (material != null && _meshRenderer != null)
                {
                    _meshRenderer.sharedMaterial = material;
                }
            }
        }

        private void OnInteracted(InteractableCollider col)
        {
            if (this == null || !gameObject.activeInHierarchy)
                return;

            if (_stopwatch.IsRunning && _stopwatch.Elapsed.TotalSeconds < UseCooldown)
                return;

            if (col.Target == null)
                return;

            if (col.Target is DoorVariant door)
            {
                ProcessDoor(door, col.ColliderId);
            }
            else if (col.Target is Locker locker)
            {
                ProcessLocker(locker, col.ColliderId);
            }
        }

        public void ProcessDoor(DoorVariant dv, byte colId)
        {
            if (dv.RequiredPermissions.RequiredPermissions != KeycardPermissions.None)
            {
                if (dv.AllowInteracting(ReferenceHub.LocalHub, colId))
                {
                    PlayInteractAnimations();
                }
            }
        }

        public void ProcessLocker(Locker locker, int chamberId)
        {
            if (chamberId >= 0 && chamberId < locker.Chambers.Length)
            {
                var chamber = locker.Chambers[chamberId];
                if (chamber.RequiredPermissions != KeycardPermissions.None)
                {
                    if (locker.CheckPerms(chamber.RequiredPermissions, ReferenceHub.LocalHub))
                    {
                        PlayInteractAnimations();
                    }
                }
            }
        }

        protected virtual void PlayInteractAnimations()
        {
            _stopwatch.Restart();
            base.AnimatorSetTrigger(AnimHash);

            if (NetworkClient.active)
            {
                NetworkClient.Send(new KeycardItem.UseMessage());
            }
        }

        public static void RegisterUseMessageHandler()
        {
            NetworkClient.ReplaceHandler<KeycardItem.UseMessage>(msg =>
            {
                if (_lastTarget != null)
                {
                    _lastTarget.PlayInteractAnimations();
                }
            }, true);
        }

        private void OnDestroy()
        {
            InteractionCoordinator.OnClientInteracted -= OnInteracted;

            if (_lastTarget == this)
                _lastTarget = null;
        }
    }
}