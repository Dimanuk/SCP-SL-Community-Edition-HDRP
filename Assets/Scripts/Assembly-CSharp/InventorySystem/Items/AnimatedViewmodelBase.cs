using System;
using InventorySystem.Items.SwayControllers;
using UnityEngine;

namespace InventorySystem.Items
{
    public abstract class AnimatedViewmodelBase : ItemViewmodelBase
    {
        [SerializeField]
        private Animator _animator;

        public bool DisableSharedHands;

        private const float MaxSkipEquipTime = 7.5f;

        public static event Action OnSwayUpdated;

        public Animator ViewmodelAnimator => _animator;

        public Avatar AnimatorAvatar => _animator != null ? _animator.avatar : null;

        public RuntimeAnimatorController AnimatorRuntimeController => _animator != null ? _animator.runtimeAnimatorController : null;

        public Transform AnimatorTransform => _animator != null ? _animator.transform : null;

        public bool IsFastForwarding { get; private set; }

        public abstract IItemSwayController SwayController { get; }

        protected float SkipEquipTime
        {
            get
            {
                if (Hub == null)
                    return 0f;

                return Mathf.Min(Hub.inventory.LastItemSwitch, MaxSkipEquipTime);
            }
        }

        protected virtual void LateUpdate()
        {
            SwayController?.UpdateSway();
            AnimatedViewmodelBase.OnSwayUpdated?.Invoke();
        }

        public override void InitAny()
        {
            base.InitAny();
            if (_animator == null)
                _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError($"[AnimatedViewmodelBase] No Animator found on '{gameObject.name}' ({GetType().Name}) — first-person animations will not work!");
                return;
            }
            _animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            AnimatorForceUpdate(Time.deltaTime, true);
        }

        public AnimatorStateInfo GetAnimatorStateInfo(int layer)
        {
            if (_animator == null) return default;
            return _animator.GetCurrentAnimatorStateInfo(layer);
        }

        public void AnimatorForceUpdate()
        {
            AnimatorForceUpdate(Time.deltaTime, true);
        }

        public virtual void AnimatorForceUpdate(float deltaTime, bool fastMode = true)
        {
            if (_animator == null || !_animator.gameObject.activeInHierarchy)
                return;

            if (fastMode)
            {
                IsFastForwarding = true;
                _animator.Update(deltaTime);
                SharedHandsController.Singleton?.Hands?.Update(deltaTime);
                IsFastForwarding = false;
            }
            else
            {
                while (deltaTime > 0f)
                {
                    AnimatorForceUpdate(Mathf.Min(deltaTime, 0.07f));
                    deltaTime -= 0.07f;
                }
            }
        }

        public void AnimatorSetBool(int hash, bool val)
        {
            if (_animator == null) return;
            _animator.SetBool(hash, val);
            if (!DisableSharedHands && SharedHandsController.Singleton != null)
                SharedHandsController.Singleton.Hands.SetBool(hash, val);
        }

        public void AnimatorSetFloat(int hash, float val)
        {
            if (_animator == null) return;
            _animator.SetFloat(hash, val);
            if (!DisableSharedHands && SharedHandsController.Singleton != null)
                SharedHandsController.Singleton.Hands.SetFloat(hash, val);
        }

        public void AnimatorSetInt(int hash, int val)
        {
            if (_animator == null) return;
            _animator.SetInteger(hash, val);
            if (!DisableSharedHands && SharedHandsController.Singleton != null)
                SharedHandsController.Singleton.Hands.SetInteger(hash, val);
        }

        public void AnimatorSetTrigger(int hash)
        {
            if (_animator == null) return;
            _animator.SetTrigger(hash);
            if (!DisableSharedHands && SharedHandsController.Singleton != null)
                SharedHandsController.Singleton.Hands.SetTrigger(hash);
        }

        public void AnimatorSetLayerWeight(int layer, float val)
        {
            if (_animator == null) return;
            _animator.SetLayerWeight(layer, val);
            if (!DisableSharedHands && SharedHandsController.Singleton != null)
                SharedHandsController.Singleton.Hands.SetLayerWeight(layer, val);
        }

        public void AnimatorPlay(int hash, int layer = 0, float time = 0f)
        {
            if (_animator == null) return;
            _animator.Play(hash, layer, time);
            if (!DisableSharedHands && SharedHandsController.Singleton != null)
                SharedHandsController.Singleton.Hands.Play(hash, layer, time);
        }

        public void AnimatorRebind()
        {
            if (_animator == null) return;
            _animator.Rebind();
            if (!DisableSharedHands && SharedHandsController.Singleton != null)
                SharedHandsController.Singleton.Hands.Rebind();
        }
    }
}