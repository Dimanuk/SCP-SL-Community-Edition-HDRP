using System;
using System.Collections.Generic;
using System.Diagnostics;
using AudioPooling;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Subroutines;
using RelativePositioning;
using UnityEngine;
using Utils.Networking;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{
    public class ZombieAudioPlayer : SubroutineBase
    {
        [Serializable]
        private class Scp0492Sound
        {
            public Scp0492SoundId Id;

            public float MaxDistance;

            [SerializeField]
            private AudioClip[] _targetClips;

            [SerializeField]
            public AudioMixerChannelType _channel;

            [SerializeField]
            public float _volume = 1f;

            [SerializeField]
            private float _rateLimit;

            private readonly Stopwatch _sw;

            public AudioClip Random => RandomElement.RandomItem(_targetClips);

            public Scp0492Sound()
            {
                _volume = 1f;
                _sw = Stopwatch.StartNew();
            }

            public bool CheckRateLimit()
            {
                if (_rateLimit <= 0f)
                    return true;

                if (_sw.Elapsed.TotalSeconds >= _rateLimit)
                {
                    _sw.Restart();
                    return true;
                }
                return false;
            }

            public void PlayLocal()
            {
                if (!CheckRateLimit())
                    return;

                AudioClip clip = Random;
                if (clip == null)
                    return;

                AudioSourcePoolManager.PlaySound(
                    clip, 
                    Vector3.zero, 
                    MaxDistance, 
                    _volume
                );
            }

            public void PlayThirdperson(NetworkReader reader)
            {
                if (!CheckRateLimit())
                    return;

                RelativePosition relativePos = RelativePositionSerialization.ReadRelativePosition(reader);
                AudioClip clip = Random;
                if (clip == null)
                    return;

                AudioSourcePoolManager.PlaySound(
                    clip,
                    relativePos.Position,
                    MaxDistance,
                    _volume
                );
            }
        }

        public enum Scp0492SoundId : byte
        {
            Growl = 0,
            AngryGrowl = 1,
            Attack = 2
        }

        private const float GrowlMaxCooldown = 7.5f;

        private const float GrowlMinCooldown = 11.25f;

        private static bool _soundsSerialized;

        private static readonly Dictionary<byte, Scp0492Sound> Sounds = new Dictionary<byte, Scp0492Sound>();

        public readonly AbilityCooldown GrowlTimer = new AbilityCooldown();

        [SerializeField]
        private Scp0492Sound[] _sounds;

        private ZombieBloodlustAbility _visionTracker;

        private byte _soundToSend;

        private Vector3 _lastPos;

        public void ServerGrowl()
        {
            GrowlTimer.Trigger(UnityEngine.Random.Range(GrowlMinCooldown, GrowlMaxCooldown));
            ServerSendSound(_visionTracker.LookingAtTarget ? Scp0492SoundId.AngryGrowl : Scp0492SoundId.Growl);
        }

        private void Update()
        {
            if (NetworkServer.active && GrowlTimer.IsReady)
            {
                ServerGrowl();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            if (base.Role is ZombieRole zombieRole)
            {
                zombieRole.SubroutineModule.TryGetSubroutine(out _visionTracker);
            }

            if (!_soundsSerialized)
            {
                foreach (Scp0492Sound sound in _sounds)
                {
                    Sounds.Add((byte)sound.Id, sound);
                }
                _soundsSerialized = true;
            }
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            
            if (!Sounds.TryGetValue(reader.ReadByte(), out Scp0492Sound sound))
                return;

            if (base.Role != null && base.Role.IsLocalPlayer)
            {
                if (sound.CheckRateLimit())
                {
                    sound.PlayLocal();
                }
            }
            else
            {
                if (sound.CheckRateLimit())
                {
                    RelativePosition relativePos = RelativePositionSerialization.ReadRelativePosition(reader);
                    
                    AudioClip clip = sound.Random;
                    if (clip != null)
                    {
                        AudioSourcePoolManager.PlaySound(
                            clip,
                            relativePos.Position,
                            sound.MaxDistance,
                            sound._volume
                        );
                    }
                }
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            writer.WriteByte(_soundToSend);
            RelativePositionSerialization.WriteRelativePosition(writer, new RelativePosition(_lastPos));
        }

        public void ServerSendSound(Scp0492SoundId soundId)
        {
            _soundToSend = (byte)soundId;
            
            if (base.Role is ZombieRole zombieRole)
            {
                _lastPos = zombieRole.FpcModule.Position;
            }

            if (Sounds.TryGetValue(_soundToSend, out Scp0492Sound sound))
            {
                float disSqr = sound.MaxDistance * sound.MaxDistance;
                
                ServerSendRpc((ReferenceHub x) =>
                {
                    if (x.roleManager.CurrentRole is IFpcRole fpcRole)
                    {
                        return (fpcRole.FpcModule.Position - _lastPos).sqrMagnitude <= disSqr;
                    }
                    return true;
                });
            }
        }
    }
}
