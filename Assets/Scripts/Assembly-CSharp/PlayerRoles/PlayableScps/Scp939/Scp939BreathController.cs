using System;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using UnityEngine;
using Mirror;

namespace PlayerRoles.PlayableScps.Scp939
{
    public class Scp939BreathController : ScpStandardSubroutine<Scp939Role>
    {
        [Serializable]
        private class IdleLoop939
        {
            [SerializeField]
            private AudioSource _thirdperson;

            [SerializeField]
            private AudioSource _firstperson;

            private bool _cacheSet;
            private bool _has3rd;
            private bool _has1st;
            private bool _local;

            public float CurVolume { get; private set; }

            public void SetVolume(bool isOn, float lerp)
            {
                SetVolume(isOn ? 1f : 0f, lerp);
            }

            public void SetOwner(bool isLocalPlayer)
            {
                _local = isLocalPlayer;
                SetVolume(CurVolume, 1f);
            }

            public void SetVolume(float vol, float lerp = 1f)
            {
                if (!_cacheSet)
                {
                    _has3rd = _thirdperson != null;
                    _has1st = _firstperson != null;
                    _cacheSet = true;
                }

                CurVolume = Mathf.Lerp(CurVolume, vol, lerp);

                if (_has3rd)
                    _thirdperson.volume = _local ? 0f : CurVolume;

                if (_has1st)
                    _firstperson.volume = _local ? CurVolume : 0f;
            }
        }

        [SerializeField]
        private float _exhaustionGainLerp;

        [SerializeField]
        private float _exhaustionDropLerp;

        [SerializeField]
        private float _exhaustionMuteLoopsThreshold;

        [SerializeField]
        private AnimationCurve _exhaustionVolume;

        [SerializeField]
        private float _breathLerp;

        [SerializeField]
        private IdleLoop939 _focusLoop;

        [SerializeField]
        private IdleLoop939 _breathLoop;

        [SerializeField]
        private IdleLoop939 _exhaustionLoop;

        private float _curExhaustion;
        private StaminaStat _stamina;
        private Scp939FocusAbility _focus;

        public override void SpawnObject()
        {
            base.SpawnObject();

            RefreshPerspective();

            ForEachLoop(x => x.SetVolume(0f, 1f));

            _stamina = Owner.playerStats.GetModule<StaminaStat>();
            SpectatorTargetTracker.OnTargetChanged = (Action)Delegate.Combine(
                SpectatorTargetTracker.OnTargetChanged,
                new Action(RefreshPerspective));

            if (NetworkServer.active)
            {
                _stamina.ChangeSyncMode(SyncedStatBase.SyncMode.Public);
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _curExhaustion = 0f;
            SpectatorTargetTracker.OnTargetChanged = (Action)Delegate.Remove(
                SpectatorTargetTracker.OnTargetChanged,
                new Action(RefreshPerspective));
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _focus);
        }

        private void RefreshPerspective()
        {
            bool isLocal = SpectatorNetworking.IsLocallySpectated(Owner) || Owner.isLocalPlayer;
            ForEachLoop(x => x.SetOwner(isLocal));
        }

        private void ForEachLoop(Action<IdleLoop939> action)
        {
            action(_focusLoop);
            action(_breathLoop);
            action(_exhaustionLoop);
        }

        private void Update()
        {
            float targetExhaustion = Mathf.Clamp01(1f - _stamina.CurValue);
            float lerpSpeed = (targetExhaustion > _curExhaustion) ? _exhaustionGainLerp : _exhaustionDropLerp;
            _curExhaustion = Mathf.Lerp(_curExhaustion, targetExhaustion, Time.deltaTime * lerpSpeed);

            _exhaustionLoop.SetVolume(_exhaustionVolume.Evaluate(_curExhaustion), 1f);

            bool isExhausted = _curExhaustion > _exhaustionMuteLoopsThreshold;

            bool isFocused = false;
            if (!isExhausted && _focus != null)
            {
                if (_focus.TargetState)
                {
                    isFocused = true;
                }
                else if (_focus != null && _focus.State == 1)
                {
                    isFocused = true;
                }
            }

            float breathLerp = Time.deltaTime * _breathLerp;

            _focusLoop.SetVolume(isFocused, breathLerp);
            _breathLoop.SetVolume(!isFocused, breathLerp);
        }
    }
}