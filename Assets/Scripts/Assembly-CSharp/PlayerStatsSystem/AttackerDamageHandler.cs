
using Footprinting;
using Mirror;
using UnityEngine;

namespace PlayerStatsSystem
{
	public abstract class AttackerDamageHandler : StandardDamageHandler
	{
        private static float _ffMultiplier = 1f;

        private static bool _eventAssigned = false;

        public abstract Footprint Attacker { get; protected set; }

		public bool IsFriendlyFire { get; private set; }

		public bool IsSuicide { get; private set; }

        public override global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement CassieDeathAnnouncement
        {
            get
            {
                if (!Attacker.IsSet || !global::PlayerRoles.PlayerRoleLoader.TryGetRoleTemplate<global::PlayerRoles.PlayerRoleBase>(Attacker.Role, out var result))
                {
                    return global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement.Default;
                }
                global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement cassieAnnouncement = new global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement();
                switch (result.Team)
                {
                    case global::PlayerRoles.Team.ClassD:
                        cassieAnnouncement.Announcement = "CONTAINEDSUCCESSFULLY BY CLASSD PERSONNEL";
                        cassieAnnouncement.SubtitleParts = new global::Subtitles.SubtitlePart[1]
                        {
                        new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.ContainedByClassD, (string[])null)
                        };
                        break;
                    case global::PlayerRoles.Team.ChaosInsurgency:
                        cassieAnnouncement.Announcement = "CONTAINEDSUCCESSFULLY BY CHAOSINSURGENCY";
                        cassieAnnouncement.SubtitleParts = new global::Subtitles.SubtitlePart[1]
                        {
                        new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.ContainedByChaos, (string[])null)
                        };
                        break;
                    case global::PlayerRoles.Team.Scientists:
                        cassieAnnouncement.Announcement = "CONTAINEDSUCCESSFULLY BY SCIENCE PERSONNEL";
                        cassieAnnouncement.SubtitleParts = new global::Subtitles.SubtitlePart[1]
                        {
                        new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.ContainedByScientist, (string[])null)
                        };
                        break;
                    case global::PlayerRoles.Team.FoundationForces:
                        {
                            if (!global::Respawning.NamingRules.UnitNamingRule.TryGetNamingRule(global::Respawning.SpawnableTeamType.NineTailedFox, out var rule))
                            {
                                cassieAnnouncement.Announcement = "CONTAINEDSUCCESSFULLY CONTAINMENTUNIT UNKNOWN";
                                cassieAnnouncement.SubtitleParts = new global::Subtitles.SubtitlePart[1]
                                {
                            new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.ContainUnitUnknown, (string[])null)
                                };
                            }
                            else
                            {
                                string cassieUnitName = rule.GetCassieUnitName(Attacker.UnitName);
                                cassieAnnouncement.Announcement = "CONTAINEDSUCCESSFULLY CONTAINMENTUNIT " + cassieUnitName;
                                cassieAnnouncement.SubtitleParts = new global::Subtitles.SubtitlePart[1]
                                {
                            new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.ContainUnit, Attacker.UnitName)
                                };
                            }
                            break;
                        }
                    case global::PlayerRoles.Team.SCPs:
                        {
                            NineTailedFoxAnnouncer.ConvertSCP(Attacker.Role, out var withoutSpace, out var withSpace);
                            cassieAnnouncement.Announcement = "TERMINATED BY SCP " + withSpace;
                            cassieAnnouncement.SubtitleParts = new global::Subtitles.SubtitlePart[1]
                            {
                        new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.TerminatedBySCP, withoutSpace)
                            };
                            break;
                        }
                    default:
                        return global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement.Default;
                }
                return cassieAnnouncement;
            }
        }

        public abstract bool AllowSelfDamage { get; }

		protected virtual bool ForceFullFriendlyFire { get; set; }

        public virtual bool IgnoreFriendlyFireDetector => false;

        protected override void ProcessDamage(ReferenceHub ply)
        {
            if (DisableSpawnProtect(Attacker.Hub, ply))
            {
                Damage = 0f;
                return;
            }
            if (ply.networkIdentity.netId == Attacker.NetId || ForceFullFriendlyFire)
            {
                if (!AllowSelfDamage && !ForceFullFriendlyFire)
                {
                    Damage = 0f;
                    return;
                }
                IsSuicide = true;
            }
            else if (!HitboxIdentity.CheckFriendlyFire(Attacker.Role, global::PlayerRoles.PlayerRolesUtils.GetRoleId(ply), ignoreConfig: true))
            {
                Damage *= _ffMultiplier;
                IsFriendlyFire = true;
            }
            base.ProcessDamage(ply);
        }

        private bool DisableSpawnProtect(ReferenceHub attacker, ReferenceHub target)
        {
            if (attacker == null)
            {
                return false;
            }
            if (global::CustomPlayerEffects.SpawnProtected.CanShoot && global::CustomPlayerEffects.SpawnProtected.CheckPlayer(attacker))
            {
                return attacker != target;
            }
            return false;
        }

        public override void WriteDeathScreen(global::Mirror.NetworkWriter writer)
        {
            global::PlayerRoles.Spectating.SpectatorSpawnReasonReaderWriter.WriteSpawnReason(writer, global::PlayerRoles.Spectating.SpectatorSpawnReason.KilledByPlayer);
            global::Mirror.NetworkWriterExtensions.WriteString(writer, Attacker.Nickname);
            global::PlayerRoles.PlayerRoleEnumsReadersWriters.WriteRoleType(writer, Attacker.Role);
        }

        public override void WriteAdditionalData(global::Mirror.NetworkWriter writer)
        {
            base.WriteAdditionalData(writer);
            global::Utils.Networking.ReferenceHubReaderWriter.WriteReferenceHub(writer, Attacker.Hub);
        }

        public override void ReadAdditionalData(global::Mirror.NetworkReader reader)
        {
            base.ReadAdditionalData(reader);
            Attacker = new global::Footprinting.Footprint(global::Utils.Networking.ReferenceHubReaderWriter.ReadReferenceHub(reader));
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void RefreshConfigs()
        {
            if (!_eventAssigned)
            {
                global::GameCore.ConfigFile.OnConfigReloaded = (global::System.Action)global::System.Delegate.Combine(global::GameCore.ConfigFile.OnConfigReloaded, new global::System.Action(RefreshConfigs));
                ServerConfigSynchronizer.OnRefreshed = (global::System.Action)global::System.Delegate.Combine(ServerConfigSynchronizer.OnRefreshed, new global::System.Action(RefreshConfigs));
                _eventAssigned = true;
            }
            _ffMultiplier = (ServerConsole.FriendlyFire ? global::GameCore.ConfigFile.ServerConfig.GetFloat("friendly_fire_multiplier", 0.4f) : 0f);
        }
	}
}
