namespace Discord
{
	public class ActivityManager
	{
		internal struct FFIEvents
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ActivityJoinHandler(global::System.IntPtr ptr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string secret);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ActivitySpectateHandler(global::System.IntPtr ptr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string secret);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ActivityJoinRequestHandler(global::System.IntPtr ptr, ref global::Discord.User user);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ActivityInviteHandler(global::System.IntPtr ptr, global::Discord.ActivityActionType type, ref global::Discord.User user, ref global::Discord.Activity activity);

			internal global::Discord.ActivityManager.FFIEvents.ActivityJoinHandler OnActivityJoin;

			internal global::Discord.ActivityManager.FFIEvents.ActivitySpectateHandler OnActivitySpectate;

			internal global::Discord.ActivityManager.FFIEvents.ActivityJoinRequestHandler OnActivityJoinRequest;

			internal global::Discord.ActivityManager.FFIEvents.ActivityInviteHandler OnActivityInvite;
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result RegisterCommandMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string command);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result RegisterSteamMethod(global::System.IntPtr methodsPtr, uint steamId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void UpdateActivityCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void UpdateActivityMethod(global::System.IntPtr methodsPtr, ref global::Discord.Activity activity, global::System.IntPtr callbackData, global::Discord.ActivityManager.FFIMethods.UpdateActivityCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ClearActivityCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ClearActivityMethod(global::System.IntPtr methodsPtr, global::System.IntPtr callbackData, global::Discord.ActivityManager.FFIMethods.ClearActivityCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SendRequestReplyCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SendRequestReplyMethod(global::System.IntPtr methodsPtr, long userId, global::Discord.ActivityJoinRequestReply reply, global::System.IntPtr callbackData, global::Discord.ActivityManager.FFIMethods.SendRequestReplyCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SendInviteCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SendInviteMethod(global::System.IntPtr methodsPtr, long userId, global::Discord.ActivityActionType type, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string content, global::System.IntPtr callbackData, global::Discord.ActivityManager.FFIMethods.SendInviteCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void AcceptInviteCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void AcceptInviteMethod(global::System.IntPtr methodsPtr, long userId, global::System.IntPtr callbackData, global::Discord.ActivityManager.FFIMethods.AcceptInviteCallback callback);

			internal global::Discord.ActivityManager.FFIMethods.RegisterCommandMethod RegisterCommand;

			internal global::Discord.ActivityManager.FFIMethods.RegisterSteamMethod RegisterSteam;

			internal global::Discord.ActivityManager.FFIMethods.UpdateActivityMethod UpdateActivity;

			internal global::Discord.ActivityManager.FFIMethods.ClearActivityMethod ClearActivity;

			internal global::Discord.ActivityManager.FFIMethods.SendRequestReplyMethod SendRequestReply;

			internal global::Discord.ActivityManager.FFIMethods.SendInviteMethod SendInvite;

			internal global::Discord.ActivityManager.FFIMethods.AcceptInviteMethod AcceptInvite;
		}

		public delegate void UpdateActivityHandler(global::Discord.Result result);

		public delegate void ClearActivityHandler(global::Discord.Result result);

		public delegate void SendRequestReplyHandler(global::Discord.Result result);

		public delegate void SendInviteHandler(global::Discord.Result result);

		public delegate void AcceptInviteHandler(global::Discord.Result result);

		public delegate void ActivityJoinHandler(string secret);

		public delegate void ActivitySpectateHandler(string secret);

		public delegate void ActivityJoinRequestHandler(ref global::Discord.User user);

		public delegate void ActivityInviteHandler(global::Discord.ActivityActionType type, ref global::Discord.User user, ref global::Discord.Activity activity);

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.ActivityManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.ActivityManager.FFIMethods));
				}
				return (global::Discord.ActivityManager.FFIMethods)MethodsStructure;
			}
		}

		public event global::Discord.ActivityManager.ActivityJoinHandler OnActivityJoin;

		public event global::Discord.ActivityManager.ActivitySpectateHandler OnActivitySpectate;

		public event global::Discord.ActivityManager.ActivityJoinRequestHandler OnActivityJoinRequest;

		public event global::Discord.ActivityManager.ActivityInviteHandler OnActivityInvite;

		public void RegisterCommand()
		{
			RegisterCommand(null);
		}

		internal ActivityManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.ActivityManager.FFIEvents events)
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

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.ActivityManager.FFIEvents events)
		{
			events.OnActivityJoin = OnActivityJoinImpl;
			events.OnActivitySpectate = OnActivitySpectateImpl;
			events.OnActivityJoinRequest = OnActivityJoinRequestImpl;
			events.OnActivityInvite = OnActivityInviteImpl;
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		public void RegisterCommand(string command)
		{
			global::Discord.Result result = Methods.RegisterCommand(MethodsPtr, command);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void RegisterSteam(uint steamId)
		{
			global::Discord.Result result = Methods.RegisterSteam(MethodsPtr, steamId);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void UpdateActivityCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.ActivityManager.UpdateActivityHandler obj = (global::Discord.ActivityManager.UpdateActivityHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void UpdateActivity(global::Discord.Activity activity, global::Discord.ActivityManager.UpdateActivityHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.UpdateActivity(MethodsPtr, ref activity, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), UpdateActivityCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void ClearActivityCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.ActivityManager.ClearActivityHandler obj = (global::Discord.ActivityManager.ClearActivityHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void ClearActivity(global::Discord.ActivityManager.ClearActivityHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.ClearActivity(MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), ClearActivityCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void SendRequestReplyCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.ActivityManager.SendRequestReplyHandler obj = (global::Discord.ActivityManager.SendRequestReplyHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void SendRequestReply(long userId, global::Discord.ActivityJoinRequestReply reply, global::Discord.ActivityManager.SendRequestReplyHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.SendRequestReply(MethodsPtr, userId, reply, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), SendRequestReplyCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void SendInviteCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.ActivityManager.SendInviteHandler obj = (global::Discord.ActivityManager.SendInviteHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void SendInvite(long userId, global::Discord.ActivityActionType type, string content, global::Discord.ActivityManager.SendInviteHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.SendInvite(MethodsPtr, userId, type, content, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), SendInviteCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void AcceptInviteCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.ActivityManager.AcceptInviteHandler obj = (global::Discord.ActivityManager.AcceptInviteHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void AcceptInvite(long userId, global::Discord.ActivityManager.AcceptInviteHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.AcceptInvite(MethodsPtr, userId, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), AcceptInviteCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnActivityJoinImpl(global::System.IntPtr ptr, string secret)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.ActivityManagerInstance.OnActivityJoin != null)
			{
				discord.ActivityManagerInstance.OnActivityJoin(secret);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnActivitySpectateImpl(global::System.IntPtr ptr, string secret)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.ActivityManagerInstance.OnActivitySpectate != null)
			{
				discord.ActivityManagerInstance.OnActivitySpectate(secret);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnActivityJoinRequestImpl(global::System.IntPtr ptr, ref global::Discord.User user)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.ActivityManagerInstance.OnActivityJoinRequest != null)
			{
				discord.ActivityManagerInstance.OnActivityJoinRequest(ref user);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnActivityInviteImpl(global::System.IntPtr ptr, global::Discord.ActivityActionType type, ref global::Discord.User user, ref global::Discord.Activity activity)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.ActivityManagerInstance.OnActivityInvite != null)
			{
				discord.ActivityManagerInstance.OnActivityInvite(type, ref user, ref activity);
			}
		}
	}
}
