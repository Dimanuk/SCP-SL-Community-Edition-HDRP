using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace InventorySystem.Items.Thirdperson
{
	public class IdleThirdpersonItem : ThirdpersonItemBase
	{
        [global::UnityEngine.SerializeField]
        private global::UnityEngine.AnimationClip _idleOverride;

        internal override void Initialize(global::PlayerRoles.FirstPersonControl.Thirdperson.HumanCharacterModel model, global::InventorySystem.Items.ItemIdentifier id)
        {
            base.Initialize(model, id);
            if (global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationManager.TryGetDefaultAnimation(model, global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.Override0, out global::UnityEngine.AnimationClip clip))
            {
                model.AnimatorOverride[clip] = _idleOverride;
            }
            model.Animator.SetFloat(global::InventorySystem.Items.Thirdperson.ThirdpersonItemBase.HashOverrideBlend, 0f);
        }
	}
}
