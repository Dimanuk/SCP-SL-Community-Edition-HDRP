using System.Collections.Generic;

using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace InventorySystem.Items.Thirdperson
{
	public static class ThirdpersonItemAnimationManager
	{
        private static readonly global::System.Collections.Generic.List<global::System.Collections.Generic.KeyValuePair<global::UnityEngine.AnimationClip, global::UnityEngine.AnimationClip>> OverridesPuller = new global::System.Collections.Generic.List<global::System.Collections.Generic.KeyValuePair<global::UnityEngine.AnimationClip, global::UnityEngine.AnimationClip>>();

        private static readonly global::System.Collections.Generic.Dictionary<global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName, global::UnityEngine.AnimationClip> CachedClips = new global::System.Collections.Generic.Dictionary<global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName, global::UnityEngine.AnimationClip>();

        public static bool TryGetDefaultAnimation(global::PlayerRoles.FirstPersonControl.Thirdperson.AnimatedCharacterModel target, global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName name, out global::UnityEngine.AnimationClip clip)
        {
            if (CachedClips.TryGetValue(name, out clip))
            {
                return true;
            }
            clip = null;
            OverridesPuller.Clear();
            target.AnimatorOverride.GetOverrides(OverridesPuller);
            foreach (global::System.Collections.Generic.KeyValuePair<global::UnityEngine.AnimationClip, global::UnityEngine.AnimationClip> item in OverridesPuller)
            {
                if (global::System.Enum.TryParse<global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName>(item.Key.name, out var result))
                {
                    if (result == name)
                    {
                        clip = item.Key;
                    }
                    CachedClips[result] = item.Key;
                }
            }
            return clip != null;
        }

        public static void ResetOverrides(global::PlayerRoles.FirstPersonControl.Thirdperson.AnimatedCharacterModel target)
        {
            OverridesPuller.Clear();
            target.AnimatorOverride.GetOverrides(OverridesPuller);
            for (int i = 0; i < OverridesPuller.Count; i++)
            {
                OverridesPuller[i] = new global::System.Collections.Generic.KeyValuePair<global::UnityEngine.AnimationClip, global::UnityEngine.AnimationClip>(OverridesPuller[i].Key, null);
            }
            target.AnimatorOverride.ApplyOverrides(OverridesPuller);
        }

        public static void SetAnimation(global::PlayerRoles.FirstPersonControl.Thirdperson.AnimatedCharacterModel target, global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName name, global::UnityEngine.AnimationClip clip)
        {
            if (TryGetDefaultAnimation(target, name, out var clip2))
            {
                target.AnimatorOverride[clip2] = clip;
            }
        }
	}
}
