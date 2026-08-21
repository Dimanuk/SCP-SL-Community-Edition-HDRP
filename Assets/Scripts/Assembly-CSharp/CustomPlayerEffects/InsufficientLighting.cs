using System;
using System.Runtime.CompilerServices;

using PlayerRoles;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace CustomPlayerEffects
{
	public class InsufficientLighting : StatusEffectBase, ICustomRADisplay
	{
		private bool _prevTarget;

		private float _limit;

		private const float LimitAdjustSpeed = 0.15f;

		private const float LightLerpSpeed = 5f;

        private global::PlayerRoles.PlayerRoleBase CurRole => base.Hub.roleManager.CurrentRole;

        public string DisplayName { get; }

		public bool CanBeDisplayed { get; }

		public static event Action<float> OnLimitChanged;

        public static global::UnityEngine.Color DefaultColor
        {
            get
            {
                if (!VeryHighPerformance.LightsOff)
                {
                    return global::UnityEngine.Color.black;
                }
                return VeryHighPerformance.VHColor;
            }
        }

        internal override void OnRoleChanged(global::PlayerRoles.PlayerRoleBase previousRole, global::PlayerRoles.PlayerRoleBase newRole)
        {
            base.OnRoleChanged(previousRole, newRole);
            _prevTarget = false;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            StaticUnityMethods.OnUpdate += AlwaysUpdate;
        }

        private void OnDestroy()
        {
            StaticUnityMethods.OnUpdate -= AlwaysUpdate;
        }

        private void AlwaysUpdate()
        {
            if (global::Mirror.NetworkServer.active)
            {
                UpdateServer();
            }

            if (base.Hub != null && base.Hub.isLocalPlayer)
            {
                UpdateLocal();
            }
        }

        private void UpdateServer()
        {
            bool flag = CurRole is global::PlayerRoles.IAmbientLightRole ambientLightRole && ambientLightRole.InsufficientLight;
            if (flag != _prevTarget)
            {
                base.Intensity = (byte)(flag ? 1u : 0u);
                _prevTarget = flag;
            }
        }

        private void UpdateLocal()
        {
            global::UnityEngine.Color targetAmbient;
            bool insufficient = false;

            if (CurRole is global::PlayerRoles.IAmbientLightRole ambientLightRole)
            {
                targetAmbient = ambientLightRole.AmbientLight;
                insufficient = base.Hub.IsHuman() && ambientLightRole.InsufficientLight;
            }
            else
            {
                targetAmbient = DefaultColor;
            }

            global::UnityEngine.RenderSettings.ambientLight = global::UnityEngine.Color.Lerp(
                global::UnityEngine.RenderSettings.ambientLight, targetAmbient,
                global::UnityEngine.Time.deltaTime * LightLerpSpeed);

            float targetLimit = insufficient ? -2f : 1f;
            float newLimit = global::UnityEngine.Mathf.Lerp(_limit, targetLimit, LimitAdjustSpeed);

            if (newLimit != _limit)
            {
                _limit = newLimit;
                OnLimitChanged?.Invoke(newLimit);
            }
        }
	}
}
