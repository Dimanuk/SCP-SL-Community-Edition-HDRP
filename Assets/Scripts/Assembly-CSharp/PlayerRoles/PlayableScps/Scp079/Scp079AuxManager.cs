using System.Text;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079AuxManager : ScpStandardSubroutine<Scp079Role>, IScp079LevelUpNotifier
    {
        [SerializeField]
        private float[] _regenerationPerTier;

        [SerializeField]
        public float[] _maxPerTier;

        public Scp079TierManager _tierManager;
        private Scp079AbilityBase[] _abilities;
        private int _abilitiesCount;
        private float _aux;
        private bool _valueDirty;
        private ushort _prevSent;

        private static string _textEtaFormat;
        public static string _textHigherTierRequired;
        private static string _textNewMaxAux;

        private ushort Compressed => (ushort)Mathf.Min(CurrentAuxFloored, 65535);

        public float RegenSpeed
        {
            get
            {
                float num = _regenerationPerTier[_tierManager.AccessTierIndex];
                for (int i = 0; i < _abilitiesCount; i++)
                    num *= _abilities[i].AuxRegenMultiplier;
                return num;
            }
        }

        public float CurrentAux
        {
            get => _aux;
            set
            {
                value = Mathf.Clamp(value, 0f, MaxAux);
                if (value != _aux)
                {
                    _aux = value;
                    _valueDirty = true;
                }
            }
        }

        public int CurrentAuxFloored => Mathf.FloorToInt(CurrentAux);

        public float MaxAux => _maxPerTier[_tierManager.AccessTierIndex];

        private void Start()
        {
            _textEtaFormat = Translations.Get(Scp079HudTranslation.EtaTimer);
            _textHigherTierRequired = Translations.Get(Scp079HudTranslation.HigherTierRequired);
            _textNewMaxAux = Translations.Get(Scp079HudTranslation.AuxPowerLimitIncreased);
        }

        private void Update()
        {
            if (NetworkServer.active)
            {
                Regenerate();
                SyncValues();
            }
        }

        private void Regenerate()
        {
            CurrentAux += Time.deltaTime * RegenSpeed;
        }

        private void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prev, PlayerRoleBase cur)
        {
            if (NetworkServer.active && cur is SpectatorRole)
                ServerSendRpc(hub);
        }

        private void SyncValues()
        {
            if (!_valueDirty)
                return;

            ushort compressed = Compressed;
            _valueDirty = false;

            if (compressed != _prevSent)
            {
                _prevSent = compressed;
                ServerSendRpc(x => x == base.Owner || SpectatorNetworking.IsSpectatedBy(base.Owner, x));
            }
        }

        public string GenerateETA(float requiredAux)
        {
            if (requiredAux > MaxAux)
                return _textHigherTierRequired;

            float regenSpeed = RegenSpeed;
            if (regenSpeed <= 0f)
                return string.Empty;

            float num = Mathf.Max(0f, requiredAux - CurrentAux);
            return GenerateCustomETA(Mathf.CeilToInt(num / regenSpeed));
        }

        public string GenerateCustomETA(int secondsRemaining)
        {
            return string.Format(_textEtaFormat, secondsRemaining);
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            PlayerRoleManager.OnRoleChanged += OnRoleChanged;

            SubroutineManagerModule subroutineModule = base.ScpRole.SubroutineModule;
            subroutineModule.TryGetSubroutine(out _tierManager);

            int num = subroutineModule.AllSubroutines.Length;
            _abilities = new Scp079AbilityBase[num];
            _abilitiesCount = 0;

            for (int i = 0; i < num; i++)
            {
                if (subroutineModule.AllSubroutines[i] is Scp079AbilityBase ability)
                {
                    _abilities[_abilitiesCount] = ability;
                    _abilitiesCount++;
                }
            }

            CurrentAux = _maxPerTier[0];
        }

        public override void ResetObject()
        {
            base.ResetObject();
            PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            NetworkWriterExtensions.WriteUShort(writer, _prevSent);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            if (!NetworkServer.active)
                CurrentAux = NetworkReaderExtensions.ReadUShort(reader);
        }

        public bool WriteLevelUpNotification(StringBuilder sb, int newLevel)
        {
            sb.AppendFormat(_textNewMaxAux, _maxPerTier[newLevel]);
            return true;
        }
    }
}