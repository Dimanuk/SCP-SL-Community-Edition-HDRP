using System.Collections.Generic;

using PlayerRoles;
using PlayerStatsSystem;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace CustomPlayerEffects
{
	public class SpawnProtected : StatusEffectBase, ISpectatorDataPlayerEffect, ICustomRADisplay, IDamageModifierEffect, IPulseEffect
	{
		[SerializeField]
		private PostProcessEffectPulse _pulseEffect;

		public static bool IsProtectionEnabled;

		public static bool CanShoot;

		public static bool PreventAllDamage;

		public static float SpawnDuration;

		public static readonly List<Team> ProtectedTeams = new();

		private static bool _eventAssigned;

		public string DisplayName => "Spawn Protection";

		public bool CanBeDisplayed => true;

		public override EffectClassification Classification => EffectClassification.Positive;

        internal override void OnRoleChanged(global::PlayerRoles.PlayerRoleBase previousRole, global::PlayerRoles.PlayerRoleBase newRole)
        {
            if (!TryGiveProtection(base.Hub))
            {
                base.OnRoleChanged(previousRole, newRole);
            }
        }

        public float GetDamageModifier(float baseDamage, global::PlayerStatsSystem.DamageHandlerBase handler, HitboxType hitboxType)
        {
            if (!IsProtectionEnabled)
            {
                return 1f;
            }
            if (!(handler is global::PlayerStatsSystem.AttackerDamageHandler attackerDamageHandler))
            {
                if (!PreventAllDamage)
                {
                    return 1f;
                }
                return CancelDamage();
            }
            if (!(attackerDamageHandler.Attacker.Hub == base.Hub))
            {
                return CancelDamage();
            }
            return 1f;
        }

        public bool GetSpectatorText(out string display)
        {
            display = "Spawn protected";
            return base.IsEnabled;
        }

        public void ExecutePulse()
		{
			_pulseEffect.enabled = true;
		}

		private float CancelDamage()
		{
			base.Hub.playerEffectsController.ServerSendPulse<SpawnProtected>();
			return 0f;
		}

        public static bool CheckPlayer(ReferenceHub ply)
        {
            if (!global::Mirror.NetworkServer.active || !IsProtectionEnabled)
            {
                return false;
            }
            return ply.playerEffectsController.GetEffect<global::CustomPlayerEffects.SpawnProtected>().IsEnabled;
        }

        public static bool TryGiveProtection(ReferenceHub hub)
        {
            if (!global::Mirror.NetworkServer.active || !IsProtectionEnabled)
            {
                return false;
            }
            global::PlayerRoles.PlayerRoleBase currentRole = hub.roleManager.CurrentRole;
            if (!(currentRole is global::PlayerRoles.IHealthbarRole))
            {
                return false;
            }
            if (!ProtectedTeams.Contains(currentRole.Team))
            {
                return false;
            }
            hub.playerEffectsController.EnableEffect<global::CustomPlayerEffects.SpawnProtected>(SpawnDuration);
            return true;
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            if (!_eventAssigned)
            {
                global::GameCore.ConfigFile.OnConfigReloaded = (global::System.Action)global::System.Delegate.Combine(global::GameCore.ConfigFile.OnConfigReloaded, new global::System.Action(RefreshConfigs));
                ServerConfigSynchronizer.OnRefreshed = (global::System.Action)global::System.Delegate.Combine(ServerConfigSynchronizer.OnRefreshed, new global::System.Action(RefreshConfigs));
                _eventAssigned = true;
            }
            RefreshConfigs();
        }

        private static void RefreshConfigs()
        {
            ProtectedTeams.Clear();
            IsProtectionEnabled = global::GameCore.ConfigFile.ServerConfig.GetBool("spawn_protect_enabled");
            CanShoot = global::GameCore.ConfigFile.ServerConfig.GetBool("spawn_protect_can_shoot");
            PreventAllDamage = global::GameCore.ConfigFile.ServerConfig.GetBool("spawn_protect_prevent_all");
            SpawnDuration = global::GameCore.ConfigFile.ServerConfig.GetFloat("spawn_protect_time", 8f);
            global::System.Collections.Generic.List<int> intList = global::GameCore.ConfigFile.ServerConfig.GetIntList("spawn_protect_team");
            if (intList.Count == 0)
            {
                ProtectedTeams.Add(global::PlayerRoles.Team.FoundationForces);
                ProtectedTeams.Add(global::PlayerRoles.Team.ChaosInsurgency);
            }
            else
            {
                intList.ForEach(delegate (int t)
                {
                    ProtectedTeams.Add((global::PlayerRoles.Team)t);
                });
            }
        }
    }
}
