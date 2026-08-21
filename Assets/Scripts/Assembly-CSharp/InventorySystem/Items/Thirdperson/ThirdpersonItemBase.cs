using GameObjectPools;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace InventorySystem.Items.Thirdperson
{
    public abstract class ThirdpersonItemBase : PoolObject, IPoolResettable
    {
        private Transform _tr;

        protected static readonly int HashOverrideBlend = Animator.StringToHash("OverrideBlend");

        protected static readonly int HashPrimaryAdditiveBlend = Animator.StringToHash("PrimaryAdditiveBlend");

        protected static readonly int HashSecondaryAdditiveBlend = Animator.StringToHash("SecondaryAdditiveBlend");

        public ItemIdentifier Identifier { get; private set; }

        public HumanCharacterModel TargetModel { get; private set; }
        public Animator Animator => TargetModel.Animator;
        public ReferenceHub OwnerHub => TargetModel.OwnerHub;
        public virtual float RotationOffset => 0f;

        public virtual void ResetObject()
        {
            TargetModel = null;
        }

        public virtual float GetTransitionTime(ItemIdentifier iid)
        {
            return 0.5f;
        }

        internal virtual void OnFadeChanged(float newFade)
        {
            _tr.localScale = Vector3.one * newFade;
        }

        internal virtual void Initialize(HumanCharacterModel model, ItemIdentifier id)
        {
            _tr = base.transform;
            _tr.parent = model.ItemSpawnpoint;
            _tr.localScale = Vector3.one;
            _tr.localPosition = Vector3.zero;
            _tr.localRotation = Quaternion.identity;
            TargetModel = model;
            Identifier = id;
            OnFadeChanged(model.Fade);
        }
    }
}
