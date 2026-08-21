using InventorySystem.Items.Firearms.Attachments;
using Mirror;
using PlayerRoles.Spectating;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CustomPlayerEffects
{
    public class AmnesiaItems : StatusEffectBase, IUsableItemModifierEffect, IWeaponModifierPlayerEffect, IPulseEffect
    {
        private float _activeTime;

        [SerializeField]
        private ItemType[] _blockedUsableItems;

        [SerializeField]
        private float _blockDelay;

        [SerializeField]
        private float _pulseMax;

        [SerializeField]
        private float _vignetteLerp;

        [SerializeField]
        private float _pulseDrop;

        [SerializeField]
        private AnimationCurve _pulseMinOverTime;

        private Vignette _vignette;
        private float _pulseTarget;

        public bool ParamsActive => IsEnabled && _activeTime >= _blockDelay;

        protected override void OnAwake()
        {
            var volume = GetComponent<PostProcessVolume>();
            volume.profile.TryGetSettings(out _vignette);
        }

        protected override void OnEffectUpdate()
        {
            if (!Hub.isLocalPlayer && !SpectatorNetworking.IsLocallySpectated(Hub))
                return;

            _vignette.smoothness.value = Mathf.Lerp(_vignette.smoothness.value, _pulseTarget, _vignetteLerp * Time.deltaTime);
            _pulseTarget = Mathf.MoveTowards(_pulseTarget, _pulseMinOverTime.Evaluate(_activeTime), _pulseDrop * Time.deltaTime);
        }

        protected override void Update()
        {
            base.Update();
            if (IsEnabled)
                _activeTime += Time.deltaTime;
        }

        protected override void Enabled()
        {
            _activeTime = 0f;
        }

        public bool TryGetSpeed(ItemType item, out float speed)
        {
            speed = 0f;
            if (!NetworkServer.active || !_blockedUsableItems.Contains(item) || _activeTime < _blockDelay)
                return false;

            ServerSendPulse();
            return true;
        }

        public bool TryGetWeaponParam(AttachmentParam param, out float val)
        {
            val = 1f;
            if (!NetworkServer.active || param != AttachmentParam.PreventReload || _activeTime < _blockDelay)
                return false;

            ServerSendPulse();
            return true;
        }

        public void ExecutePulse()
        {
            _pulseTarget = _pulseMax;
        }

        private void ServerSendPulse()
        {
            Hub.playerEffectsController.ServerSendPulse<AmnesiaItems>();
        }
    }
}
