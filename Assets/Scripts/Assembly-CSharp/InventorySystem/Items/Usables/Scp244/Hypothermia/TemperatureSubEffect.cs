
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp244.Hypothermia
{
	public class TemperatureSubEffect : HypothermiaSubEffectBase
	{
		[SerializeField]
		private float _maxExitTemp;

		[SerializeField]
		private float _temperatureDrop;

		public float CurTemperature { get; private set; }

        public override bool IsActive => CurTemperature > 0f;

        internal override void UpdateEffect(float curExposure)
        {
            if (!global::PlayerRoles.PlayerRolesUtils.IsAlive(base.Hub))
            {
                DisableEffect();
            }
            else if (curExposure == 0f)
            {
                if (CurTemperature != 0f)
                {
                    float value = CurTemperature - _temperatureDrop * global::UnityEngine.Time.deltaTime;
                    CurTemperature = global::UnityEngine.Mathf.Clamp(value, 0f, _maxExitTemp);
                }
            }
            else
            {
                CurTemperature += curExposure * global::CustomPlayerEffects.RainbowTaste.CurrentMultiplier(base.Hub) * global::UnityEngine.Time.deltaTime;
            }
        }

        public override void DisableEffect()
		{
			CurTemperature = 0f;
		}
	}
}
