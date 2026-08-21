using System.Diagnostics;
using AudioPooling;
using CustomPlayerEffects;
using Mirror;
using PlayerRoles.PlayableScps.HumeShield;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp244.Hypothermia
{
    public class HumeShieldSubEffect : HypothermiaSubEffectBase, IHumeShieldBlocker
    {
        public struct HumeBlockMsg : NetworkMessage
        {
        }

        [SerializeField]
        private AudioClip[] _freezeSounds;

        [SerializeField]
        private float _hsSustainTime;

        [SerializeField]
        private float _hsDecreaseStartTime;

        [SerializeField]
        private float _hsDecreaseAbsolute;

        [SerializeField]
        private float _hsDecreasePerExposure;

        private float _decreaseTimer;

        private static HumeShieldSubEffect _localEffect;

        private readonly Stopwatch _cooldownSw = Stopwatch.StartNew();
        private readonly Stopwatch _sustainSw = new Stopwatch();

        public override bool IsActive
        {
            get
            {
                if (HumeShieldBlocked)
                    return true;

                if (_sustainSw.IsRunning)
                    return _sustainSw.Elapsed.TotalSeconds < (double)_hsSustainTime;

                return false;
            }
        }

        public bool HumeShieldBlocked { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        private static void Register()
        {
            CustomNetworkManager.OnClientReady += () =>
            {
                NetworkClient.ReplaceHandler<HumeBlockMsg>(OnMessageReceived, true);
            };
        }

        private static void OnMessageReceived(HumeBlockMsg msg)
        {
            if (_localEffect != null)
                _localEffect.ReceiveHumeBlockMessage();
        }

        internal override void Init(StatusEffectBase mainEffect)
        {
            base.Init(mainEffect);
            if (mainEffect.IsLocalPlayer)
                _localEffect = this;
        }

        internal override void UpdateEffect(float curExposure)
        {
            if (!NetworkServer.active)
            {
                if (HumeShieldBlocked && curExposure <= 0f)
                    HumeShieldBlocked = false;
                return;
            }

            bool wasBlocked = HumeShieldBlocked;
            HumeShieldBlocked = UpdateHumeShield(curExposure);

            if (HumeShieldBlocked)
            {
                if (!wasBlocked)
                {
                    Hub.networkIdentity.connectionToClient.Send(new HumeBlockMsg());
                }
            }
            else
            {
                _decreaseTimer = 0f;
            }
        }

        private bool UpdateHumeShield(float expo)
        {
            if (expo == 0f || !TryGetController(out var ctrl) || Hub.characterClassManager.GodMode)
                return false;

            ctrl.AddBlocker(this);
            _sustainSw.Restart();

            _decreaseTimer += expo * Time.deltaTime;

            if (_decreaseTimer < _hsDecreaseStartTime)
                return true;

            ctrl.HsCurrent -= (expo * _hsDecreasePerExposure + _hsDecreaseAbsolute) * Time.deltaTime;
            return true;
        }

        private void ReceiveHumeBlockMessage()
        {
            if (!TryGetController(out var ctrl))
                return;

            HumeShieldBlocked = true;
            ctrl.AddBlocker(this);

            if (_cooldownSw.Elapsed.TotalSeconds >= (double)_hsSustainTime)
            {
                if (_freezeSounds != null && _freezeSounds.Length > 0)
                {
                    var clip = RandomElement.RandomItem(_freezeSounds);
                    AudioSourcePoolManager.PlaySound(clip, Vector3.zero, 1f, 0f, 0f);
                }
                _cooldownSw.Restart();
            }
        }

        private bool TryGetController(out DynamicHumeShieldController ctrl)
        {
            ctrl = null;

            if (Hub.roleManager.CurrentRole is not IHumeShieldedRole humeShieldedRole)
                return false;

            if (humeShieldedRole.HumeShieldModule is not DynamicHumeShieldController dynamicCtrl)
                return false;

            ctrl = dynamicCtrl;
            return true;
        }
    }
}