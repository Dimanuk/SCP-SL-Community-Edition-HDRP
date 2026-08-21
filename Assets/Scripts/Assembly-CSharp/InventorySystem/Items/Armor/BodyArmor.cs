using System;
using Mirror;
using UnityEngine;
using PlayerRoles;
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using PlayerRoles.FirstPersonControl;                   

namespace InventorySystem.Items.Armor
{
    public class BodyArmor : ItemBase,
        IEquipDequipModifier,
        IWearableItem,
        IItemNametag,                     
        ICustomSearchCompletorItem,
        IMovementSpeedModifier,
        IStaminaModifier
    {
        [Serializable]
        public struct ArmorAmmoLimit
        {
            public ItemType AmmoType;
            public ushort Limit;
        }

        [Serializable]
        public struct ArmorCategoryLimitModifier
        {
            public ItemCategory Category;
            public byte Limit;
        }


        [NonSerialized] public bool DontRemoveExcessOnDrop;

        [Range(0f, 100f)]
        public int HelmetEfficacy;               

        [Range(0f, 100f)]
        public int VestEfficacy;                 

        public float CivilianClassDownsidesMultiplier = 1f;

        public ArmorAmmoLimit[] AmmoLimits;
        public ArmorCategoryLimitModifier[] CategoryLimits;

        [SerializeField, Range(1f, 2f)]
        private float _staminaUseMultiplier = 1f;

        [SerializeField, Range(0f, 1f)]
        private float _movementSpeedMultiplier = 1f;

        [SerializeField]
        private float _weight = 1f;

        public override float Weight => _weight;

        public bool IsWorn => true;

        public bool AllowEquip => true;

        public bool AllowHolster => true;

        public bool MovementModifierActive => IsWorn;
        public float MovementSpeedMultiplier => ProcessMultiplier(_movementSpeedMultiplier);
        public float MovementSpeedLimit => float.MaxValue;

        public bool StaminaModifierActive => IsWorn;
        public float StaminaUsageMultiplier => ProcessMultiplier(_staminaUseMultiplier);
        public bool SprintingDisabled => false;
        public float StaminaRegenMultiplier => 1f;

        public override ItemDescriptionType DescriptionType => ItemDescriptionType.Armor;

        private string _name = "[NONE]";

        string IItemNametag.Name => _name;

        private float ProcessMultiplier(float value)
        {
            var team = PlayerRolesUtils.GetTeam(Owner);
            if (team == Team.ClassD || team == Team.Scientists)
            {
                value = (value - 1f) * CivilianClassDownsidesMultiplier + 1f;
            }
            return value;
        }

        public override void OnAdded(ItemPickupBase pickup)
        {
            if (IsLocalPlayer)
            {
                var reader = new ItemTranslationReader(ItemTypeId);
                _name = reader.Name;
            }
        }

        public SearchCompletor GetCustomSearchCompletor(ReferenceHub hub,
                                                       ItemPickupBase ipb,
                                                       ItemBase ib,
                                                       double disSqrt)
        {
            return new ArmorSearchCompletor(hub, ipb, ib, disSqrt);
        }

        public override void OnRemoved(ItemPickupBase pickup)
        {
            base.OnRemoved(pickup);

            if (NetworkServer.active && !DontRemoveExcessOnDrop)
            {
                BodyArmorUtils.RemoveEverythingExceedingLimits(OwnerInventory, null);
            }
        }
    }
}
