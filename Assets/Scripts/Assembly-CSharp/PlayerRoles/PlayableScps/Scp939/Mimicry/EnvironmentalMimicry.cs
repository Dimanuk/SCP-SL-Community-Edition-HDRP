using System;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{
    public class EnvironmentalMimicry : ScpStandardSubroutine<Scp939Role>
    {
        [Serializable]
        public struct EnvMimicryCategory
        {
            public Scp939HudTranslation Name;
            public EnvMimicryOption[] Options;
        }

        private byte _syncCat;
        private byte _syncSound;
        private MimicPointController _mimicPoint;

        [SerializeField] private float _activationCooldown;

        public readonly AbilityCooldown Cooldown = new AbilityCooldown();

        [field: SerializeField]
        public EnvMimicryCategory[] Categories { get; private set; }

        public string CooldownText
        {
            get
            {
                if (Cooldown.IsReady)
                    return string.Empty;

                float remaining = Mathf.RoundToInt(Cooldown.Remaining * 10f) / 10f;
                return string.Format(
                    Translations.Get(Scp939HudTranslation.EnvMimicryCooldown),
                    remaining.ToString("0.0")
                );
            }
        }

        public event Action OnSoundPlayed;

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _mimicPoint);
        }

        public override void ResetObject()
        {
            base.ResetObject();
            Cooldown.Clear();
        }

        public void ClientPlay(int category, int sound)
        {
            _syncCat = (byte)category;
            _syncSound = (byte)sound;
            ClientSendCmd();
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            writer.WriteByte(_syncCat);
            writer.WriteByte(_syncSound);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (!Cooldown.IsReady)
                return;

            _syncCat = reader.ReadByte();
            _syncSound = reader.ReadByte();
            Cooldown.Trigger(_activationCooldown);
            ServerSendRpc(toAll: true);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            writer.WriteByte(_syncCat);
            writer.WriteByte(_syncSound);
            Cooldown.WriteCooldown(writer);
            writer.WriteByte((byte)UnityEngine.Random.Range(0, 255));
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            _syncCat = (byte)(reader.ReadByte() % Categories.Length);
            _syncSound = (byte)(reader.ReadByte() % Categories[_syncCat].Options.Length);
            Cooldown.ReadCooldown(reader);

            Categories[_syncCat].Options[_syncSound].Play(reader.ReadByte());
            OnSoundPlayed?.Invoke();
        }

        private void Update()
        {
            foreach (var category in Categories)
            {
                foreach (var option in category.Options)
                    option.UpdateSequence(_mimicPoint);
            }
        }
    }
}