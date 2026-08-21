using System;
using InventorySystem.Drawers;
using InventorySystem.Items.Pickups;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Usables
{
    public abstract class UsableItem : ItemBase, IEquipDequipModifier, IItemAlertDrawer, IItemDrawer, IItemDescription, IItemNametag
    {
        [NonSerialized]
        public float RemainingCooldown;

        [NonSerialized]
        public bool IsUsing;

        public float UseTime;
        public float MaxCancellableTime;

        [SerializeField]
        private float _weight = 1f;

        private static KeyCode _useKey;
        private static KeyCode _cancelKey;
        private static string _cooldownFormat;

        public const float AudibleSfxRange = 15f;

        public AudioClip UsingSfxClip;

        protected ItemTranslationReader Translation;

        public virtual bool CanStartUsing { get; protected set; } = true;

        public override float Weight => _weight;

        public virtual string AlertText
        {
            get
            {
                if (RemainingCooldown <= 0f)
                    return string.Empty;

                // The countdown lives in this getter: the drawer HUD reads AlertText
                // every frame while the item is held, ticking the cooldown down.
                RemainingCooldown -= Time.deltaTime;

                return string.Format(_cooldownFormat,
                    TimeSpan.FromSeconds(RemainingCooldown).ToString("mm\\:ss"),
                    Name);
            }
        }

        public string Description => Translation?.Description ?? string.Empty;

        public string Name
        {
            get
            {
                if (ItemTypeId == ItemType.SCP1576)
                    return "SCP-1576";

                return Translation?.Name ?? string.Empty;
            }
        }

        public virtual bool AllowHolster => !IsUsing;
        public virtual bool AllowEquip => true;

        public abstract void ServerOnUsingCompleted();

        public virtual void OnUsingStarted()
        {
            IsUsing = true;

            if (base.IsLocalPlayer)
            {
                ItemViewmodelBase viewModel = ViewModel;  

                if (viewModel != null)
                {
                    if (viewModel is UsableItemViewmodel usableViewmodel)
                    {
                        usableViewmodel.OnUsingStarted();
                    }
                }
            }
        }

        public virtual void OnUsingCancelled()
        {
            IsUsing = false;

            if (base.IsLocalPlayer)
            {
                ItemViewmodelBase viewModel = ViewModel;

                if (viewModel is UsableItemViewmodel usableViewmodel)
                {
                    usableViewmodel.OnUsingCancelled();
                }
            }
        }

        protected void ServerRemoveSelf()
        {
            base.OwnerInventory.ServerRemoveItem(base.ItemSerial, null);
        }

        protected void ServerSetPersonalCooldown(float timeSeconds)
        {
            var handler = UsableItemsController.GetHandler(base.Owner);
            if (handler != null)
                handler.PersonalCooldowns[ItemTypeId] = Time.timeSinceLevelLoad + timeSeconds;
        }

        protected void ServerSetGlobalItemCooldown(float timeSeconds)
        {
            UsableItemsController.GlobalItemCooldowns[base.ItemSerial] = Time.timeSinceLevelLoad + timeSeconds;
        }

        protected void ServerAddRegeneration(AnimationCurve regenCurve, float speedMultiplier = 1f, float hpMultiplier = 1f)
        {
            var handler = UsableItemsController.GetHandler(base.Owner);
            if (handler != null)
            {
                handler.ActiveRegenerations.Add(new RegenerationProcess(regenCurve, speedMultiplier, hpMultiplier));
            }
        }

        public override void OnAdded(ItemPickupBase pickup)
        {
            if (base.IsLocalPlayer)
            {
                _useKey = NewInput.GetKey(ActionName.Shoot);
                _cancelKey = NewInput.GetKey(ActionName.Zoom);

                _cooldownFormat = TranslationReader.Get("Facility", 33, "Wait {0} until you can use {1} again.");
                // Re-wrap the placeholders so the timer value and item name render in the
                // alert's own colour while the surrounding text stays white.
                _cooldownFormat = string.Format(_cooldownFormat, "</color>{0}<color=white>", "</color>{1}<color=white>");
                _cooldownFormat = $"<color=white>{_cooldownFormat}</color>";

                Translation = new ItemTranslationReader(ItemTypeId);
            }
        }

        public override void OnEquipped()
        {
            if (NetworkServer.active)
            {
                float cooldown = UsableItemsController.GetCooldown(base.ItemSerial, this, UsableItemsController.GetHandler(base.Owner));
                base.OwnerInventory.connectionToClient?.Send(new ItemCooldownMessage(base.ItemSerial, cooldown));
            }
        }

        public override void EquipUpdate()
        {
            if (!base.IsLocalPlayer)
                return;

            if (!InventorySystem.GUI.InventoryGuiController.ItemsSafeForInteraction || Cursor.visible)
                return;

            if (UnityEngine.Input.GetKeyDown(_useKey))
            {
                Mirror.NetworkClient.Send(new StatusMessage(StatusMessage.StatusType.Start, base.ItemSerial));
            }

            if (UnityEngine.Input.GetKeyDown(_cancelKey))
            {
                Mirror.NetworkClient.Send(new StatusMessage(StatusMessage.StatusType.Cancel, base.ItemSerial));
            }
        }

        public virtual bool ServerValidateCancelRequest(PlayerHandler handler) => true;
        public virtual bool ServerValidateStartRequest(PlayerHandler handler) => true;

        public UsableItem()
        {
            CanStartUsing = true;
            _weight = 1f;
        }
    }
}