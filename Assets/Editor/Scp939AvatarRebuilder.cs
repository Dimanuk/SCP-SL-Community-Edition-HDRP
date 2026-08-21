#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Scp939EditorTools
{
	// Rebuilds the SCP-939 humanoid avatar natively in the current Unity version.
	//
	// The shipped 939_RIGAvatar.asset was extracted from an older Unity build. Its bone
	// hierarchy/names are correct (verified against SCP-939 Model.prefab), but its serialized
	// muscle/DoF data crashes the Unity 6 native retargeter (Human2SkeletonPose / Hand2SkeletonPose /
	// SkeletonSetDoF) the moment 939 spawns and its Animator (CullingMode = AlwaysAnimate) retargets.
	//
	// This tool reconstructs the avatar with AvatarBuilder.BuildHumanAvatar from the live "Anims"
	// hierarchy + the original human-bone mapping, using DEFAULT muscle limits (so any corrupt
	// extracted limit data is discarded). The result is saved as a NEW asset and assigned to the
	// model + ragdoll prefabs; the original avatar is left untouched (reversible).
	public static class Scp939AvatarRebuilder
	{
		private const string ModelPrefabPath = "Assets/GameObject/SCP-939 Model.prefab";
		private const string RagdollPrefabPath = "Assets/GameObject/SCP-939_Ragdoll.prefab";
		private const string RebuiltAvatarPath = "Assets/Avatar/939_RIGAvatar_Rebuilt.asset";
		private const string AnimsRootName = "Anims";

		// humanName -> bone (transform) name, copied verbatim from 939_RIGAvatar.asset (m_HumanDescription.m_Human).
		// SCP-939 has 4 fingers (thumb/index/ring/little) — no middle — matching the original rig.
		private static readonly (string human, string bone)[] HumanMap =
		{
			("Hips", "HipControl"),
			("LeftUpperLeg", "DEF-Thigh.L"),
			("RightUpperLeg", "DEF-Thigh.R"),
			("LeftLowerLeg", "DEF-Leg.L"),
			("RightLowerLeg", "DEF-Leg.R"),
			("LeftFoot", "DEF-Foot.L"),
			("RightFoot", "DEF-Foot.R"),
			("Spine", "DEF-Stomach"),
			("Chest", "DEF-Chest"),
			("Neck", "DEF-Neck"),
			("Head", "DEF-Head"),
			("LeftShoulder", "DEF-Shoulder.L"),
			("RightShoulder", "DEF-Shoulder.R"),
			("LeftUpperArm", "DEF-UpperArm.L"),
			("RightUpperArm", "DEF-UpperArm.R"),
			("LeftLowerArm", "DEF-LowerArm.L"),
			("RightLowerArm", "DEF-LowerArm.R"),
			("LeftHand", "DEF-Hand.L"),
			("RightHand", "DEF-Hand.R"),
			("LeftToes", "DEF-ToeA1.L"),
			("RightToes", "DEF-ToeA1.R"),
			("RightEye", "DEF-Neck.Spine.001"),
			("Jaw", "DEF-Jaw"),
			("Left Thumb Proximal", "DEF-Thumb.L.001"),
			("Left Thumb Intermediate", "DEF-Thumb.L.002"),
			("Left Thumb Distal", "DEF-Thumb.L.003"),
			("Left Index Proximal", "DEF-Finger.A.L.001"),
			("Left Index Intermediate", "DEF-Finger.A.L.002"),
			("Left Index Distal", "DEF-Finger.A.L.003"),
			("Left Ring Proximal", "DEF-Finger.B.L.001"),
			("Left Ring Intermediate", "DEF-Finger.B.L.002"),
			("Left Ring Distal", "DEF-Finger.B.L.003"),
			("Left Little Proximal", "DEF-Finger.C.L.001"),
			("Left Little Intermediate", "DEF-Finger.C.L.002"),
			("Left Little Distal", "DEF-Finger.C.L.003"),
			("Right Thumb Proximal", "DEF-Thumb.R.001"),
			("Right Thumb Intermediate", "DEF-Thumb.R.002"),
			("Right Thumb Distal", "DEF-Thumb.R.003"),
			("Right Index Proximal", "DEF-Finger.A.R.001"),
			("Right Index Intermediate", "DEF-Finger.A.R.002"),
			("Right Index Distal", "DEF-Finger.A.R.003"),
			("Right Ring Proximal", "DEF-Finger.B.R.001"),
			("Right Ring Intermediate", "DEF-Finger.B.R.002"),
			("Right Ring Distal", "DEF-Finger.B.R.003"),
			("Right Little Proximal", "DEF-Finger.C.R.001"),
			("Right Little Intermediate", "DEF-Finger.C.R.002"),
			("Right Little Distal", "DEF-Finger.C.R.003")
		};

		// 939 has no middle finger; in a humanoid avatar that leaves a -1 gap inside the hand bone
		// table. Unity 6's native hand retargeter (Hand2SkeletonPose) crashes on that gap. Rebuilding
		// WITHOUT any finger bones makes the avatar fingerless, so Hand2SkeletonPose is skipped entirely.
		private static readonly string[] FingerKeywords = { "Thumb", "Index", "Middle", "Ring", "Little" };

		[MenuItem("SCP-939/Rebuild Humanoid Avatar (no fingers, no TDoF) [try this first]")]
		public static void RebuildNoFingers() => Rebuild(hasTranslationDoF: false, includeFingers: false);

		[MenuItem("SCP-939/Rebuild Humanoid Avatar (no Translation DoF)")]
		public static void RebuildNoTdof() => Rebuild(hasTranslationDoF: false, includeFingers: true);

		[MenuItem("SCP-939/Rebuild Humanoid Avatar (keep Translation DoF)")]
		public static void RebuildWithTdof() => Rebuild(hasTranslationDoF: true, includeFingers: true);

		// Guaranteed crash fix: drop the humanoid avatar on the body model + ragdoll so the native
		// retargeter (Human2SkeletonPose/Hand2SkeletonPose/SkeletonSetDoF) is never invoked. The 939
		// clips are humanoid-muscle-only, so the third-person body will no longer pose (it follows
		// movement but doesn't play idle/walk/attack on its bones). The local player's first-person
		// claws are a SEPARATE viewmodel (SCP-939 HUD.prefab, already Generic) and are unaffected.
		[MenuItem("SCP-939/Set Body Model to Generic (guaranteed no crash)")]
		public static void SetGeneric()
		{
			int n = 0;
			n += AssignAvatar(ModelPrefabPath, null) ? 1 : 0;
			n += AssignAvatar(RagdollPrefabPath, null) ? 1 : 0;
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Debug.Log($"[Scp939AvatarRebuilder] Cleared humanoid avatar on {n} prefab(s) (Generic). " +
				"Humanoid retargeting is now disabled — spawn as SCP-939 to confirm no crash.");
		}

		private static bool IsFinger(string humanName)
		{
			foreach (string kw in FingerKeywords)
			{
				if (humanName.Contains(kw))
				{
					return true;
				}
			}
			return false;
		}

		private static void Rebuild(bool hasTranslationDoF, bool includeFingers)
		{
			Avatar built = null;

			GameObject modelRoot = PrefabUtility.LoadPrefabContents(ModelPrefabPath);
			try
			{
				Transform anims = FindDeep(modelRoot.transform, AnimsRootName);
				if (anims == null)
				{
					Debug.LogError($"[Scp939AvatarRebuilder] '{AnimsRootName}' not found under {ModelPrefabPath}.");
					return;
				}

				HumanDescription hd = BuildDescription(anims.gameObject, hasTranslationDoF, includeFingers);
				built = AvatarBuilder.BuildHumanAvatar(anims.gameObject, hd);
				built.name = "939_RIGAvatar_Rebuilt";

				if (!built.isValid || !built.isHuman)
				{
					Debug.LogError($"[Scp939AvatarRebuilder] Build failed (isValid={built.isValid}, isHuman={built.isHuman}). " +
						"Check the console for AvatarBuilder warnings about unmapped/missing bones.");
					Object.DestroyImmediate(built);
					return;
				}

				AssetDatabase.CreateAsset(built, RebuiltAvatarPath);
				AssetDatabase.SaveAssets();
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(modelRoot);
			}

			Avatar saved = AssetDatabase.LoadAssetAtPath<Avatar>(RebuiltAvatarPath);
			int assigned = 0;
			assigned += AssignAvatar(ModelPrefabPath, saved) ? 1 : 0;
			assigned += AssignAvatar(RagdollPrefabPath, saved) ? 1 : 0;
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Debug.Log($"[Scp939AvatarRebuilder] Rebuilt avatar '{RebuiltAvatarPath}' " +
				$"(TranslationDoF={hasTranslationDoF}, fingers={includeFingers}) and assigned it to {assigned} prefab(s). " +
				"Enter Play mode and spawn as SCP-939 to verify the crash is gone.");
		}

		private static HumanDescription BuildDescription(GameObject animsRoot, bool hasTranslationDoF, bool includeFingers)
		{
			Transform[] all = animsRoot.GetComponentsInChildren<Transform>(includeInactive: true);
			SkeletonBone[] skeleton = new SkeletonBone[all.Length];
			for (int i = 0; i < all.Length; i++)
			{
				Transform t = all[i];
				skeleton[i] = new SkeletonBone
				{
					name = t.name,
					position = t.localPosition,
					rotation = t.localRotation,
					scale = t.localScale
				};
			}

			List<HumanBone> human = new List<HumanBone>(HumanMap.Length);
			foreach ((string humanName, string boneName) in HumanMap)
			{
				if (!includeFingers && IsFinger(humanName))
				{
					continue;
				}
				HumanBone bone = new HumanBone
				{
					humanName = humanName,
					boneName = boneName,
					limit = new HumanLimit { useDefaultValues = true }
				};
				human.Add(bone);
			}

			return new HumanDescription
			{
				human = human.ToArray(),
				skeleton = skeleton,
				upperArmTwist = 0.5f,
				lowerArmTwist = 0.5f,
				upperLegTwist = 0.5f,
				lowerLegTwist = 0.5f,
				armStretch = 0.05f,
				legStretch = 0.05f,
				feetSpacing = 0f,
				hasTranslationDoF = hasTranslationDoF
			};
		}

		private static bool AssignAvatar(string prefabPath, Avatar avatar)
		{
			GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
			try
			{
				Animator animator = root.GetComponentInChildren<Animator>(includeInactive: true);
				if (animator == null)
				{
					Debug.LogWarning($"[Scp939AvatarRebuilder] No Animator in {prefabPath} — skipped.");
					return false;
				}
				animator.avatar = avatar;
				PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
				return true;
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(root);
			}
		}

		private static Transform FindDeep(Transform root, string name)
		{
			if (root.name == name)
			{
				return root;
			}
			foreach (Transform child in root)
			{
				Transform found = FindDeep(child, name);
				if (found != null)
				{
					return found;
				}
			}
			return null;
		}
	}
}
#endif
