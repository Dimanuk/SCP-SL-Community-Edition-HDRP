using System;
using System.Collections.Generic;

using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.FunctionalParts;
using InventorySystem.Items.Thirdperson;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace InventorySystem.Items.Firearms
{
    public class FirearmThirdpersonItem : ThirdpersonItemBase
    {
        [Serializable]
        private struct AttachmentAnimOverride
        {
            public int AttachmentId;

            public ThirdpersonItemAnimationName AnimName;

            public AnimationClip Override;
        }

        [SerializeField]
        private FirearmWorldmodel _instance;

        [SerializeField]
        private AnimationClip _shootAnim;

        [SerializeField]
        private AnimationClip _reloadAnim;

        [SerializeField]
        private AnimationClip _hipAnim;

        [SerializeField]
        private AnimationClip _adsAnim;

        [SerializeField]
        private AnimationClip _lookForward;

        [SerializeField]
        private AnimationClip _lookDown;

        [SerializeField]
        private AnimationClip _lookUp;

        [SerializeField]
        private AnimationClip _corrLeft;

        [SerializeField]
        private AnimationClip _corrRight;

        [SerializeField]
        private AnimationClip _corrStraight;

        [SerializeField]
        private AnimationClip _corrBack;

        [SerializeField]
        private AnimationClip _corrCenter;

        [SerializeField]
        private AnimationCurve _rotatorDotCurve;

        [SerializeField]
        private AnimationCurve _rotatorDisCurve;

        [SerializeField]
        private AttachmentAnimOverride[] _attachmentOverrides;

        [SerializeField]
        private bool _isAdsing;

        [SerializeField]
        private bool _isReloading;

        private FirearmFlashlight _flashlight;

        private int _prevFlashlight;

        private float _rotOffset;

        private float _prevBlend;

        private bool _shotReceived;

        private Transform _hubTransform;

        private FirstPersonMovementModule _fpmm;

        private FirearmStatus _lastStatus;

        private const float DefaultEquipTime = 0.5f;

        private const float AdsTransitionSpeed = 4f;

        private const float ShootNormalizedTime = 0.1f;

        private const float OffsetAdjustSpeed = 40f;

        private const int ShootLayer = 3;

        private readonly Dictionary<Light, Color> _defaultColors = new Dictionary<Light, Color>();

        private static readonly int HashHeadTilt = global::UnityEngine.Animator.StringToHash("HeadTilt");

        private static readonly int HashShoot = global::UnityEngine.Animator.StringToHash("PrimaryAdditive");

        public override float RotationOffset => _rotOffset;

        public override float GetTransitionTime(global::InventorySystem.Items.ItemIdentifier iid)
        {
            if (!global::InventorySystem.InventoryItemLoader.TryGetItem<global::InventorySystem.Items.Firearms.Firearm>(iid.TypeId, out var result))
            {
                return 0.5f;
            }
            return global::UnityEngine.Mathf.Min(result.BaseStats.BaseDrawTime, 0.5f);
        }

        public override void ResetObject()
        {
            base.ResetObject();
            global::InventorySystem.Items.Firearms.FirearmAudioManager.OnAudioReceived -= AudioReceived;
            global::InventorySystem.Items.Firearms.BasicMessages.FirearmBasicMessagesHandler.OnClientConfirmationReceived -= ConfirmationReceived;
            _prevBlend = 0f;
            _isReloading = false;
            _isAdsing = false;
        }

        internal override void Initialize(global::PlayerRoles.FirstPersonControl.Thirdperson.HumanCharacterModel model, global::InventorySystem.Items.ItemIdentifier id)
        {
            base.Initialize(model, id);
            _rotOffset = 0f;
            if (!global::Mirror.NetworkClient.active && model.OwnerHub == null)
            {
                InitializeAnims();
                return;
            }
            _hubTransform = model.OwnerHub.transform;
            _fpmm = (model.OwnerHub.roleManager.CurrentRole as global::PlayerRoles.FirstPersonControl.IFpcRole).FpcModule;
            global::InventorySystem.Items.Firearms.FirearmAudioManager.OnAudioReceived += AudioReceived;
            global::InventorySystem.Items.Firearms.BasicMessages.FirearmBasicMessagesHandler.OnClientConfirmationReceived += ConfirmationReceived;
            InitializeAnims();
        }

        internal override void OnFadeChanged(float newFade)
        {
            base.OnFadeChanged(newFade);
            foreach (global::System.Collections.Generic.KeyValuePair<global::UnityEngine.Light, global::UnityEngine.Color> defaultColor in _defaultColors)
            {
                defaultColor.Key.color = global::UnityEngine.Color.Lerp(global::UnityEngine.Color.black, defaultColor.Value, newFade);
            }
        }

        private void InitializeAnims()
        {
            base.TargetModel.Animator.SetFloat(global::InventorySystem.Items.Thirdperson.ThirdpersonItemBase.HashOverrideBlend, 0f);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.PrimaryAdditive, _shootAnim);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.Override0, _reloadAnim);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.Override1, _hipAnim);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.Override2, _adsAnim);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.SecAdditive0, _lookDown);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.SecAdditive1, _lookForward);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.SecAdditive2, _lookUp);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.SprintLeftAdditive, _corrLeft);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.WalkLeftAdditive, _corrLeft);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.SprintRightAdditive, _corrRight);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.WalkRightAdditive, _corrRight);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.SprintStraightAdditive, _corrStraight);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.SprintBackAdditive, _corrBack);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.WalkStraightAdditive, _corrCenter);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.WalkBackAdditive, _corrCenter);
            SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.IdlePoseAdditive, _corrCenter);
            ApplyAttachmentOverrides();
        }

        private void SetAnim(global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName n, global::UnityEngine.AnimationClip clip)
        {
            global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationManager.SetAnimation(base.TargetModel, n, clip);
        }

        private void ApplyAttachmentOverrides()
        {
            global::InventorySystem.Items.Firearms.FirearmThirdpersonItem.AttachmentAnimOverride[] attachmentOverrides = _attachmentOverrides;
            for (int i = 0; i < attachmentOverrides.Length; i++)
            {
                global::InventorySystem.Items.Firearms.FirearmThirdpersonItem.AttachmentAnimOverride attachmentAnimOverride = attachmentOverrides[i];
                uint num = (uint)(1 << attachmentAnimOverride.AttachmentId);
                if ((_lastStatus.Attachments & num) == num)
                {
                    SetAnim(attachmentAnimOverride.AnimName, attachmentAnimOverride.Override);
                }
            }
        }

        private void AudioReceived(ReferenceHub hub, ItemType weapon, global::InventorySystem.Items.Firearms.FirearmAudioClip clip)
        {
            if (!hub.isLocalPlayer && !(hub != base.TargetModel.OwnerHub) && hub.inventory.CurItem.TypeId == weapon && clip.HasFlag(global::InventorySystem.Items.Firearms.FirearmAudioFlags.IsGunshot) && (!(base.TargetModel.Fade < 0f) || !(global::UnityEngine.Random.value > base.TargetModel.Fade)))
            {
                _shotReceived = true;
            }
        }

        private void ConfirmationReceived(global::InventorySystem.Items.Firearms.BasicMessages.RequestMessage msg)
        {
            if (msg.Serial == base.Identifier.SerialNumber)
            {
                switch (msg.Request)
                {
                    case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.AdsIn:
                        _isAdsing = true;
                        break;
                    case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.AdsOut:
                        _isAdsing = false;
                        break;
                    case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.Unload:
                    case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.Reload:
                        _isReloading = true;
                        break;
                    case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.ReloadStop:
                        _isReloading = false;
                        break;
                    case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.Dryfire:
                    case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.ToggleFlashlight:
                        break;
                }
            }
        }

        private void Awake()
        {
            global::UnityEngine.Light[] componentsInChildren = GetComponentsInChildren<global::UnityEngine.Light>(includeInactive: true);
            foreach (global::UnityEngine.Light light in componentsInChildren)
            {
                _defaultColors[light] = light.color;
            }
        }

        private void Update()
        {
            UpdateStatus();
            UpdateAnims();
        }

        private void UpdateStatus()
        {
            if (_instance == null || base.Identifier == null)
                return;

            if (global::InventorySystem.Items.Firearms.BasicMessages.FirearmBasicMessagesHandler.ReceivedStatuses == null)
                return;

            if (FirearmBasicMessagesHandler.ReceivedStatuses.TryGetValue(base.Identifier.SerialNumber, out _lastStatus)
                && _instance.ApplyStatus(_lastStatus, base.Identifier.TypeId))
            {
                ApplyAttachmentOverrides();
            }
        }
        private void UpdateAnims()
        {
            global::UnityEngine.Animator animator = base.TargetModel.Animator;
            if (global::Mirror.NetworkServer.active && base.TargetModel.OwnerHub.inventory.CurInstance is global::InventorySystem.Items.Firearms.Firearm firearm && firearm != null)
            {
                _isAdsing = firearm.AdsModule.ServerAds;
            }
            float target = ((!_isReloading) ? ((!_isAdsing) ? 1 : 2) : 0);
            _prevBlend = global::UnityEngine.Mathf.MoveTowards(_prevBlend, target, global::UnityEngine.Time.deltaTime * 4f);
            animator.SetFloat(global::InventorySystem.Items.Thirdperson.ThirdpersonItemBase.HashOverrideBlend, _prevBlend);
            animator.SetFloat(global::InventorySystem.Items.Thirdperson.ThirdpersonItemBase.HashSecondaryAdditiveBlend, animator.GetFloat(HashHeadTilt) + 1f);
            if (_shotReceived)
            {
                animator.Play(HashShoot, 3, 0.1f);
                _instance.PlayParticleEffects();
                _shotReceived = false;
            }
            if (ReferenceHub.TryGetLocalHub(out var hub) && hub.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole)
            {
                global::UnityEngine.Vector3 vector = fpcRole.FpcModule.Position - _fpmm.Position;
                float magnitude = vector.magnitude;
                float target2;
                if (magnitude > float.Epsilon)
                {
                    global::UnityEngine.Vector3 rhs = vector / magnitude;
                    float time = global::UnityEngine.Vector3.Dot(_hubTransform.forward, rhs);
                    target2 = _rotatorDisCurve.Evaluate(magnitude) * _rotatorDotCurve.Evaluate(time);
                }
                else
                {
                    target2 = 0f;
                }
                _rotOffset = global::UnityEngine.Mathf.MoveTowards(_rotOffset, target2, global::UnityEngine.Time.deltaTime * 40f);
            }
        }
    }
}
