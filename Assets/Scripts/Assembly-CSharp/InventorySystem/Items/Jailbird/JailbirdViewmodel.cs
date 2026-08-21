using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Jailbird
{
    public class JailbirdViewmodel : StandardAnimatedViemodel
    {
        private static readonly Dictionary<ushort, double> LastUpdates = new Dictionary<ushort, double>();
        private static readonly Dictionary<ushort, JailbirdMessageType> LastRpcs = new Dictionary<ushort, JailbirdMessageType>();
        private static readonly HashSet<ushort> BrokenJailbirds = new HashSet<ushort>();

        private static readonly int BrokenHash;
        private static readonly int LeftHandHash;
        private static readonly int ChargeLoadHash;
        private static readonly int ChargingHash;
        private static readonly int SkipPickupHash;
        private static readonly int AttackTriggerHash;
        private static readonly int InspectTriggerHash;
        private static readonly int IdleTagHash;

        private const float FastModeThreshold = 1.5f;
        private const float InspectCooldown = 0.5f;

        private static bool _alreadyPickedUp;
        private static bool _anyCollectionModified;
        private static bool _wasLeftHand;

        [SerializeField] private JailbirdMaterialController _materialController;
        [SerializeField] private GameObject _particlesSmall;
        [SerializeField] private GameObject _particlesLarge;
        [SerializeField] private GameObject _particlesTrail;
        [SerializeField] private GameObject _particlesBroken;

        [SerializeField] private AudioClip _inspectSound;
        [SerializeField] private AudioClip _firstEquipSound;
        [SerializeField] private AudioClip _normalEquipSound;
        [SerializeField] private AudioClip _chargeLoadSound;
        [SerializeField] private AudioClip _chargingSound;
        [SerializeField] private AudioClip _swipeSoundLeft;
        [SerializeField] private AudioClip _swipeSoundRight;
        [SerializeField] private AudioClip _chargeHitSound;
        [SerializeField] private AudioClip _brokenSound;
        [SerializeField] private AudioSource _targetAudioSource;

        private double _nextInspect;
        private bool _wasCharging;

        static JailbirdViewmodel()
        {
            BrokenHash = Animator.StringToHash("Broken");
            LeftHandHash = Animator.StringToHash("LeftHand");
            ChargeLoadHash = Animator.StringToHash("ChargeLoad");
            ChargingHash = Animator.StringToHash("Charging");
            SkipPickupHash = Animator.StringToHash("AlreadyPickedUp");
            AttackTriggerHash = Animator.StringToHash("Attack");
            InspectTriggerHash = Animator.StringToHash("Inspect");
            IdleTagHash = Animator.StringToHash("Idle");
        }

        public override void InitAny()
        {
            base.InitAny();
            JailbirdItem.OnRpcReceived += RpcReceived;

            if (_materialController != null)
            {
                _materialController.SetSerial(ItemId.SerialNumber);
            }
        }

        internal override void OnEquipped()
        {
            base.OnEquipped();
            AnimatorSetBool(SkipPickupHash, _alreadyPickedUp);

            PlaySound(_alreadyPickedUp ? _normalEquipSound : _firstEquipSound, SkipEquipTime, true, 0f);
            _alreadyPickedUp = true;
        }

        public override void InitSpectator(ReferenceHub ply, ItemIdentifier id, bool wasEquipped)
        {
            base.InitSpectator(ply, id, wasEquipped);
            if (BrokenJailbirds.Contains(id.SerialNumber))
            {
                SetBroken();
            }

            AnimatorForceUpdate(SkipEquipTime, true);

            if (LastRpcs.TryGetValue(id.SerialNumber, out JailbirdMessageType lastRpc) &&
                LastUpdates.TryGetValue(id.SerialNumber, out double lastUpdateTime))
            {
                float delay = (float)(NetworkTime.time - lastUpdateTime);
                ProcessRpc(lastRpc, delay);

                // Catch the animator up to the elapsed time. Large gaps are stepped in
                // fast-mode chunks so a single ForceUpdate does not skip transitions.
                if (delay > FastModeThreshold)
                {
                    AnimatorForceUpdate(FastModeThreshold, true);
                    AnimatorForceUpdate(delay - FastModeThreshold, true);
                }
                else
                {
                    AnimatorForceUpdate(delay, false);
                }
            }
        }

        public override void InitLocal(ItemBase ib)
        {
            base.InitLocal(ib);
            if (ib is JailbirdItem jailbird)
            {
                jailbird.OnCmdSent += OnCmdSent;
            }
        }

        private void OnDestroy()
        {
            JailbirdItem.OnRpcReceived -= RpcReceived;
        }

        private void Update()
        {
            int tagHash = GetAnimatorStateInfo(0).tagHash;

            if (_particlesSmall == null) return;
            _particlesSmall.SetActive(tagHash == AttackTriggerHash);

            if (_particlesTrail == null) return;
            _particlesTrail.SetActive(tagHash == ChargingHash);

            if (_particlesLarge == null) return;
            _particlesLarge.SetActive(tagHash == ChargeLoadHash);
        }

        private void RpcReceived(ushort serial, JailbirdMessageType rpc)
        {
            if (serial == ItemId.SerialNumber)
            {
                ProcessRpc(rpc, 0f);
            }
        }

        private void ProcessRpc(JailbirdMessageType rpc, float delay)
        {
            AnimatorSetBool(ChargingHash, rpc == JailbirdMessageType.ChargeStarted);
            AnimatorSetBool(ChargeLoadHash, rpc == JailbirdMessageType.ChargeLoadTriggered);

            if (_wasCharging && rpc != JailbirdMessageType.ChargeStarted)
            {
                PlaySound(_chargeHitSound, delay, true, 0f);
            }

            switch (rpc)
            {
                case JailbirdMessageType.Broken:
                    SetBroken();
                    PlaySound(_brokenSound, delay, false, 0f);
                    break;

                case JailbirdMessageType.AttackPerformed:
                    // Only spectator viewmodels play the swing from the RPC; the local
                    // player already predicted it via OnCmdSent(AttackTriggered). Firing it
                    // here too would set a stray Attack trigger that hijacks the
                    // Any State -> swing transition and blocks charge -> chargeSwing -> idle
                    // (the charge "fly" pose would stick until the next click).
                    if (IsSpectator)
                    {
                        PlayAttackAnim(delay);
                    }
                    break;

                case JailbirdMessageType.ChargeLoadTriggered:
                    PlaySound(_chargeLoadSound, delay, true, 0f);
                    break;

                case JailbirdMessageType.ChargeFailed:
                    // Binary passes a null clip here: stop the charge-load sound.
                    PlaySound(null, delay, true, 0f);
                    break;

                case JailbirdMessageType.ChargeStarted:
                    PlaySound(_chargingSound, delay, true, 0f);
                    break;

                case JailbirdMessageType.Inspect:
                    if (GetAnimatorStateInfo(0).tagHash == IdleTagHash && _nextInspect <= NetworkTime.time)
                    {
                        AnimatorSetTrigger(InspectTriggerHash);
                        PlaySound(_inspectSound, delay, true, 0f);
                        _nextInspect = NetworkTime.time + InspectCooldown;
                    }
                    break;
            }

            _wasCharging = rpc == JailbirdMessageType.ChargeStarted;
        }

        private void PlaySound(AudioClip clip, float delay, bool stopPrev = true, float pitchRandom = 0f)
        {
            if (stopPrev)
            {
                if (_targetAudioSource == null) return;
                _targetAudioSource.Stop();
            }

            if (clip == null) return;

            int sampleOffset = 0;
            if (delay > 0f)
            {
                sampleOffset = Mathf.RoundToInt(AudioSettings.outputSampleRate * delay);
                if (sampleOffset > clip.samples) return;
            }

            if (_targetAudioSource == null) return;
            _targetAudioSource.PlayOneShot(clip);

            _targetAudioSource.pitch = 1f + Random.Range(-pitchRandom, pitchRandom);

            if (sampleOffset != 0)
            {
                _targetAudioSource.timeSamples = sampleOffset;
            }
        }

        private void SetBroken()
        {
            AnimatorSetBool(BrokenHash, true);
            if (_particlesBroken != null)
            {
                _particlesBroken.SetActive(true);
            }
        }

        private void OnCmdSent(JailbirdMessageType cmd)
        {
            // Matches the binary: only the melee trigger is predicted locally. The old
            // AttackPerformed prediction block (clearing Charging/_wasCharging early) was a
            // band-aid for the host-mode state race in JailbirdItem — with that fixed, the
            // server's AttackPerformed RPC arrives normally and ProcessRpc both exits the
            // charge pose and plays _chargeHitSound. Clearing _wasCharging here swallowed
            // that charge-hit sound for the attacker.
            if (cmd == JailbirdMessageType.AttackTriggered)
            {
                PlayAttackAnim(0f);
            }
        }

        private void PlayAttackAnim(float delay)
        {
            _wasLeftHand = !_wasLeftHand;
            AnimatorSetBool(LeftHandHash, _wasLeftHand);
            AnimatorSetTrigger(AttackTriggerHash);
            PlaySound(_wasLeftHand ? _swipeSoundLeft : _swipeSoundRight, delay, true, 0.05f);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                _alreadyPickedUp = false;
                if (_anyCollectionModified)
                {
                    LastRpcs.Clear();
                    LastUpdates.Clear();
                    BrokenJailbirds.Clear();
                    _anyCollectionModified = false;
                }
            };

            JailbirdItem.OnRpcReceived += delegate (ushort serial, JailbirdMessageType rpc)
            {
                _anyCollectionModified = true;
                LastRpcs[serial] = rpc;
                LastUpdates[serial] = NetworkTime.time;
                if (rpc == JailbirdMessageType.Broken)
                {
                    BrokenJailbirds.Add(serial);
                }
            };
        }
    }
}
