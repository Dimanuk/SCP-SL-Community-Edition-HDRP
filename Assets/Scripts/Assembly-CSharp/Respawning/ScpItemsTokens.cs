using System.Collections.Generic;

using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables;
using PlayerRoles;
using UnityEngine;

namespace Respawning
{
	public static class ScpItemsTokens
	{
		private const float HighReward = 1f;

		private const float MidReward = 0.7f;

		private const float LowReward = 0.4f;

		private const float CandyReward = 0.1f;

        private static readonly global::System.Collections.Generic.Dictionary<ItemType, float> ItemUseRewards = new global::System.Collections.Generic.Dictionary<ItemType, float>
        {
            [ItemType.SCP500] = 0.4f,
            [ItemType.SCP207] = 0.7f,
            [ItemType.SCP018] = 1f,
            [ItemType.SCP268] = 1f,
            [ItemType.SCP330] = 0.1f,
            [ItemType.SCP2176] = 0.7f,
            [ItemType.SCP244a] = 0.7f,
            [ItemType.SCP244b] = 0.7f,
            [ItemType.SCP1853] = 0.7f
        };

        private static readonly global::System.Collections.Generic.HashSet<ushort> AlreadyUsedItems = new global::System.Collections.Generic.HashSet<ushort>();

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += AlreadyUsedItems.Clear;
            global::InventorySystem.Items.Usables.UsableItemsController.ServerOnUsingCompleted += ServerOnUsingCompleted;
            global::InventorySystem.Items.ThrowableProjectiles.ThrownProjectile.OnProjectileSpawned += OnThrown;
        }

        private static void ServerOnUsingCompleted(ReferenceHub ply, global::InventorySystem.Items.Usables.UsableItem ui)
        {
            GrantTokens(ui.ItemSerial, ui.ItemTypeId, global::PlayerRoles.PlayerRolesUtils.GetRoleId(ply));
        }

        private static void OnThrown(global::InventorySystem.Items.ThrowableProjectiles.ThrownProjectile projectile)
        {
            GrantTokens(projectile.Info.Serial, projectile.Info.ItemId, projectile.PreviousOwner.Role);
        }

        private static void GrantTokens(ushort serial, ItemType itemType, global::PlayerRoles.RoleTypeId role)
        {
            if (AlreadyUsedItems.Add(serial) && ItemUseRewards.TryGetValue(itemType, out var value) && role.TryGetAssignedSpawnableTeam(out var stt))
            {
                global::Respawning.RespawnTokensManager.GrantTokens(stt, value);
            }
        }
	}
}
