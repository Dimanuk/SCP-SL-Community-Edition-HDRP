namespace Discord
{
	public class VoiceManager
	{
		internal struct FFIEvents
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SettingsUpdateHandler(global::System.IntPtr ptr);

			internal global::Discord.VoiceManager.FFIEvents.SettingsUpdateHandler OnSettingsUpdate;
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetInputModeMethod(global::System.IntPtr methodsPtr, ref global::Discord.InputMode inputMode);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SetInputModeCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SetInputModeMethod(global::System.IntPtr methodsPtr, global::Discord.InputMode inputMode, global::System.IntPtr callbackData, global::Discord.VoiceManager.FFIMethods.SetInputModeCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result IsSelfMuteMethod(global::System.IntPtr methodsPtr, ref bool mute);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SetSelfMuteMethod(global::System.IntPtr methodsPtr, bool mute);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result IsSelfDeafMethod(global::System.IntPtr methodsPtr, ref bool deaf);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SetSelfDeafMethod(global::System.IntPtr methodsPtr, bool deaf);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result IsLocalMuteMethod(global::System.IntPtr methodsPtr, long userId, ref bool mute);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SetLocalMuteMethod(global::System.IntPtr methodsPtr, long userId, bool mute);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetLocalVolumeMethod(global::System.IntPtr methodsPtr, long userId, ref byte volume);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SetLocalVolumeMethod(global::System.IntPtr methodsPtr, long userId, byte volume);

			internal global::Discord.VoiceManager.FFIMethods.GetInputModeMethod GetInputMode;

			internal global::Discord.VoiceManager.FFIMethods.SetInputModeMethod SetInputMode;

			internal global::Discord.VoiceManager.FFIMethods.IsSelfMuteMethod IsSelfMute;

			internal global::Discord.VoiceManager.FFIMethods.SetSelfMuteMethod SetSelfMute;

			internal global::Discord.VoiceManager.FFIMethods.IsSelfDeafMethod IsSelfDeaf;

			internal global::Discord.VoiceManager.FFIMethods.SetSelfDeafMethod SetSelfDeaf;

			internal global::Discord.VoiceManager.FFIMethods.IsLocalMuteMethod IsLocalMute;

			internal global::Discord.VoiceManager.FFIMethods.SetLocalMuteMethod SetLocalMute;

			internal global::Discord.VoiceManager.FFIMethods.GetLocalVolumeMethod GetLocalVolume;

			internal global::Discord.VoiceManager.FFIMethods.SetLocalVolumeMethod SetLocalVolume;
		}

		public delegate void SetInputModeHandler(global::Discord.Result result);

		public delegate void SettingsUpdateHandler();

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.VoiceManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.VoiceManager.FFIMethods));
				}
				return (global::Discord.VoiceManager.FFIMethods)MethodsStructure;
			}
		}

		public event global::Discord.VoiceManager.SettingsUpdateHandler OnSettingsUpdate;

		internal VoiceManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.VoiceManager.FFIEvents events)
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

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.VoiceManager.FFIEvents events)
		{
			events.OnSettingsUpdate = OnSettingsUpdateImpl;
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		public global::Discord.InputMode GetInputMode()
		{
			global::Discord.InputMode inputMode = default(global::Discord.InputMode);
			global::Discord.Result result = Methods.GetInputMode(MethodsPtr, ref inputMode);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return inputMode;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void SetInputModeCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.VoiceManager.SetInputModeHandler obj = (global::Discord.VoiceManager.SetInputModeHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void SetInputMode(global::Discord.InputMode inputMode, global::Discord.VoiceManager.SetInputModeHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.SetInputMode(MethodsPtr, inputMode, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), SetInputModeCallbackImpl);
		}

		public bool IsSelfMute()
		{
			bool mute = false;
			global::Discord.Result result = Methods.IsSelfMute(MethodsPtr, ref mute);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return mute;
		}

		public void SetSelfMute(bool mute)
		{
			global::Discord.Result result = Methods.SetSelfMute(MethodsPtr, mute);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public bool IsSelfDeaf()
		{
			bool deaf = false;
			global::Discord.Result result = Methods.IsSelfDeaf(MethodsPtr, ref deaf);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return deaf;
		}

		public void SetSelfDeaf(bool deaf)
		{
			global::Discord.Result result = Methods.SetSelfDeaf(MethodsPtr, deaf);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public bool IsLocalMute(long userId)
		{
			bool mute = false;
			global::Discord.Result result = Methods.IsLocalMute(MethodsPtr, userId, ref mute);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return mute;
		}

		public void SetLocalMute(long userId, bool mute)
		{
			global::Discord.Result result = Methods.SetLocalMute(MethodsPtr, userId, mute);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public byte GetLocalVolume(long userId)
		{
			byte volume = 0;
			global::Discord.Result result = Methods.GetLocalVolume(MethodsPtr, userId, ref volume);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return volume;
		}

		public void SetLocalVolume(long userId, byte volume)
		{
			global::Discord.Result result = Methods.SetLocalVolume(MethodsPtr, userId, volume);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnSettingsUpdateImpl(global::System.IntPtr ptr)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.VoiceManagerInstance.OnSettingsUpdate != null)
			{
				discord.VoiceManagerInstance.OnSettingsUpdate();
			}
		}
	}
}
