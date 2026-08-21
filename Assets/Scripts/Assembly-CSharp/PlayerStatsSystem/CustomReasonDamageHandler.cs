
using Mirror;

namespace PlayerStatsSystem
{
	public class CustomReasonDamageHandler : StandardDamageHandler
	{
		private string _deathReason;

		private readonly CassieAnnouncement _cassieAnnouncement;

		public override float Damage { get; protected set; }

		public override string RagdollInspectText => _deathReason;

		public override string DeathScreenText => _deathReason;

		public override CassieAnnouncement CassieDeathAnnouncement => _cassieAnnouncement;

        public override string ServerLogsText => "Killed with a custom reason - " + _deathReason;
        public CustomReasonDamageHandler(string customReason)
        {
            _deathReason = customReason;
            Damage = -1f;
            _cassieAnnouncement = new global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement();
        }

        public CustomReasonDamageHandler(string customReason, float damage, string customCassieAnnouncement = "")
        {
            _deathReason = customReason;
            Damage = damage;
            _cassieAnnouncement = new global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement();
            _cassieAnnouncement.Announcement = customCassieAnnouncement;
            _cassieAnnouncement.SubtitleParts = new global::Subtitles.SubtitlePart[1]
            {
                new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.Custom, customCassieAnnouncement)
            };
        }

        public override void WriteAdditionalData(global::Mirror.NetworkWriter writer)
        {
            base.WriteAdditionalData(writer);
            global::Mirror.NetworkWriterExtensions.WriteString(writer, _deathReason);
        }

        public override void ReadAdditionalData(global::Mirror.NetworkReader reader)
        {
            base.ReadAdditionalData(reader);
            _deathReason = global::Mirror.NetworkReaderExtensions.ReadString(reader);
        }
    }
}
