using PlayerRoles.Spectating;
using UnityEngine;

namespace CustomPlayerEffects
{
    public class Concussed : StatusEffectBase, IHealablePlayerEffect
    {
        public float intensityIncreasePer90Degree = 0.4f;

        private Quaternion _prevRot;
        private DiminishingLerpVisuals _postProcessBehavior;

        public override bool AllowEnabling => !Vitality.CheckPlayer(Hub);

        protected override void OnAwake()
        {
            _postProcessBehavior = GetComponent<DiminishingLerpVisuals>();
        }

        protected override void Enabled()
        {
            _prevRot = transform.rotation;
        }

        protected override void OnEffectUpdate()
        {
            var hub = Hub;

            if (!hub.isLocalPlayer && !SpectatorNetworking.IsLocallySpectated(hub))
                return;

            Quaternion currentRot = transform.rotation;
            float angle = Quaternion.Angle(_prevRot, currentRot);

            float intensity = angle / 90f * intensityIncreasePer90Degree;
            intensity *= RainbowTaste.CurrentMultiplier(hub);

            _postProcessBehavior.Intensity = intensity;
            _prevRot = currentRot;
        }

        public bool IsHealable(ItemType it)
        {
            return it == ItemType.SCP500 || it == ItemType.Adrenaline || it == ItemType.Painkillers;
        }
    }
}
