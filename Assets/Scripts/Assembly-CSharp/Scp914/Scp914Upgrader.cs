using System;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Pickups;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using Scp914.Processors;
using UnityEngine;

namespace Scp914
{
    public static class Scp914Upgrader
    {
        public static int SolidObjectMask;

        public static Action<ItemPickupBase, Scp914KnobSetting> OnPickupUpgraded;
        public static Action<ItemBase, Scp914KnobSetting> OnInventoryItemUpgraded;

        public static event Func<ReferenceHub, Scp914KnobSetting, Vector3, (bool Allow, Scp914KnobSetting Setting, Vector3 Position)> OnPlayerProcess;
        public static event Func<ItemPickupBase, Vector3, Scp914KnobSetting, (bool Allow, Scp914KnobSetting Setting, Vector3 Position)> OnPickupProcess;

        public static event Action<ReferenceHub, ItemBase, Scp914KnobSetting> OnInventoryItemProcessAttempt;
        public static event Action<ReferenceHub, ItemBase, Scp914KnobSetting> OnInventoryItemUpgradedFull;

        public static event Action<ItemPickupBase, Vector3, Scp914KnobSetting> OnPickupUpgradedFull;


        public static void Upgrade(Collider[] intake, Vector3 moveVector, Scp914Mode mode, Scp914KnobSetting setting)
        {
            if (!NetworkServer.active)
            {
                throw new InvalidOperationException("Scp914Upgrader.Upgrade is a serverside-only method.");
            }

            HashSet<GameObject> processedRoots = HashSetPool<GameObject>.Shared.Rent();
            bool upgradeDropped = (mode & Scp914Mode.Dropped) == Scp914Mode.Dropped;
            bool upgradeInventory = (mode & Scp914Mode.Inventory) == Scp914Mode.Inventory;
            bool heldOnly = upgradeInventory && (mode & Scp914Mode.Held) == Scp914Mode.Held;

            for (int i = 0; i < intake.Length; i++)
            {
                GameObject root = intake[i].transform.root.gameObject;
                if (!processedRoots.Add(root))
                    continue;

                if (ReferenceHub.TryGetHub(root, out var hub))
                {
                    ProcessPlayer(hub, upgradeInventory, heldOnly, moveVector, setting);
                }
                else if (root.TryGetComponent<ItemPickupBase>(out var pickup))
                {
                    ProcessPickup(pickup, upgradeDropped, moveVector, setting);
                }
            }

            HashSetPool<GameObject>.Shared.Return(processedRoots);
        }

        private static void ProcessPlayer(ReferenceHub ply, bool upgradeInventory, bool heldOnly, Vector3 moveVector, Scp914KnobSetting setting)
        {
            if (Physics.Linecast(ply.transform.position, Scp914Controller.Singleton.IntakeChamber.position, SolidObjectMask))
                return;

            Vector3 outPosition = ply.transform.position + moveVector;

            var (Allow, Setting, Position) = OnPlayerProcess?.Invoke(ply, setting, outPosition)
                ?? (true, setting, outPosition);

            if (!Allow)
                return;

            setting = Setting;
            outPosition = Position;

            ply.TryOverridePosition(outPosition, Vector3.zero);

            if (!upgradeInventory)
                return;

            HashSet<ushort> toProcess = HashSetPool<ushort>.Shared.Rent();

            foreach (var item in ply.inventory.UserInventory.Items)
            {
                if (!heldOnly || item.Key == ply.inventory.CurItem.SerialNumber)
                {
                    toProcess.Add(item.Key);
                }
            }

            foreach (ushort serial in toProcess)
            {
                if (!ply.inventory.UserInventory.Items.TryGetValue(serial, out var itemBase))
                    continue;

                if (!TryGetProcessor(itemBase.ItemTypeId, out var processor))
                    continue;

                OnInventoryItemProcessAttempt?.Invoke(ply, itemBase, setting);

                OnInventoryItemUpgraded?.Invoke(itemBase, setting);

                ItemBase newItem = processor.OnInventoryItemUpgraded(setting, ply, serial);

                if (newItem != null)
                {
                    OnInventoryItemUpgradedFull?.Invoke(ply, newItem, setting);
                }
            }

            HashSetPool<ushort>.Shared.Return(toProcess);

            BodyArmorUtils.RemoveEverythingExceedingLimits(
                ply.inventory,
                ply.inventory.TryGetBodyArmor(out var armor) ? armor : null
            );
        }

        private static void ProcessPickup(ItemPickupBase pickup, bool upgradeDropped, Vector3 moveVector, Scp914KnobSetting setting)
        {
            if (pickup.Info.Locked || !upgradeDropped)
                return;

            if (!TryGetProcessor(pickup.Info.ItemId, out var processor))
                return;

            Vector3 outputPosition = pickup.transform.position + moveVector;

            var (Allow, Setting, Position) = OnPickupProcess?.Invoke(pickup, outputPosition, setting)
                ?? (true, setting, outputPosition);

            if (!Allow)
                return;

            outputPosition = Position;
            setting = Setting;

            OnPickupUpgraded?.Invoke(pickup, setting);

            ItemPickupBase newPickup = processor.OnPickupUpgraded(setting, pickup, outputPosition);

            if (newPickup != null)
            {
                OnPickupUpgradedFull?.Invoke(newPickup, outputPosition, setting);
            }
        }

        private static bool TryGetProcessor(ItemType itemType, out Scp914ItemProcessor processor)
        {
            if (InventoryItemLoader.AvailableItems.TryGetValue(itemType, out var item) &&
                item.TryGetComponent<Scp914ItemProcessor>(out processor))
            {
                return true;
            }

            processor = null;
            return false;
        }
    }
}