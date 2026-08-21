using System;
using UnityEngine;
using Mirror;
using InventorySystem.Items.Pickups;
using InventorySystem.GUI;

namespace InventorySystem.Items.Flashlight
{
    public class FlashlightItem : ItemBase, IItemDescription, IItemNametag, ILightEmittingItem
    {
        private const float ToggleCooldownTime = 0.13f;
        private const float EquipCooldownTime = 0.6f;

        private float _nextAllowedTime;
        private bool _isEmitting;
        private Light _lightSource;

        public AudioClip OnClip;
        public AudioClip OffClip;

        private static readonly ActionName[] ToggleKeys;
        private static FlashlightItem _cachedFlashlight;
        private static bool _cacheSet;

        public override float Weight => 0.7f;

        public string Description { get; set; }
        public string Name { get; set; }

        public bool IsEmittingLight
        {
            get => _isEmitting;
            set
            {
                if (_isEmitting != value)
                {
                    _isEmitting = value;
                    if (Owner != null && Owner.isLocalPlayer)
                    {
                        if (_lightSource != null)
                            _lightSource.enabled = value;

                        AudioClip clip = value ? OnClip : OffClip;
                        if (clip != null)
                        {
                            AudioPooling.AudioSourcePoolManager.PlaySound(clip, transform.position, 1);
                        }
                    }
                }
            }
        }

        public static FlashlightItem Template
        {
            get
            {
                if (!_cacheSet)
                {
                    if (InventoryItemLoader.TryGetItem<FlashlightItem>(ItemType.Flashlight, out var template))
                    {
                        _cachedFlashlight = template;
                        _cacheSet = true;
                    }
                }
                return _cachedFlashlight;
            }
        }

        public override void OnAdded(ItemPickupBase pickup)
        {
            base.OnAdded(pickup);
            if (Owner != null && Owner.isLocalPlayer)
            {
                ItemTranslationReader reader = new(ItemTypeId);
                Name = reader.Name;
                Description = reader.Description;

                if (ViewModel != null)
                {
                    _lightSource = ViewModel.GetComponentInChildren<Light>(true);
                }
            }
        }

        public override void OnEquipped()
        {
            _nextAllowedTime = Time.timeSinceLevelLoad + EquipCooldownTime;
            IsEmittingLight = true;

            if (NetworkServer.active)
            {
                global::Utils.Networking.NetworkUtils.SendToAuthenticated(new FlashlightNetworkHandler.FlashlightMessage(ItemSerial, IsEmittingLight));
            }
        }

        public override void EquipUpdate()
        {
            if (!Owner.isLocalPlayer || !InventoryGuiController.ItemsSafeForInteraction)
                return;

            if (Time.timeSinceLevelLoad < _nextAllowedTime)
                return;

            foreach (ActionName action in ToggleKeys)
            {
                if (NewInput.GetKeyDown(action))
                {
                    if (ViewModel is FlashlightViewmodel flashlightVM)
                    {
                        flashlightVM.PlayAnimation();
                        _nextAllowedTime = Time.timeSinceLevelLoad + ToggleCooldownTime;
                    }
                    break;
                }
            }
        }

        public void ClientSendRequest(bool value)
        {
            if (Owner.isLocalPlayer && value != _isEmitting)
            {
                IsEmittingLight = value;
                NetworkClient.Send(new FlashlightNetworkHandler.FlashlightMessage(ItemSerial, value));
            }
        }

        static FlashlightItem()
        {
            ToggleKeys = new ActionName[]
            {
                ActionName.Shoot,
                ActionName.Zoom,
                ActionName.ToggleFlashlight
            };
        }
    }
}