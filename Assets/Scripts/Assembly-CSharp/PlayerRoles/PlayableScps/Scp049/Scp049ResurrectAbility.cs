using System.Collections.Generic;

using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp049
{
	public class Scp049ResurrectAbility : RagdollAbilityBase<Scp049Role>
	{
		private enum ResurrectError
		{
			None = 0,
			TargetNull = 1,
			Expired = 2,
			MaxReached = 3,
			Refused = 4,
			TargetInvalid = 5
		}

		public const int MaxResurrections = 4;

		private const float TargetCorpseMultiplier = 1.5f;

		private const float HumanCorpseDuration = 10f;

		private const float ResurrectTargetReward = 100f;

        private static readonly global::System.Collections.Generic.Dictionary<uint, int> ResurrectedPlayers = new global::System.Collections.Generic.Dictionary<uint, int>();

        private static readonly global::System.Collections.Generic.HashSet<uint> DeadZombies = new global::System.Collections.Generic.HashSet<uint>();

        private Scp049SenseAbility _senseAbility;

        protected override float RangeSqr => 3.5f;

        protected override float Duration => 10f;

        protected override void ServerComplete()
        {
            ReferenceHub ownerHub = base.CurRagdoll.Info.OwnerHub;
            ownerHub.transform.position = base.ScpRole.FpcModule.Position;
            if (_senseAbility.DeadTargets.Contains(ownerHub))
            {
                global::PlayerRoles.PlayableScps.HumeShield.HumeShieldModuleBase humeShieldModule = base.ScpRole.HumeShieldModule;
                humeShieldModule.HsCurrent = global::UnityEngine.Mathf.Min(humeShieldModule.HsCurrent + 100f, humeShieldModule.HsMax);
            }
            ownerHub.roleManager.ServerSetRole(global::PlayerRoles.RoleTypeId.Scp0492, global::PlayerRoles.RoleChangeReason.Revived);
            global::Mirror.NetworkServer.Destroy(base.CurRagdoll.gameObject);
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine<global::PlayerRoles.PlayableScps.Scp049.Scp049SenseAbility>(out _senseAbility);
        }

        protected override void OnKeyDown()
        {
            base.OnKeyDown();
            ClientTryStart();
        }

        protected override void OnKeyUp()
        {
            base.OnKeyUp();
            ClientTryCancel();
        }

        public bool CheckRagdoll(BasicRagdoll ragdoll)
        {
            return CheckBeginConditions(ragdoll) == global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError.None;
        }

        protected override byte ServerValidateBegin(BasicRagdoll ragdoll)
        {
            global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError resurrectError = CheckBeginConditions(ragdoll);
            if (resurrectError != global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError.None)
            {
                return (byte)resurrectError;
            }
            if (!ServerValidateAny())
            {
                return 1;
            }
            return 0;
        }

        protected override bool ServerValidateAny()
        {
            if (base.CurRagdoll == null)
            {
                return false;
            }
            ReferenceHub ownerHub = base.CurRagdoll.Info.OwnerHub;
            bool flag = ownerHub != null && base.ServerValidateAny() && ownerHub.roleManager.CurrentRole is global::PlayerRoles.Spectating.SpectatorRole && CheckMaxResurrections(ownerHub) == global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError.None;
            return flag;
        }

        private global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError CheckBeginConditions(BasicRagdoll ragdoll)
        {
            ReferenceHub ownerHub = ragdoll.Info.OwnerHub;
            bool flag = ownerHub == null;
            if (ragdoll.Info.RoleType == global::PlayerRoles.RoleTypeId.Scp0492)
            {
                if (flag || !DeadZombies.Contains(ownerHub.netId))
                {
                    return global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError.TargetNull;
                }
                if (!(ragdoll.Info.Handler is global::PlayerStatsSystem.AttackerDamageHandler))
                {
                    return global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError.TargetInvalid;
                }
            }
            else
            {
                float num = 10f;
                if (!flag && _senseAbility.DeadTargets.Contains(ownerHub))
                {
                    num *= 1.5f;
                }
                if (ragdoll.Info.ExistenceTime > num)
                {
                    return global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError.Expired;
                }
                if (!ragdoll.Info.RoleType.IsHuman())
                {
                    return global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError.TargetInvalid;
                }
                if (flag)
                {
                    return global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError.TargetNull;
                }
            }
            return CheckMaxResurrections(ownerHub);
        }

        private global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError CheckMaxResurrections(ReferenceHub owner)
        {
            int resurrectionsNumber = GetResurrectionsNumber(owner);
            if (resurrectionsNumber < 4)
            {
                return global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError.None;
            }
            if (resurrectionsNumber <= 4)
            {
                return global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError.MaxReached;
            }
            return global::PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ResurrectError.Refused;
        }


        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::PlayerRoles.PlayerRoleManager.OnServerRoleSet += delegate (ReferenceHub hub, global::PlayerRoles.RoleTypeId newRole, global::PlayerRoles.RoleChangeReason changeReason)
            {
                if (newRole.IsHuman())
                {
                    ClearPlayerResurrections(hub);
                }
            };
            CustomNetworkManager.OnClientReady += DeadZombies.Clear;
            ReferenceHub.OnPlayerRemoved = (global::System.Action<ReferenceHub>)global::System.Delegate.Combine(ReferenceHub.OnPlayerRemoved, (global::System.Action<ReferenceHub>)delegate (ReferenceHub hub)
            {
                DeadZombies.Remove(hub.netId);
            });
            global::PlayerRoles.PlayerRoleManager.OnRoleChanged += delegate (ReferenceHub hub, global::PlayerRoles.PlayerRoleBase prevRole, global::PlayerRoles.PlayerRoleBase newRole)
            {
                if (global::Mirror.NetworkServer.active)
                {
                    if (prevRole is global::PlayerRoles.PlayableScps.Scp049.Zombies.ZombieRole && newRole is global::PlayerRoles.Spectating.SpectatorRole)
                    {
                        DeadZombies.Add(hub.netId);
                    }
                    else
                    {
                        DeadZombies.Remove(hub.netId);
                    }
                }
            };
        }

        public static int GetResurrectionsNumber(ReferenceHub hub)
        {
            if (!ResurrectedPlayers.TryGetValue(hub.netId, out var value))
            {
                return 0;
            }
            return value;
        }


        public static void RegisterPlayerResurrection(ReferenceHub hub, int amount = 1)
        {
            ResurrectedPlayers[hub.netId] = GetResurrectionsNumber(hub) + amount;
        }

        public static void ClearPlayerResurrections(ReferenceHub hub)
        {
            ResurrectedPlayers.Remove(hub.netId);
        }
	}
}
