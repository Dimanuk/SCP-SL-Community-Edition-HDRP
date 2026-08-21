using System.Collections.Generic;
using AudioPooling;
using GameObjectPools;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace PlayerRoles.PlayableScps.HumeShield
{
    public class DynamicHumeShieldController : HumeShieldModuleBase, IPoolSpawnable, IPoolResettable
    {
        public struct ShieldBreakMessage : NetworkMessage
        {
            public ReferenceHub Target;
        }

        private HealthStat _hp;
        private double _nextRegenTime;
        private int _blockersCount;

        private const float ShieldBreakSoundRange = 37f;
        private const AudioMixerChannelType ShieldBreakSoundChannel = AudioMixerChannelType.NoDucking;

        [SerializeField]
        private AudioClip _shieldBreakSound;

        public AnimationCurve ShieldOverHealth;
        public float RegenerationRate;
        public float RegenerationCooldown;

        private readonly HashSet<IHumeShieldBlocker> _blockers = new HashSet<IHumeShieldBlocker>();
        private bool IsBlocked
        {
            get
            {
                _blockersCount -= _blockers.RemoveWhere(static x =>
                    (x is UnityEngine.Object obj && obj == null) || x == null || !x.HumeShieldBlocked);
                return _blockersCount > 0;
            }
        }

        public override float HsMax => ShieldOverHealth.Evaluate(_hp.NormalizedValue);

        public override float HsRegeneration
        {
            get
            {
                if (!(NetworkTime.time > _nextRegenTime) || IsBlocked)
                    return 0f;
                return RegenerationRate;
            }
        }

        public override Color? HsWarningColor
        {
            get
            {
                if (!IsBlocked)
                    return null;
                return new Color(1f, 0f, 0f, 0.3f);
            }
        }


        public void AddBlocker(IHumeShieldBlocker blocker)
        {
            if (_blockers.Add(blocker))
                _blockersCount++;
        }

        public void ResumeRegen()
        {
            _nextRegenTime = 0.0;
        }

        public override void OnHsValueChanged(float prevValue, float newValue)
        {

            if (NetworkServer.active && newValue <= 0f && prevValue > 0f && _shieldBreakSound != null)
            {
                NetworkServer.SendToReady(new ShieldBreakMessage { Target = base.Owner });
            }
        }

        public override void SpawnObject()
        {

            base.SpawnObject();

            PlayerStats playerStats = base.Owner.playerStats;
            _hp = playerStats.GetModule<HealthStat>();
            playerStats.OnThisPlayerDamaged += OnDamaged;

            if (NetworkServer.active)
            {
                base.HsCurrent = ShieldOverHealth.Evaluate(1f);
            }
        }

        public void ResetObject()
        {
            if (base.Owner != null)
            {
                base.Owner.playerStats.OnThisPlayerDamaged -= OnDamaged;
            }

            _nextRegenTime = 0.0;
            _blockersCount = 0;
            _blockers.Clear();
        }

        private void OnDamaged(DamageHandlerBase dhb)
        {

            if (NetworkServer.active)
            {
                _nextRegenTime = NetworkTime.time + RegenerationCooldown;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += () =>
            {
                NetworkClient.ReplaceHandler<ShieldBreakMessage>(msg =>
                {
                    if (msg.Target == null)
                        return;

                    if (msg.Target.roleManager.CurrentRole is IHumeShieldedRole hsRole &&
                        hsRole.HumeShieldModule is DynamicHumeShieldController ctrl &&
                        ctrl._shieldBreakSound != null)
                    {
                        AudioSourcePoolManager.PlaySound(
                            ctrl._shieldBreakSound,
                            ctrl.transform,
                            ShieldBreakSoundRange,
                            1f,
                            FalloffType.Exponential,
                            ShieldBreakSoundChannel);
                    }
                });
            };
        }
    }
}
