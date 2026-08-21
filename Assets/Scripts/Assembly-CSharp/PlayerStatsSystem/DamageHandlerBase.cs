
using Mirror;
using PlayerRoles.Ragdolls;
using Subtitles;

namespace PlayerStatsSystem
{
	public abstract class DamageHandlerBase
	{
        public class CassieAnnouncement
        {
            public static readonly global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement Default = new global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement
            {
                Announcement = "SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED",
                SubtitleParts = new global::Subtitles.SubtitlePart[1]
                {
                    new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.TerminationCauseUnspecified, (string[])null)
                }
            };

            public string Announcement;

            public global::Subtitles.SubtitlePart[] SubtitleParts;
        }

        public enum HandlerOutput : byte
		{
			Nothing = 0,
			Damaged = 1,
			Death = 2
		}

		public abstract string RagdollInspectText { get; }

		public abstract string DeathScreenText { get; }

		public abstract string ServerLogsText { get; }

		public abstract CassieAnnouncement CassieDeathAnnouncement { get; }

        public virtual void WriteDeathScreen(global::Mirror.NetworkWriter writer)
        {
            global::PlayerRoles.Spectating.SpectatorSpawnReasonReaderWriter.WriteSpawnReason(writer, global::PlayerRoles.Spectating.SpectatorSpawnReason.Other);
            writer.WriteDamageHandler(this);
        }

        public virtual void WriteAdditionalData(NetworkWriter writer)
		{
			
		}

		public virtual void ReadAdditionalData(NetworkReader reader)
		{
			
		}

		public virtual void ProcessRagdoll(BasicRagdoll ragdoll)
		{
			
		}

		public abstract HandlerOutput ApplyDamage(ReferenceHub ply);
	}
}
