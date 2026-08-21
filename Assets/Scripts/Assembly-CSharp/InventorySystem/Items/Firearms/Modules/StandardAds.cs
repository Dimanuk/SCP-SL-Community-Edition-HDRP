using AudioPooling;
using InventorySystem.GUI;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class StandardAds : IAdsModule, IFirearmModuleBase
    {
        private static readonly int AdsInAnimation;
        private static readonly int AdsOutAnimation;
        private static readonly int AdsCurrentHash;
        private static readonly Dictionary<ushort, bool> SyncAdsStates;

        private bool _prevState;      
        private bool _serverAds;      
        private bool _deAdsAnimation; 
        private bool _state;         

        private float _curAds;        
        private float _curAnimation; 
        private float _extraDeltaTime;

        private AudioSource _adsSoundSource;

        protected readonly Firearm Firearm;   
        private readonly ushort _serial;       
        private readonly float _defaultAdsTime; 
        private readonly int _adsLayer;        
        private readonly byte _adsInClip;      
        private readonly byte _adsOutClip;     

        public bool Standby
        {
            get
            {
                if (Firearm?.Owner != null && !Firearm.Owner.isLocalPlayer)
                    return true;

                float amount = ClientAdsAmount;
                if (amount == 0f || amount == 1f)
                    return true;
                return false;
            }
        }

        public bool ServerAds
        {
            get
            {
                if (!_serverAds) return false;
                if (Firearm.AmmoManagerModule?.Standby != true) return false;
                if (Firearm.EquipperModule?.Standby != true) return false;
                return AdsSpeed > 0f;
            }
            set => _serverAds = value;
        }

        public float ClientAdsAmount
        {
            get
            {
                float normalized = Mathf.Clamp01(_curAds);
                return Firearm.GlobalSettingsPreset.AdsAnimationCurve.Evaluate(normalized);
            }
        }

        public bool ClientAllowAds
        {
            get
            {
                if (!InventoryGuiController.ItemsSafeForInteraction)
                    return Firearm.IsSpectated;
                return true;
            }
        }

        private float AdsSpeed =>
            Mathf.Max(0f, (1f / _defaultAdsTime) * AttachmentsUtils.AttachmentsValue(Firearm, AttachmentParam.AdsSpeedMultiplier));

        protected virtual bool ForceDisabled =>
            !Firearm.EquipperModule.Standby || !Firearm.AmmoManagerModule.Standby;

        protected virtual bool AllowChange =>
            InventoryGuiController.ItemsSafeForInteraction || Firearm.IsSpectated;

        static StandardAds()
        {
            AdsInAnimation = Animator.StringToHash("ADS_in");
            AdsOutAnimation = Animator.StringToHash("ADS_out");
            AdsCurrentHash = Animator.StringToHash("AdsCurrent");
            SyncAdsStates = new Dictionary<ushort, bool> ();
        }

        public StandardAds(Firearm selfRef, ushort serial, float defaultAdsTime, int adsLayer, byte adsInClip, byte adsOutClip)
        {
            Firearm = selfRef;
            _serial = serial;
            _defaultAdsTime = defaultAdsTime;
            _adsLayer = adsLayer;
            _adsInClip = adsInClip;
            _adsOutClip = adsOutClip;

            selfRef.OnHolsteredCalled += ResetAds;

            if (selfRef.IsSpectated)
            {
                selfRef.OnEquipUpdateCalled += UpdateSpectator;
                _extraDeltaTime = selfRef.OwnerInventory.LastItemSwitch;
                UpdateSpectator();
            }
        }

        private void ResetAds()
        {
            _curAds = 0f;
            _serverAds = false;
        }

        private void UpdateSpectator()
        {
            // IL calls ClientUpdateAds unconditionally with (found && value). Skipping the call
            // when no sync entry exists leaves the ADS animator layer undriven, so it free-runs
            // its default state (ADS_in) and spectators see the gun aimed until the owner re-ADSes.
            ClientUpdateAds(SyncAdsStates.TryGetValue(_serial, out bool adsState) && adsState);
        }

        public void ClientUpdateAds(bool newState)
        {
            // Determine final _state from ForceDisabled / AllowChange guards
            if (ForceDisabled)
            {
                _state = false;
            }
            else if (AllowChange)
            {
                if (newState != _state)
                {
                    // Play audio only at boundary positions and only when not catching up (spectator)
                    if (_extraDeltaTime == 0f)
                    {
                        if (_curAds == 0f && _adsInClip != 0)
                            PlayAdsClip(_adsInClip);
                        if (_curAds == 1f && _adsOutClip != 0)
                            PlayAdsClip(_adsOutClip);
                    }
                    _state = newState;
                }
            }

            // Everything below drives the animator and advances the ADS ramp — all of it is a
            // no-op without a viewmodel. Spectator instances construct their StandardAds module
            // (which fires this once from the ctor) before Firearm.ViewModel is assigned, so this
            // first call must bail out entirely rather than silently burning _curAds/_extraDeltaTime
            // via raw arithmetic with no AnimatorPlay to keep the animator layer in sync — otherwise
            // the layer only catches up (and visibly snaps) on the next real ADS toggle.
            var viewmodel = Firearm.ClientViewmodel;
            if (viewmodel == null)
                return;

            // Drive animation at the current _curAnimation position every frame
            if (!_deAdsAnimation)
            {
                // Play ADS_in animation at current position
                viewmodel.AnimatorPlay(AdsInAnimation, _adsLayer, _curAnimation);

                // Switch to de-ADS when animation completes or when state flipped while fully ADS'd
                if (_curAnimation == 1f || (!_state && _curAds == 1f))
                {
                    _curAnimation = 1f;
                    _deAdsAnimation = true;
                }
            }
            else
            {
                // Play ADS_out animation at mirrored position
                viewmodel.AnimatorPlay(AdsOutAnimation, _adsLayer, 1f - _curAnimation);

                // Switch back to ADS_in when animation completes or when state flipped while fully un-ADS'd
                if (_curAnimation == 0f || (_state && _curAds == 0f))
                {
                    _curAnimation = 0f;
                    _deAdsAnimation = false;
                }
            }

            // Advance _curAds and _curAnimation; positive speed when ADS-ing, negative when un-ADS-ing
            float adsSpeed = _state ? AdsSpeed : -AdsSpeed;

            float stateLength = 1f;
            var hands = SharedHandsController.Singleton != null ? SharedHandsController.Singleton.Hands : null;
            if (hands != null)
            {
                float length = hands.GetCurrentAnimatorStateInfo(_adsLayer).length;
                if (length > 0f)
                    stateLength = length;
            }

            if (adsSpeed == 0f)
            {
                _extraDeltaTime = 0f;
                _curAds = 0f;
                _curAnimation = 0f;
            }
            else
            {
                // Consume any accumulated catch-up time (used by spectators on equip)
                float totalDelta = Time.deltaTime + _extraDeltaTime;
                _extraDeltaTime = 0f;

                _curAds = Mathf.Clamp01(_curAds + totalDelta * adsSpeed);
                _curAnimation = Mathf.Clamp01(_curAnimation + totalDelta / stateLength * adsSpeed);
            }

            // Keep the animator blend float in sync
            viewmodel.AnimatorSetFloat(AdsCurrentHash, _curAds);

            // Send the network request only when the effective state actually changes
            if (_prevState != _state)
            {
                _prevState = _state;
                NetworkClient.Send(new RequestMessage
                {
                    Serial = _serial,
                    Request = _state ? RequestType.AdsIn : RequestType.AdsOut
                });
            }
        }

        private void PlayAdsClip(byte clipId)
        {
            if (_adsSoundSource != null)
                _adsSoundSource.Stop();

            if (Firearm.AudioClips == null || clipId >= Firearm.AudioClips.Length)
                return;

            FirearmAudioClip clip = Firearm.AudioClips[clipId];
            _adsSoundSource = AudioSourcePoolManager.PlaySound(
                clip.Sound,
                Firearm.transform.position,
                clip.MaxDistance,
                volume: 1f,
                falloffType: FalloffType.Exponential,
                channel: AudioMixerChannelType.Weapons);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            FirearmBasicMessagesHandler.OnClientConfirmationReceived += msg =>
            {
                if (msg.Request == RequestType.AdsIn)       
                    SyncAdsStates[msg.Serial] = true;
                else if (msg.Request == RequestType.AdsOut) 
                    SyncAdsStates[msg.Serial] = false;
            };
        }
    }
}
