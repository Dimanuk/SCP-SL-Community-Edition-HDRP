
using PlayerRoles.Spectating;
using PostProcessing;
using UnityEngine;

namespace CustomPlayerEffects
{
	public class Decontaminating : TickingEffectBase
	{
		private const string CassieAnnouncement = "LOST IN DECONTAMINATION SEQUENCE";

		public float FogFadeInSpeed;

		public float FogFadeOutSpeed;

		private bool AffectsLocalFog =>
			base.Hub != null && (base.Hub.isLocalPlayer || SpectatorNetworking.IsLocallySpectated(base.Hub));

        protected override void OnTick()
        {
            if (global::Mirror.NetworkServer.active && base.Hub.roleManager.CurrentRole is global::PlayerRoles.IHealthbarRole healthbarRole)
            {
                float damage = healthbarRole.MaxHealth / 10f;
                global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement cassieAnnouncement = new global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement();
                cassieAnnouncement.Announcement = "LOST IN DECONTAMINATION SEQUENCE";
                cassieAnnouncement.SubtitleParts = new global::Subtitles.SubtitlePart[1]
                {
                    new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.LostInDecontamination, (string[])null)
                };
                global::PlayerStatsSystem.DamageHandlerBase.CassieAnnouncement cassieAnnouncement2 = cassieAnnouncement;
                base.Hub.playerStats.DealDamage(new global::PlayerStatsSystem.UniversalDamageHandler(damage, global::PlayerStatsSystem.DeathTranslations.Decontamination, cassieAnnouncement2));
            }
        }

        protected override void Enabled()
        {
            base.Enabled();
            if (AffectsLocalFog)
            {
                FogController.EnableFogType(FogType.Decontamination, FogFadeInSpeed);
            }
        }

        public override void OnBeginSpectating()
        {
            if (base.IsEnabled)
            {
                FogController.EnableFogType(FogType.Decontamination, 0f);
            }
            else
            {
                FogController.DisableFogType(FogType.Decontamination, 0f);
            }
        }

        public override void OnStopSpectating()
        {
            FogController.DisableFogType(FogType.Decontamination, 0f);
        }

        protected override void Disabled()
        {
            if (AffectsLocalFog)
            {
                FogController.DisableFogType(FogType.Decontamination, FogFadeOutSpeed);
            }
        }
	}
}
