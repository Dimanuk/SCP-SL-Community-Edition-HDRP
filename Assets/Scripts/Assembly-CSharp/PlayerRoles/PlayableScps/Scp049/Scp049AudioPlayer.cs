using System;
using System.Diagnostics;
using AudioPooling;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp049
{
    public class Scp049AudioPlayer : SubroutineBase
    {
        [Serializable]
        private class Scp049Sound
        {
            public SoundType Id;
            public float MaxDistance;

            [SerializeField]
            private AudioClip[] _targetClips;

            [SerializeField]
            private AudioMixerChannelType _channel;

            [SerializeField]
            private float _volume;

            [SerializeField]
            private float _rateLimit;

            private readonly Stopwatch _sw;


            private AudioClip Random => RandomElement.RandomItem(_targetClips);

            private bool CheckRateLimit()
            {
                if (_rateLimit <= 0f)
                    return true;

                if (_sw.Elapsed.TotalSeconds < 1f / _rateLimit)
                    return false;

                _sw.Restart();
                return true;
            }


            public void PlayLocal()
            {
                if (!CheckRateLimit())
                    return;

                AudioClip clip = Random;
                AudioSourcePoolManager.PlaySound(clip, Vector3.zero, 0f, _volume,default,_channel, 0, false);
            }
            public void PlayThirdperson(NetworkReader reader)
            {
                if (!CheckRateLimit())
                    return;

                RelativePosition relativePosition = RelativePositionSerialization.ReadRelativePosition(reader);
                AudioClip clip = Random;

                AudioSourcePoolManager.PlaySound(clip, relativePosition.Position, MaxDistance, _volume,default, _channel, 0, false);
            }

            public Scp049Sound()
            {
                _volume = 1f;
                _sw = Stopwatch.StartNew();
            }
        }

        public enum SoundType : byte
        {
            ChaseMusic = 0,
            Teleport = 1,
            Snap = 2
        }

        [SerializeField]
        private AudioClip _chaseMusic;

        [SerializeField]
        private float _volume;

        private Scp049SenseAbility _senseAbility;
        private byte _soundToSend;

        public void ServerSendSound(SoundType soundId)
        {
            _soundToSend = (byte)soundId;
            ServerSendRpc(false);

            if (soundId == SoundType.ChaseMusic && _senseAbility != null && _senseAbility.HasTarget)
            {
                ServerSendRpc(_senseAbility.Target);
            }
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            if (_chaseMusic != null)
            {
                AudioSourcePoolManager.PlaySound(_chaseMusic, Vector3.zero, 0f, 1f, 0, 0, 0f);
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            writer.WriteByte(_soundToSend);
        }
        protected override void Awake()
        {
            base.Awake();

            if (Role is Scp049Role scp049Role)
            {
                scp049Role.SubroutineModule.TryGetSubroutine(out _senseAbility);
            }
        }
    }
}