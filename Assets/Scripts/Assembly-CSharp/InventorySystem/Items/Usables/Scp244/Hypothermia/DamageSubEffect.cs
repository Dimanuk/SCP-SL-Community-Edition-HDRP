using System;

using UnityEngine;

namespace InventorySystem.Items.Usables.Scp244.Hypothermia
{
	public class DamageSubEffect : HypothermiaSubEffectBase
	{
		private float _damageCounter;

		[SerializeField]
		private TemperatureSubEffect _temperature;

		[SerializeField]
		private AnimationCurve _damageOverTemperature;


        public override bool IsActive => _temperature.IsActive;

        internal override void UpdateEffect(float curExposure)
        {
            if (global::Mirror.NetworkServer.active && !global::CustomPlayerEffects.SpawnProtected.CheckPlayer(base.Hub) && (!(base.Hub.roleManager.CurrentRole is global::PlayerRoles.PlayableScps.HumeShield.IHumeShieldedRole humeShieldedRole) || !(humeShieldedRole.HumeShieldModule.HsCurrent > 0f)))
            {
                DealDamage(_temperature.CurTemperature);
            }
        }


        private void DealDamage(float curTemp)
        {
            _damageCounter += global::UnityEngine.Mathf.Max(_damageOverTemperature.Evaluate(curTemp) * global::UnityEngine.Time.deltaTime, 0f);
            if (!(_damageCounter < 1f))
            {
                base.Hub.playerStats.DealDamage(new global::PlayerStatsSystem.UniversalDamageHandler(_damageCounter, global::PlayerStatsSystem.DeathTranslations.Hypothermia));
                _damageCounter = 0f;
            }
        }
    }
}
