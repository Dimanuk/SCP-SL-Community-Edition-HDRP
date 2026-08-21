using Mirror;
using UnityEngine;

namespace PlayerStatsSystem
{
    public class UniversalDamageHandler : StandardDamageHandler
    {
        private string _ragdollInspectText;
        private string _deathscreenText;
        private string _logsText;
        private readonly CassieAnnouncement _cassieAnnouncement;

        public readonly byte TranslationId;

        public override float Damage { get; protected set; }
        public override string RagdollInspectText => _ragdollInspectText;
        public override string DeathScreenText => _deathscreenText;
        public override CassieAnnouncement CassieDeathAnnouncement => _cassieAnnouncement;
        public override string ServerLogsText => _logsText;

        public UniversalDamageHandler()
        {
            TranslationId = 0;
            _cassieAnnouncement = CassieAnnouncement.Default;
        }

        public UniversalDamageHandler(float damage, DeathTranslation deathReason, CassieAnnouncement cassieAnnouncement = null)
        {
            Damage = damage;
            ApplyTranslation(deathReason);
            
            TranslationId = deathReason.Id;
            
            _cassieAnnouncement = cassieAnnouncement ?? CassieAnnouncement.Default;
        }

        private void ApplyTranslation(DeathTranslation translation)
        {
            _ragdollInspectText = translation.RagdollTranslation;
            _deathscreenText = translation.DeathscreenTranslation;
            _logsText = translation.LogLabel;
        }

        public override void WriteAdditionalData(NetworkWriter writer)
        {
            base.WriteAdditionalData(writer);
            writer.WriteByte(TranslationId);
        }

        public override void ReadAdditionalData(NetworkReader reader)
        {
            base.ReadAdditionalData(reader);
            if (DeathTranslations.TranslationsById.TryGetValue(reader.ReadByte(), out DeathTranslation translation))
            {
                ApplyTranslation(translation);
            }
        }
    }
}