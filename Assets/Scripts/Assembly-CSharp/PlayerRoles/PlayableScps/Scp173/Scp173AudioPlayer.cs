using System;
using System.Collections.Generic;
using System.Diagnostics;
using AudioPooling;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp173;
using PlayerRoles.PlayableScps.Subroutines;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp173
{
    public class Scp173AudioPlayer : SubroutineBase
    {
        [Serializable]
        public class Scp173Sound
        {
            public Scp173SoundId Id;

            [SerializeField]
            private AudioClip[] _targetClips;

            [SerializeField]
            private AudioMixerChannelType _channel;

            [SerializeField]
            private float _volume = 1f;

            [SerializeField]
            private float _rateLimit;

            [SerializeField]
            public float _maxDistance;

            private readonly Stopwatch _sw = Stopwatch.StartNew();

            private AudioClip Random => _targetClips.RandomItem();

            public bool CheckRateLimit()
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

                AudioSourcePoolManager.PlaySound(
                    Random,
                    Vector3.zero,
                    _maxDistance,
                    _volume,
                    FalloffType.Exponential,
                    _channel,
                    0f,
                    false);
            }

            public void PlayThirdperson(NetworkReader reader)
            {
                if (!CheckRateLimit())
                    return;

                RelativePosition relPos = RelativePositionSerialization.ReadRelativePosition(reader);
                AudioSourcePoolManager.PlaySound(
                    Random,
                    relPos.Position,
                    _maxDistance,
                    _volume,
                    FalloffType.Exponential,
                    _channel,
                    1f,
                    false);
            }
        }

        public enum Scp173SoundId : byte
        {
            Hit = 0,
            Teleport = 1,
            Snap = 2
        }

        [SerializeField]
        private Scp173Sound[] _sounds;

        private byte _soundToSend;
        private Vector3 _lastPos;

        private static bool _soundsDictionarized;
        private static readonly Dictionary<byte, Scp173Sound> Sounds = new Dictionary<byte, Scp173Sound>();

        protected override void Awake()
        {
            base.Awake();

            if (!_soundsDictionarized)
            {
                foreach (Scp173Sound sound in _sounds)
                {
                    Sounds.Add((byte)sound.Id, sound);
                }
                _soundsDictionarized = true;
            }
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            byte soundId = reader.ReadByte();
            if (!Sounds.TryGetValue(soundId, out Scp173Sound sound))
                return;

            if (Role.IsLocalPlayer)
            {
                sound.PlayLocal();
            }
            else
            {
                sound.PlayThirdperson(reader);
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            writer.WriteByte(_soundToSend);
            RelativePositionSerialization.WriteRelativePosition(writer, new RelativePosition(_lastPos));
        }

        public void ServerSendSound(Scp173SoundId soundId)
        {
            _soundToSend = (byte)soundId;
            _lastPos = (Role as Scp173Role).FpcModule.Position;

            if (!Sounds.TryGetValue(_soundToSend, out Scp173Sound sound))
                return;

            float disSqr = sound._maxDistance * sound._maxDistance;

            var displayClass = new __c__DisplayClass8_0();
            displayClass.__4__this = this;
            displayClass.disSqr = disSqr;

            ServerSendRpc(displayClass.__ServerSendSound_b__0);
        }

        private class __c__DisplayClass8_0
        {
            public Scp173AudioPlayer __4__this;
            public float disSqr;

            internal bool __ServerSendSound_b__0(ReferenceHub x)
            {
                if (x.roleManager.CurrentRole is not IFpcRole fpcRole)
                    return true;

                return (fpcRole.FpcModule.Position - __4__this._lastPos).sqrMagnitude <= disSqr;
            }
        }
    }
}
