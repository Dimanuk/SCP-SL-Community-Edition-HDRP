using InventorySystem;
using InventorySystem.Disarming;
using InventorySystem.Items;
using InventorySystem.Items.Thirdperson;
using System.Diagnostics;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl.Thirdperson
{
    public class HumanCharacterModel : AnimatedCharacterModel
    {
        private static readonly int HashCuffed = Animator.StringToHash("Cuffed");

        private static readonly int HashGrounded = Animator.StringToHash("Grounded");

        private static readonly int HashHeadTilt = Animator.StringToHash("HeadTilt");

        private readonly Stopwatch _itemTransitionSw = Stopwatch.StartNew();

        private const float DefaultTransitionTime = 0.5f;

        private const float SpawnGroundedLock = 0.3f;

        private ItemIdentifier _prevItem;

        private AnimationCurve _transitionAnimation;

        private float _prevTransitionWeight;

        private float _prevRotationOffset;

        private Quaternion _initialRotation;

        private bool _modelUpdated;

        private bool _hasThirdpersonItem;

        private bool _hasItemSpawnpoint;

        [SerializeField]
        private int[] _itemLayers;

        public bool PoolHeldItem;

        public Transform ItemSpawnpoint;

        public ThirdpersonItemBase LastItemInstance { get; private set; }

        public override float Fade
        {
            get
            {
                return base.Fade;
            }
            set
            {
                if (Fade != Mathf.Clamp01(value))
                {
                    base.Fade = value;
                }
            }
        }

        private void ResetThirdpersonItem()
        {
            ThirdpersonItemAnimationManager.ResetOverrides(this);
            if (_hasThirdpersonItem)
            {
                LastItemInstance.ReturnToPool();
                _hasThirdpersonItem = false;
            }
        }

        private void SetTransitionAnim(float targetTime)
        {
            _transitionAnimation = new AnimationCurve(new Keyframe(0f, _prevTransitionWeight), new Keyframe(targetTime / 2f, 0f), new Keyframe(targetTime, 1f));
        }


        protected override void Awake()
        {
            base.Awake();
            _initialRotation = base.CachedTransform.localRotation;
            _hasItemSpawnpoint = ItemSpawnpoint != null;
            SetTransitionAnim(0.5f);
        }

        protected override void Update()
        {
            base.Update();
            if (!base.Pooled)
            {
                UpdateHeldItem(base.OwnerHub.inventory.CurItem);
                UpdateHeadTilt(base.OwnerHub.PlayerCameraReference);
                base.Animator.SetBool(HashCuffed, base.OwnerHub.inventory.IsDisarmed());
                base.Animator.SetBool(HashGrounded, base.FpcModule.Noclip.IsActive || base.FpcModule.IsGrounded || base.Role.ActiveTime < 0.3f);
            }
        }

        public override void SetVisibility(bool newState)
        {
            base.SetVisibility(newState);
            if (_hasItemSpawnpoint)
                ItemSpawnpoint.gameObject.SetActive(newState);
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _prevItem = ItemIdentifier.None;
            ThirdpersonItemAnimationManager.ResetOverrides(this);
        }

        public void UpdateHeldItem(ItemIdentifier item)
        {
            if (base.Pooled || !base.OwnerHub.isLocalPlayer)
            {
                if (_prevItem != item)
                {
                    _prevItem = item;
                    _modelUpdated = false;
                    _itemTransitionSw.Restart();
                    float transitionAnim = ((InventoryItemLoader.TryGetItem<ItemBase>(item.TypeId, out ItemBase result) && result.ThirdpersonModel != null) ? result.ThirdpersonModel.GetTransitionTime(item) : 0.5f);
                    SetTransitionAnim(transitionAnim);
                    _prevTransitionWeight = 1f;
                }
                double totalSeconds = _itemTransitionSw.Elapsed.TotalSeconds;
                float num = _transitionAnimation.Evaluate((float)totalSeconds);
                int[] itemLayers = _itemLayers;
                foreach (int layerIndex in itemLayers)
                {
                    base.Animator.SetLayerWeight(layerIndex, num);
                }
                if (!_modelUpdated && (num > _prevTransitionWeight || num == 0f))
                {
                    ResetThirdpersonItem();
                    _hasThirdpersonItem = ThirdpersonItemPoolManager.TryGet(this, item, out var result2, PoolHeldItem);
                    LastItemInstance = result2;
                    _modelUpdated = true;
                }
                float num2 = (_hasThirdpersonItem ? LastItemInstance.RotationOffset : 0f);
                if (num2 != _prevRotationOffset)
                {
                    base.CachedTransform.localRotation = _initialRotation * Quaternion.Euler(Vector3.up * num2);
                    _prevRotationOffset = num2;
                }
                _prevTransitionWeight = num;
                _prevItem = item;
            }
        }


        public void UpdateHeadTilt(Transform cam)
        {
            float x = cam.localRotation.eulerAngles.x;
            x = ((x > 180f) ? (x - 360f) : x);
            x = Mathf.InverseLerp(-88f, 88f, 0f - x) * 2f - 1f;
            base.Animator.SetFloat(HashHeadTilt, x);
        }
    }
}
