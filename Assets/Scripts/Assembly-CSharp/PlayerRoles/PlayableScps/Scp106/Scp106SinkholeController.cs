using CursorManagement;
using GameObjectPools;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106SinkholeController : SubroutineBase, ICursorOverride, IPoolResettable, IPoolSpawnable
    {
        public const float DefaultAnimTime = 3.3f;

        private const float CooldownDuration = 20f;

        private const float MinDuration = 0.03f;

        private const float AudioFadeIntensity = 8f;

        private const float AudioFadeAbs = 0.07f;

        private bool _state;

        private float _toggleTime;

        private int _vigorAbilitiesCount;

        private float _duration = 3.3f;

        private Scp106VigorAbilityBase[] _vigorAbilities;

        [SerializeField]
        private AudioClip _emergeSound;

        [SerializeField]
        private AudioClip _submergeSound;

        [SerializeField]
        private AudioSource _toggleAudioSource;

        public readonly AbilityCooldown Cooldown = new AbilityCooldown();

        private float CurTime => Time.timeSinceLevelLoad;

        public CursorOverrideMode CursorOverride => CursorOverrideMode.NoOverride;

        public bool LockMovement
        {
            get
            {
                if (base.Role.IsLocalPlayer)
                    return IsDuringAnimation;
                return false;
            }
        }

        public float ElapsedToggle => CurTime - _toggleTime;

        public bool IsDuringAnimation => ElapsedToggle < TargetDuration;

        public bool IsHidden
        {
            get
            {
                if (State)
                    return !IsDuringAnimation;
                return false;
            }
        }

        public float NormalizedState
        {
            get
            {
                float num = ElapsedToggle / TargetDuration;
                if (!State)
                    num = 1f - num;
                return Mathf.Clamp01(num);
            }
        }

        public bool State
        {
            get => _state;
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    _toggleTime = CurTime;
                    _toggleAudioSource.PlayOneShot(value ? _submergeSound : _emergeSound);
                    
                    if (!value && NetworkServer.active)
                    {
                        Cooldown.Trigger(CooldownDuration);
                        ServerSendRpc(toAll: true);
                    }
                }
            }
        }

        public float TargetDuration
        {
            get => _duration;
            set => _duration = Mathf.Max(MinDuration, value);
        }

        public float SpeedMultiplier
        {
            get => DefaultAnimTime / TargetDuration;
            set => TargetDuration = value * DefaultAnimTime;
        }

        public void SpawnObject()
        {
            CursorManager.Register(this);
        }

        public void ResetObject()
        {
            CursorManager.Unregister(this);
            Cooldown.Clear();
            _state = false;
            _toggleTime = 0f;
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            Cooldown.WriteCooldown(writer);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            Cooldown.ReadCooldown(reader);
        }

        protected override void Awake()
        {
            base.Awake();
            
            SubroutineBase[] allSubroutines = (base.Role as Scp106Role).SubroutineModule.AllSubroutines;
            _vigorAbilities = new Scp106VigorAbilityBase[allSubroutines.Length];
            _vigorAbilitiesCount = 0;
            
            for (int i = 0; i < allSubroutines.Length; i++)
            {
                if (allSubroutines[i] is Scp106VigorAbilityBase ability)
                {
                    _vigorAbilities[_vigorAbilitiesCount++] = ability;
                }
            }
        }

        private void Update()
        {
            bool isSubmerged = false;
            bool forceHumanAnimations = false;
            
            for (int i = 0; i < _vigorAbilitiesCount; i++)
            {
                Scp106VigorAbilityBase ability = _vigorAbilities[i];
                isSubmerged |= ability.IsSubmerged;
                forceHumanAnimations |= ability.ForceHumanAnimations;
            }
            
            State = isSubmerged;
            _toggleAudioSource.volume = AudioFadeIntensity * (1f - NormalizedState) - AudioFadeAbs;
        }
    }
}