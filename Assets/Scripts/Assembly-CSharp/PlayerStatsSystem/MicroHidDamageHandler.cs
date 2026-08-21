using System.Runtime.CompilerServices;

using Footprinting;
using InventorySystem.Items.MicroHID;

namespace PlayerStatsSystem
{
	public class MicroHidDamageHandler : AttackerDamageHandler
	{
		private readonly string _deathScreenText;

		private readonly string _serverLogsText;

		private readonly string _ragdollInspectText;

		public override float Damage { get; protected set; }

		public override Footprint Attacker { get; protected set; }
        public override bool AllowSelfDamage => false;

		public override string ServerLogsText => _serverLogsText;

		public override string RagdollInspectText => _ragdollInspectText;

		public override string DeathScreenText => _deathScreenText;

        public MicroHidDamageHandler(global::InventorySystem.Items.MicroHID.MicroHIDItem micro, float impulseDamage)
        {
            DeathTranslation translation = global::PlayerStatsSystem.DeathTranslations.MicroHID;
            _ragdollInspectText = translation.RagdollTranslation;
            _deathScreenText = translation.DeathscreenTranslation;

            if (!(micro == null))
            {
                Attacker = new global::Footprinting.Footprint(micro.Owner);
                _serverLogsText = "Deep fried by " + Attacker.Nickname + " using " + translation.LogLabel;
                Damage = impulseDamage;
            }
        }
    }
}
