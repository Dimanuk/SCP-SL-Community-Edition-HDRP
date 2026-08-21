using System;
using System.Collections.Generic;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;
using Utils.NonAllocLINQ;
using AudioPooling;

namespace PlayerRoles.PlayableScps.Scp096
{
    public class Scp096AudioPlayer : ScpStandardSubroutine<Scp096Role>
    {
        [Serializable]
        public class Scp096StateAudio
        {
            public AudioClip Audio;
            public Scp096RageState State;
            public FalloffType Falloff;
            public float MaxDistance;
        }

        [SerializeField]
        private AudioSource _rageStatesSource;

        [SerializeField]
        private AudioSource _tryNotToCrySource;

        [SerializeField]
        private float _volumeAdjustLerp;

        [SerializeField]
        private CurvePreset[] _curves;

        [SerializeField]
        private Scp096StateAudio[] _rageStatesAudioClips;

        [SerializeField]
        private AudioClip[] _lethalClips;

        [SerializeField]
        private AudioClip[] _nonLethalClips;

        [SerializeField]
        private float _lethalDistance;

        [SerializeField]
        private float _nonLethalDistance;

        [SerializeField]
        private float _pitchRandomization;

        private static bool _soundsDictionarized;

        private Scp096HitResult _syncHitSound;

        private static readonly Dictionary<Scp096RageState, Scp096StateAudio> AudioStates 
            = new Dictionary<Scp096RageState, Scp096StateAudio>();

        private static readonly Dictionary<FalloffType, CurvePreset> Curves 
            = new Dictionary<FalloffType, CurvePreset>();

        private void Update()
        {
            bool tryingNotToCry = base.ScpRole.IsAbilityState(Scp096AbilityState.TryingNotToCry);
            float targetVolume = tryingNotToCry ? 1f : 0f;
            float lerpSpeed = Time.deltaTime * _volumeAdjustLerp;
            
            float volume = Mathf.Lerp(_tryNotToCrySource.volume, targetVolume, lerpSpeed);
            _tryNotToCrySource.volume = volume;
            _rageStatesSource.volume = 1f - volume;
        }

        protected override void Awake()
        {
            base.Awake();
            base.ScpRole.StateController.OnRageUpdate += SetAudioState;

            if (!_soundsDictionarized)
            {
                DictionaryExtensions.FromArray(Curves, _curves, x => x.Type);
                DictionaryExtensions.FromArray(AudioStates, _rageStatesAudioClips, x => x.State);
                _soundsDictionarized = true;
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            SetAudioState(base.ScpRole.StateController.RageState);
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _tryNotToCrySource.volume = 0f;
            _rageStatesSource.volume = 0f;
        }

        public void Play(AudioClip clip, FalloffType falloff = FalloffType.Linear, float maxDistance = -1f)
        {
            if (Curves.TryGetValue(falloff, out var curvePreset))
            {
                _rageStatesSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curvePreset.FalloffCurve);

                if (maxDistance > 0f)
                {
                    _rageStatesSource.maxDistance = maxDistance;
                }

                _rageStatesSource.clip = clip;
                _rageStatesSource.Play();
            }
        }

        public void SetAudioState(Scp096RageState state)
        {
            if (AudioStates.TryGetValue(state, out var stateAudio) && _rageStatesSource.clip != stateAudio.Audio)
            {
                Play(stateAudio.Audio, stateAudio.Falloff, stateAudio.MaxDistance);
            }
        }

        public void Stop()
        {
            if (_rageStatesSource.isPlaying)
            {
                _rageStatesSource.Stop();
            }

            _rageStatesSource.clip = null;
        }

        public void ServerPlayAttack(Scp096HitResult hitRes)
        {
            _syncHitSound = hitRes;
            ServerSendRpc(toAll: true);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            writer.WriteByte((byte)_syncHitSound);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            Scp096HitResult hitResult = (Scp096HitResult)reader.ReadByte();

            if ((hitResult & Scp096HitResult.Human) != Scp096HitResult.None)
            {
                bool lethal = (hitResult & Scp096HitResult.Lethal) == Scp096HitResult.Lethal;
                
                float maxDistance = lethal ? _lethalDistance : _nonLethalDistance;
                AudioClip[] clips = lethal ? _lethalClips : _nonLethalClips;
                AudioMixerChannelType channel = lethal ? AudioMixerChannelType.NoDucking : AudioMixerChannelType.DefaultSfx;
                
                float pitch = UnityEngine.Random.Range(1f - _pitchRandomization, 1f + _pitchRandomization);
                float spatial = base.Owner.isLocalPlayer ? 0f : 1f;

                AudioSourcePoolManager.PlaySound(
                    clips.RandomItem(), 
                    base.transform, 
                    maxDistance, 
                    1f, 
                    FalloffType.Exponential, 
                    channel, 
                    spatial).pitch = pitch;
            }
        }
    }
}