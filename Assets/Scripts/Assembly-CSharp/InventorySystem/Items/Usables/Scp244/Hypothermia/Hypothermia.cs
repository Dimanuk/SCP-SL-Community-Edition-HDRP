
using CustomPlayerEffects;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Searching;
using PlayerRoles.FirstPersonControl;

namespace InventorySystem.Items.Usables.Scp244.Hypothermia
{
	public class Hypothermia : ParentEffectBase<HypothermiaSubEffectBase>, IWeaponModifierPlayerEffect, ISoundtrackMutingEffect, ISearchTimeModifier, IMovementSpeedModifier
	{
		private float _curExposure;

		private IWeaponModifierPlayerEffect _weaponModifier;

		private const float IntensityRatio = 0.1f;

		public bool MuteSoundtrack { get; private set; }

		public bool ParamsActive { get; private set; }

        public bool MovementModifierActive => base.IsEnabled;

        public float MovementSpeedMultiplier { get; private set; }

		public float MovementSpeedLimit { get; private set; }

        public float ProcessSearchTime(float val)
        {
            global::InventorySystem.Items.Usables.Scp244.Hypothermia.HypothermiaSubEffectBase[] subEffects = base.SubEffects;
            for (int i = 0; i < subEffects.Length; i++)
            {
                if (subEffects[i] is global::InventorySystem.Searching.ISearchTimeModifier searchTimeModifier)
                {
                    val = searchTimeModifier.ProcessSearchTime(val);
                }
            }
            return val;
        }


        protected override void Update()
        {
            base.Update();
            _curExposure = 0f;
            if (!global::CustomPlayerEffects.Vitality.CheckPlayer(base.Hub))
            {
                UpdateExposure();
            }
            bool flag = false;
            ParamsActive = false;
            MuteSoundtrack = false;
            MovementSpeedLimit = float.MaxValue;
            MovementSpeedMultiplier = 1f;
            global::InventorySystem.Items.Usables.Scp244.Hypothermia.HypothermiaSubEffectBase[] subEffects = base.SubEffects;
            foreach (global::InventorySystem.Items.Usables.Scp244.Hypothermia.HypothermiaSubEffectBase hypothermiaSubEffectBase in subEffects)
            {
                flag |= hypothermiaSubEffectBase.IsActive;
                UpdateSubEffect(hypothermiaSubEffectBase, _curExposure);
            }
            if (global::Mirror.NetworkServer.active)
            {
                float a = (flag ? (1f + _curExposure / 0.1f) : 0f);
                ServerSetState((byte)global::UnityEngine.Mathf.RoundToInt(global::UnityEngine.Mathf.Min(a, 255f)));
            }
        }
        private void UpdateSubEffect(global::InventorySystem.Items.Usables.Scp244.Hypothermia.HypothermiaSubEffectBase subEffect, float curExposure)
        {
            subEffect.UpdateEffect(curExposure);
            if (subEffect is global::CustomPlayerEffects.IWeaponModifierPlayerEffect weaponModifierPlayerEffect)
            {
                ParamsActive |= weaponModifierPlayerEffect.ParamsActive;
                _weaponModifier = weaponModifierPlayerEffect;
            }
            if (subEffect is global::CustomPlayerEffects.ISoundtrackMutingEffect soundtrackMutingEffect)
            {
                MuteSoundtrack |= soundtrackMutingEffect.MuteSoundtrack;
            }
            if (subEffect is global::PlayerRoles.FirstPersonControl.IMovementSpeedModifier movementSpeedModifier)
            {
                MovementSpeedLimit = global::UnityEngine.Mathf.Min(MovementSpeedLimit, movementSpeedModifier.MovementSpeedLimit);
                MovementSpeedMultiplier *= movementSpeedModifier.MovementSpeedMultiplier;
            }
        }

        private void UpdateExposure()
        {
            foreach (global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup instance in global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup.Instances)
            {
                _curExposure += instance.FogPercentForPoint(base.Hub.PlayerCameraReference.position);
            }
        }

        public bool TryGetWeaponParam(global::InventorySystem.Items.Firearms.Attachments.AttachmentParam param, out float val)
        {
            return _weaponModifier.TryGetWeaponParam(param, out val);
        }
    }
}
