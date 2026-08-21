namespace Discord
{
	public class OverlayManager
	{
		internal struct FFIEvents
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ToggleHandler(global::System.IntPtr ptr, bool locked);

			internal global::Discord.OverlayManager.FFIEvents.ToggleHandler OnToggle;
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void IsEnabledMethod(global::System.IntPtr methodsPtr, ref bool enabled);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void IsLockedMethod(global::System.IntPtr methodsPtr, ref bool locked);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SetLockedCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SetLockedMethod(global::System.IntPtr methodsPtr, bool locked, global::System.IntPtr callbackData, global::Discord.OverlayManager.FFIMethods.SetLockedCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void OpenActivityInviteCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void OpenActivityInviteMethod(global::System.IntPtr methodsPtr, global::Discord.ActivityActionType type, global::System.IntPtr callbackData, global::Discord.OverlayManager.FFIMethods.OpenActivityInviteCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void OpenGuildInviteCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void OpenGuildInviteMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string code, global::System.IntPtr callbackData, global::Discord.OverlayManager.FFIMethods.OpenGuildInviteCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void OpenVoiceSettingsCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void OpenVoiceSettingsMethod(global::System.IntPtr methodsPtr, global::System.IntPtr callbackData, global::Discord.OverlayManager.FFIMethods.OpenVoiceSettingsCallback callback);

			internal global::Discord.OverlayManager.FFIMethods.IsEnabledMethod IsEnabled;

			internal global::Discord.OverlayManager.FFIMethods.IsLockedMethod IsLocked;

			internal global::Discord.OverlayManager.FFIMethods.SetLockedMethod SetLocked;

			internal global::Discord.OverlayManager.FFIMethods.OpenActivityInviteMethod OpenActivityInvite;

			internal global::Discord.OverlayManager.FFIMethods.OpenGuildInviteMethod OpenGuildInvite;

			internal global::Discord.OverlayManager.FFIMethods.OpenVoiceSettingsMethod OpenVoiceSettings;
		}

		public delegate void SetLockedHandler(global::Discord.Result result);

		public delegate void OpenActivityInviteHandler(global::Discord.Result result);

		public delegate void OpenGuildInviteHandler(global::Discord.Result result);

		public delegate void OpenVoiceSettingsHandler(global::Discord.Result result);

		public delegate void ToggleHandler(bool locked);

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.OverlayManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.OverlayManager.FFIMethods));
				}
				return (global::Discord.OverlayManager.FFIMethods)MethodsStructure;
			}
		}

		public event global::Discord.OverlayManager.ToggleHandler OnToggle;

		internal OverlayManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.OverlayManager.FFIEvents events)
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

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.OverlayManager.FFIEvents events)
		{
			events.OnToggle = OnToggleImpl;
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		public bool IsEnabled()
		{
			bool enabled = false;
			Methods.IsEnabled(MethodsPtr, ref enabled);
			return enabled;
		}

		public bool IsLocked()
		{
			bool locked = false;
			Methods.IsLocked(MethodsPtr, ref locked);
			return locked;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void SetLockedCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.OverlayManager.SetLockedHandler obj = (global::Discord.OverlayManager.SetLockedHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void SetLocked(bool locked, global::Discord.OverlayManager.SetLockedHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.SetLocked(MethodsPtr, locked, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), SetLockedCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OpenActivityInviteCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.OverlayManager.OpenActivityInviteHandler obj = (global::Discord.OverlayManager.OpenActivityInviteHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void OpenActivityInvite(global::Discord.ActivityActionType type, global::Discord.OverlayManager.OpenActivityInviteHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.OpenActivityInvite(MethodsPtr, type, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), OpenActivityInviteCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OpenGuildInviteCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.OverlayManager.OpenGuildInviteHandler obj = (global::Discord.OverlayManager.OpenGuildInviteHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void OpenGuildInvite(string code, global::Discord.OverlayManager.OpenGuildInviteHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.OpenGuildInvite(MethodsPtr, code, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), OpenGuildInviteCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OpenVoiceSettingsCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.OverlayManager.OpenVoiceSettingsHandler obj = (global::Discord.OverlayManager.OpenVoiceSettingsHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void OpenVoiceSettings(global::Discord.OverlayManager.OpenVoiceSettingsHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.OpenVoiceSettings(MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), OpenVoiceSettingsCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnToggleImpl(global::System.IntPtr ptr, bool locked)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.OverlayManagerInstance.OnToggle != null)
			{
				discord.OverlayManagerInstance.OnToggle(locked);
			}
		}
	}
}
