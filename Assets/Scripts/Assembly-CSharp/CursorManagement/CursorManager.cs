namespace CursorManagement
{
	public static class CursorManager
	{
		private static readonly global::System.Collections.Generic.HashSet<global::CursorManagement.ICursorOverride> Overrides = new global::System.Collections.Generic.HashSet<global::CursorManagement.ICursorOverride>();

		public static bool MovementLocked { get; private set; }

		[global::UnityEngine.RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			StaticUnityMethods.OnLateUpdate += LateUpdate;
		}

		private static void LateUpdate()
		{
			ProcessOverrides(out var bestOverride, out var anyMovementLocked);
			switch (bestOverride)
			{
			case global::CursorManagement.CursorOverrideMode.Confined:
				SetLockMode(global::UnityEngine.CursorLockMode.Confined);
				break;
			case global::CursorManagement.CursorOverrideMode.Centered:
				SetLockMode(global::UnityEngine.CursorLockMode.Locked);
				break;
			default:
				SetLockMode(global::UnityEngine.CursorLockMode.None);
				break;
			}
			MovementLocked = anyMovementLocked;
		}

		private static void ProcessOverrides(out global::CursorManagement.CursorOverrideMode bestOverride, out bool anyMovementLocked)
		{
			bestOverride = global::CursorManagement.CursorOverrideMode.NoOverride;
			anyMovementLocked = false;
			if (!StaticUnityMethods.IsPlaying)
			{
				return;
			}
			int num = 0;
			Overrides.RemoveWhere((global::CursorManagement.ICursorOverride x) => x == null || (x is global::UnityEngine.Object obj && obj == null));
			foreach (global::CursorManagement.ICursorOverride @override in Overrides)
			{
				num = global::UnityEngine.Mathf.Max(num, (int)@override.CursorOverride);
				anyMovementLocked |= @override.LockMovement;
			}
			bestOverride = (global::CursorManagement.CursorOverrideMode)num;
		}

		public static void SetLockMode(global::UnityEngine.CursorLockMode lockMode)
		{
			global::UnityEngine.Cursor.lockState = lockMode;
			global::UnityEngine.Cursor.visible = lockMode != global::UnityEngine.CursorLockMode.Locked;
		}

		public static bool Register(global::CursorManagement.ICursorOverride target)
		{
			return Overrides.Add(target);
		}

		public static bool Unregister(global::CursorManagement.ICursorOverride target)
		{
			return Overrides.Remove(target);
		}
	}
}
