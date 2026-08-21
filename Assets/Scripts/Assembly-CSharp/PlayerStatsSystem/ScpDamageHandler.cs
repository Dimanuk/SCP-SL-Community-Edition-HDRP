using Footprinting;
using Mirror;
using System.Runtime.CompilerServices;

namespace PlayerStatsSystem
{
    public class ScpDamageHandler : AttackerDamageHandler
    {
        private string _ragdollInspectText;
        private readonly byte _translationId;

        public override float Damage { get; protected set; }

        public override string RagdollInspectText => _ragdollInspectText;

        public override string DeathScreenText => string.Empty;

        public override CassieAnnouncement CassieDeathAnnouncement => new CassieAnnouncement();

        public override Footprint Attacker { get; [CompilerGenerated] protected set; }

        public override string ServerLogsText
        {
            get
            {
                return string.Concat("Died to SCP (", Attacker.Nickname, ")");
            }
        }

        public override bool AllowSelfDamage => false;

        public ScpDamageHandler()
        {
           
        }

        public ScpDamageHandler(ReferenceHub attacker, float damage, DeathTranslation deathReason) 
        {
            Damage = damage;
            Attacker = new Footprint(attacker);
            _translationId = deathReason.Id;
            _ragdollInspectText = deathReason.RagdollTranslation;
        }

        public ScpDamageHandler(ReferenceHub attacker, DeathTranslation deathReason)
        {
            Damage = StandardDamageHandler.KillValue;
            Attacker = new Footprint(attacker);
            _translationId = deathReason.Id;
            _ragdollInspectText = deathReason.RagdollTranslation;
        }

        public override void WriteAdditionalData(NetworkWriter writer)
        {
            base.WriteAdditionalData(writer);
            writer.WriteByte(_translationId);
        }

        public override void ReadAdditionalData(NetworkReader reader)
        {
            base.ReadAdditionalData(reader);
            byte id = reader.ReadByte();

            if (DeathTranslations.TranslationsById.TryGetValue(id, out DeathTranslation translation))
            {
                _ragdollInspectText = translation.RagdollTranslation;
            }
        }
    }
}