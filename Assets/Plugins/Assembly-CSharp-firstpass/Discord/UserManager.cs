namespace Discord
{
	public class UserManager
	{
		internal struct FFIEvents
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void CurrentUserUpdateHandler(global::System.IntPtr ptr);

			internal global::Discord.UserManager.FFIEvents.CurrentUserUpdateHandler OnCurrentUserUpdate;
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetCurrentUserMethod(global::System.IntPtr methodsPtr, ref global::Discord.User currentUser);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void GetUserCallback(global::System.IntPtr ptr, global::Discord.Result result, ref global::Discord.User user);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void GetUserMethod(global::System.IntPtr methodsPtr, long userId, global::System.IntPtr callbackData, global::Discord.UserManager.FFIMethods.GetUserCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetCurrentUserPremiumTypeMethod(global::System.IntPtr methodsPtr, ref global::Discord.PremiumType premiumType);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result CurrentUserHasFlagMethod(global::System.IntPtr methodsPtr, global::Discord.UserFlag flag, ref bool hasFlag);

			internal global::Discord.UserManager.FFIMethods.GetCurrentUserMethod GetCurrentUser;

			internal global::Discord.UserManager.FFIMethods.GetUserMethod GetUser;

			internal global::Discord.UserManager.FFIMethods.GetCurrentUserPremiumTypeMethod GetCurrentUserPremiumType;

			internal global::Discord.UserManager.FFIMethods.CurrentUserHasFlagMethod CurrentUserHasFlag;
		}

		public delegate void GetUserHandler(global::Discord.Result result, ref global::Discord.User user);

		public delegate void CurrentUserUpdateHandler();

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.UserManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.UserManager.FFIMethods));
				}
				return (global::Discord.UserManager.FFIMethods)MethodsStructure;
			}
		}

		public event global::Discord.UserManager.CurrentUserUpdateHandler OnCurrentUserUpdate;

		internal UserManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.UserManager.FFIEvents events)
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

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.UserManager.FFIEvents events)
		{
			events.OnCurrentUserUpdate = OnCurrentUserUpdateImpl;
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		public global::Discord.User GetCurrentUser()
		{
			global::Discord.User currentUser = default(global::Discord.User);
			global::Discord.Result result = Methods.GetCurrentUser(MethodsPtr, ref currentUser);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return currentUser;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void GetUserCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result, ref global::Discord.User user)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.UserManager.GetUserHandler obj = (global::Discord.UserManager.GetUserHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result, ref user);
		}

		public void GetUser(long userId, global::Discord.UserManager.GetUserHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.GetUser(MethodsPtr, userId, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), GetUserCallbackImpl);
		}

		public global::Discord.PremiumType GetCurrentUserPremiumType()
		{
			global::Discord.PremiumType premiumType = global::Discord.PremiumType.None;
			global::Discord.Result result = Methods.GetCurrentUserPremiumType(MethodsPtr, ref premiumType);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return premiumType;
		}

		public bool CurrentUserHasFlag(global::Discord.UserFlag flag)
		{
			bool hasFlag = false;
			global::Discord.Result result = Methods.CurrentUserHasFlag(MethodsPtr, flag, ref hasFlag);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return hasFlag;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnCurrentUserUpdateImpl(global::System.IntPtr ptr)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.UserManagerInstance.OnCurrentUserUpdate != null)
			{
				discord.UserManagerInstance.OnCurrentUserUpdate();
			}
		}
	}
}
