using PlayerRoles;
using PlayerRoles.PlayableScps.Scp939;
using PlayerRoles.Spectating;
using UnityEngine;

namespace CustomPlayerEffects
{
    public class AmnesiaVision : StatusEffectBase, ISoundtrackMutingEffect
    {
        private float _lastTime;

        [SerializeField]
        private AnimationCurve _whispersOverTime;

        [SerializeField]
        private float _soundtrackLerp;

        [SerializeField]
        private float _growlDetectionRangeSqr;

        [SerializeField]
        private AudioSource _whispersSource;

        [SerializeField]
        private AudioSource _growlSource;

        [SerializeField]
        private AudioSource _disableSource;

        public bool MuteSoundtrack => IsEnabled && _whispersSource.volume > 0f;

        public float LastActive => Time.timeSinceLevelLoad - _lastTime;

        public override void OnStopSpectating()
        {
            StopSources();
        }

        internal override void OnDeath(PlayerRoleBase previousRole)
        {
            base.OnDeath(previousRole);
            StopSources();
        }

        protected override void Disabled()
        {
            _growlSource.Stop();
            if (_whispersSource.volume > 0f)
                _disableSource.Play();
        }

        protected override void Update()
        {
            base.Update();
            if (!IsEnabled)
                _whispersSource.volume = 0f;
        }

        protected override void OnEffectUpdate()
        {
            if (!Hub.isLocalPlayer && !SpectatorNetworking.IsLocallySpectated(Hub))
                return;

            float targetVolume = _whispersOverTime.Evaluate(LastActive);
            _whispersSource.volume = Mathf.Lerp(_whispersSource.volume, targetVolume, _soundtrackLerp * Time.deltaTime);
        }

        protected override void Enabled()
        {
            _lastTime = Time.timeSinceLevelLoad;

            if (!Hub.isLocalPlayer || !(Hub.roleManager.CurrentRole is HumanRole humanRole))
                return;

            Vector3 ownPosition = humanRole.FpcModule.Position;

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (!(hub.roleManager.CurrentRole is Scp939Role scp939))
                    continue;

                if ((ownPosition - scp939.FpcModule.Position).sqrMagnitude > _growlDetectionRangeSqr)
                    continue;

                _growlSource.Play();
                return;
            }
        }

        private void StopSources()
        {
            _disableSource.Stop();
            _growlSource.Stop();
        }
    }
}
