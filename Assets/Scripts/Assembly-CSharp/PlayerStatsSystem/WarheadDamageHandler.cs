using Subtitles;

namespace PlayerStatsSystem
{
    public class WarheadDamageHandler : StandardDamageHandler
    {
        private readonly string _ragdollinspectText;
        private readonly string _deathscreenText;

        public override DamageHandlerBase.CassieAnnouncement CassieDeathAnnouncement
        {
            get
            {
                var announcement = new DamageHandlerBase.CassieAnnouncement();

                announcement.Announcement = "SUCCESSFULLY TERMINATED BY ALPHA WARHEAD";

                announcement.SubtitleParts = new SubtitlePart[1]
                {
                    new SubtitlePart(SubtitleType.TerminatedByWarhead, (string[])null)
                };

                return announcement;
            }
        }

        public override float Damage { get; protected set; }

        public override string RagdollInspectText => _ragdollinspectText;

        public override string DeathScreenText => _deathscreenText;

        public override string ServerLogsText => "Died to alpha warhead.";

        public WarheadDamageHandler()
        {
            Damage = -1f;

            var translation = DeathTranslations.Warhead;
            _ragdollinspectText = translation.RagdollTranslation;
            _deathscreenText = translation.DeathscreenTranslation;
        }
    }
}