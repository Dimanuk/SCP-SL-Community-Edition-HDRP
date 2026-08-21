namespace Discord
{
	public class AchievementManager
	{
		internal struct FFIEvents
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void UserAchievementUpdateHandler(global::System.IntPtr ptr, ref global::Discord.UserAchievement userAchievement);

			internal global::Discord.AchievementManager.FFIEvents.UserAchievementUpdateHandler OnUserAchievementUpdate;
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SetUserAchievementCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SetUserAchievementMethod(global::System.IntPtr methodsPtr, long achievementId, byte percentComplete, global::System.IntPtr callbackData, global::Discord.AchievementManager.FFIMethods.SetUserAchievementCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void FetchUserAchievementsCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void FetchUserAchievementsMethod(global::System.IntPtr methodsPtr, global::System.IntPtr callbackData, global::Discord.AchievementManager.FFIMethods.FetchUserAchievementsCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void CountUserAchievementsMethod(global::System.IntPtr methodsPtr, ref int count);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetUserAchievementMethod(global::System.IntPtr methodsPtr, long userAchievementId, ref global::Discord.UserAchievement userAchievement);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetUserAchievementAtMethod(global::System.IntPtr methodsPtr, int index, ref global::Discord.UserAchievement userAchievement);

			internal global::Discord.AchievementManager.FFIMethods.SetUserAchievementMethod SetUserAchievement;

			internal global::Discord.AchievementManager.FFIMethods.FetchUserAchievementsMethod FetchUserAchievements;

			internal global::Discord.AchievementManager.FFIMethods.CountUserAchievementsMethod CountUserAchievements;

			internal global::Discord.AchievementManager.FFIMethods.GetUserAchievementMethod GetUserAchievement;

			internal global::Discord.AchievementManager.FFIMethods.GetUserAchievementAtMethod GetUserAchievementAt;
		}

		public delegate void SetUserAchievementHandler(global::Discord.Result result);

		public delegate void FetchUserAchievementsHandler(global::Discord.Result result);

		public delegate void UserAchievementUpdateHandler(ref global::Discord.UserAchievement userAchievement);

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.AchievementManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.AchievementManager.FFIMethods));
				}
				return (global::Discord.AchievementManager.FFIMethods)MethodsStructure;
			}
		}

		public event global::Discord.AchievementManager.UserAchievementUpdateHandler OnUserAchievementUpdate;

		internal AchievementManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.AchievementManager.FFIEvents events)
		{
			if (eventsPtr == global::System.IntPtr.Zero)
			{
				throw new global::Discord.ResultException(global::Discord.Result.InternalError);
			}
			InitEvents(eventsPtr, ref events);
			MethodsPtr = ptr;
			if (MethodsPtr == global::System.IntPtr.Zero)
			{
				throw new global::Discord.ResultException(global::Discord.Result.InternalError);
			}
		}

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.AchievementManager.FFIEvents events)
		{
			events.OnUserAchievementUpdate = OnUserAchievementUpdateImpl;
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void SetUserAchievementCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.AchievementManager.SetUserAchievementHandler obj = (global::Discord.AchievementManager.SetUserAchievementHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void SetUserAchievement(long achievementId, byte percentComplete, global::Discord.AchievementManager.SetUserAchievementHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.SetUserAchievement(MethodsPtr, achievementId, percentComplete, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), SetUserAchievementCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void FetchUserAchievementsCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.AchievementManager.FetchUserAchievementsHandler obj = (global::Discord.AchievementManager.FetchUserAchievementsHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void FetchUserAchievements(global::Discord.AchievementManager.FetchUserAchievementsHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.FetchUserAchievements(MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), FetchUserAchievementsCallbackImpl);
		}

		public int CountUserAchievements()
		{
			int count = 0;
			Methods.CountUserAchievements(MethodsPtr, ref count);
			return count;
		}

		public global::Discord.UserAchievement GetUserAchievement(long userAchievementId)
		{
			global::Discord.UserAchievement userAchievement = default(global::Discord.UserAchievement);
			global::Discord.Result result = Methods.GetUserAchievement(MethodsPtr, userAchievementId, ref userAchievement);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return userAchievement;
		}

		public global::Discord.UserAchievement GetUserAchievementAt(int index)
		{
			global::Discord.UserAchievement userAchievement = default(global::Discord.UserAchievement);
			global::Discord.Result result = Methods.GetUserAchievementAt(MethodsPtr, index, ref userAchievement);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return userAchievement;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnUserAchievementUpdateImpl(global::System.IntPtr ptr, ref global::Discord.UserAchievement userAchievement)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.AchievementManagerInstance.OnUserAchievementUpdate != null)
			{
				discord.AchievementManagerInstance.OnUserAchievementUpdate(ref userAchievement);
			}
		}
	}
}
