namespace Discord
{
	public class Discord : global::System.IDisposable
	{
		internal struct FFIEvents
		{
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void DestroyHandler(global::System.IntPtr MethodsPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result RunCallbacksMethod(global::System.IntPtr methodsPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SetLogHookCallback(global::System.IntPtr ptr, global::Discord.LogLevel level, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string message);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SetLogHookMethod(global::System.IntPtr methodsPtr, global::Discord.LogLevel minLevel, global::System.IntPtr callbackData, global::Discord.Discord.FFIMethods.SetLogHookCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetApplicationManagerMethod(global::System.IntPtr discordPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetUserManagerMethod(global::System.IntPtr discordPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetImageManagerMethod(global::System.IntPtr discordPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetActivityManagerMethod(global::System.IntPtr discordPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetRelationshipManagerMethod(global::System.IntPtr discordPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetLobbyManagerMethod(global::System.IntPtr discordPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetNetworkManagerMethod(global::System.IntPtr discordPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetOverlayManagerMethod(global::System.IntPtr discordPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetStorageManagerMethod(global::System.IntPtr discordPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetStoreManagerMethod(global::System.IntPtr discordPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetVoiceManagerMethod(global::System.IntPtr discordPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::System.IntPtr GetAchievementManagerMethod(global::System.IntPtr discordPtr);

			internal global::Discord.Discord.FFIMethods.DestroyHandler Destroy;

			internal global::Discord.Discord.FFIMethods.RunCallbacksMethod RunCallbacks;

			internal global::Discord.Discord.FFIMethods.SetLogHookMethod SetLogHook;

			internal global::Discord.Discord.FFIMethods.GetApplicationManagerMethod GetApplicationManager;

			internal global::Discord.Discord.FFIMethods.GetUserManagerMethod GetUserManager;

			internal global::Discord.Discord.FFIMethods.GetImageManagerMethod GetImageManager;

			internal global::Discord.Discord.FFIMethods.GetActivityManagerMethod GetActivityManager;

			internal global::Discord.Discord.FFIMethods.GetRelationshipManagerMethod GetRelationshipManager;

			internal global::Discord.Discord.FFIMethods.GetLobbyManagerMethod GetLobbyManager;

			internal global::Discord.Discord.FFIMethods.GetNetworkManagerMethod GetNetworkManager;

			internal global::Discord.Discord.FFIMethods.GetOverlayManagerMethod GetOverlayManager;

			internal global::Discord.Discord.FFIMethods.GetStorageManagerMethod GetStorageManager;

			internal global::Discord.Discord.FFIMethods.GetStoreManagerMethod GetStoreManager;

			internal global::Discord.Discord.FFIMethods.GetVoiceManagerMethod GetVoiceManager;

			internal global::Discord.Discord.FFIMethods.GetAchievementManagerMethod GetAchievementManager;
		}

		internal struct FFICreateParams
		{
			internal long ClientId;

			internal ulong Flags;

			internal global::System.IntPtr Events;

			internal global::System.IntPtr EventData;

			internal global::System.IntPtr ApplicationEvents;

			internal uint ApplicationVersion;

			internal global::System.IntPtr UserEvents;

			internal uint UserVersion;

			internal global::System.IntPtr ImageEvents;

			internal uint ImageVersion;

			internal global::System.IntPtr ActivityEvents;

			internal uint ActivityVersion;

			internal global::System.IntPtr RelationshipEvents;

			internal uint RelationshipVersion;

			internal global::System.IntPtr LobbyEvents;

			internal uint LobbyVersion;

			internal global::System.IntPtr NetworkEvents;

			internal uint NetworkVersion;

			internal global::System.IntPtr OverlayEvents;

			internal uint OverlayVersion;

			internal global::System.IntPtr StorageEvents;

			internal uint StorageVersion;

			internal global::System.IntPtr StoreEvents;

			internal uint StoreVersion;

			internal global::System.IntPtr VoiceEvents;

			internal uint VoiceVersion;

			internal global::System.IntPtr AchievementEvents;

			internal uint AchievementVersion;
		}

		public delegate void SetLogHookHandler(global::Discord.LogLevel level, string message);

		private global::System.Runtime.InteropServices.GCHandle SelfHandle;

		private global::System.IntPtr EventsPtr;

		private global::Discord.Discord.FFIEvents Events;

		private global::System.IntPtr ApplicationEventsPtr;

		private global::Discord.ApplicationManager.FFIEvents ApplicationEvents;

		internal global::Discord.ApplicationManager ApplicationManagerInstance;

		private global::System.IntPtr UserEventsPtr;

		private global::Discord.UserManager.FFIEvents UserEvents;

		internal global::Discord.UserManager UserManagerInstance;

		private global::System.IntPtr ImageEventsPtr;

		private global::Discord.ImageManager.FFIEvents ImageEvents;

		internal global::Discord.ImageManager ImageManagerInstance;

		private global::System.IntPtr ActivityEventsPtr;

		private global::Discord.ActivityManager.FFIEvents ActivityEvents;

		internal global::Discord.ActivityManager ActivityManagerInstance;

		private global::System.IntPtr RelationshipEventsPtr;

		private global::Discord.RelationshipManager.FFIEvents RelationshipEvents;

		internal global::Discord.RelationshipManager RelationshipManagerInstance;

		private global::System.IntPtr LobbyEventsPtr;

		private global::Discord.LobbyManager.FFIEvents LobbyEvents;

		internal global::Discord.LobbyManager LobbyManagerInstance;

		private global::System.IntPtr NetworkEventsPtr;

		private global::Discord.NetworkManager.FFIEvents NetworkEvents;

		internal global::Discord.NetworkManager NetworkManagerInstance;

		private global::System.IntPtr OverlayEventsPtr;

		private global::Discord.OverlayManager.FFIEvents OverlayEvents;

		internal global::Discord.OverlayManager OverlayManagerInstance;

		private global::System.IntPtr StorageEventsPtr;

		private global::Discord.StorageManager.FFIEvents StorageEvents;

		internal global::Discord.StorageManager StorageManagerInstance;

		private global::System.IntPtr StoreEventsPtr;

		private global::Discord.StoreManager.FFIEvents StoreEvents;

		internal global::Discord.StoreManager StoreManagerInstance;

		private global::System.IntPtr VoiceEventsPtr;

		private global::Discord.VoiceManager.FFIEvents VoiceEvents;

		internal global::Discord.VoiceManager VoiceManagerInstance;

		private global::System.IntPtr AchievementEventsPtr;

		private global::Discord.AchievementManager.FFIEvents AchievementEvents;

		internal global::Discord.AchievementManager AchievementManagerInstance;

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::System.Runtime.InteropServices.GCHandle? setLogHook;

		private global::Discord.Discord.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.Discord.FFIMethods));
				}
				return (global::Discord.Discord.FFIMethods)MethodsStructure;
			}
		}

		[global::System.Runtime.InteropServices.DllImport("discord_game_sdk", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl, ExactSpelling = true)]
		private static extern global::Discord.Result DiscordCreate(uint version, ref global::Discord.Discord.FFICreateParams createParams, out global::System.IntPtr manager);

		public Discord(long clientId, ulong flags)
		{
			global::Discord.Discord.FFICreateParams createParams = default(global::Discord.Discord.FFICreateParams);
			createParams.ClientId = clientId;
			createParams.Flags = flags;
			Events = default(global::Discord.Discord.FFIEvents);
			EventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(Events));
			createParams.Events = EventsPtr;
			SelfHandle = global::System.Runtime.InteropServices.GCHandle.Alloc(this);
			createParams.EventData = global::System.Runtime.InteropServices.GCHandle.ToIntPtr(SelfHandle);
			ApplicationEvents = default(global::Discord.ApplicationManager.FFIEvents);
			ApplicationEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(ApplicationEvents));
			createParams.ApplicationEvents = ApplicationEventsPtr;
			createParams.ApplicationVersion = 1u;
			UserEvents = default(global::Discord.UserManager.FFIEvents);
			UserEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(UserEvents));
			createParams.UserEvents = UserEventsPtr;
			createParams.UserVersion = 1u;
			ImageEvents = default(global::Discord.ImageManager.FFIEvents);
			ImageEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(ImageEvents));
			createParams.ImageEvents = ImageEventsPtr;
			createParams.ImageVersion = 1u;
			ActivityEvents = default(global::Discord.ActivityManager.FFIEvents);
			ActivityEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(ActivityEvents));
			createParams.ActivityEvents = ActivityEventsPtr;
			createParams.ActivityVersion = 1u;
			RelationshipEvents = default(global::Discord.RelationshipManager.FFIEvents);
			RelationshipEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(RelationshipEvents));
			createParams.RelationshipEvents = RelationshipEventsPtr;
			createParams.RelationshipVersion = 1u;
			LobbyEvents = default(global::Discord.LobbyManager.FFIEvents);
			LobbyEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(LobbyEvents));
			createParams.LobbyEvents = LobbyEventsPtr;
			createParams.LobbyVersion = 1u;
			NetworkEvents = default(global::Discord.NetworkManager.FFIEvents);
			NetworkEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(NetworkEvents));
			createParams.NetworkEvents = NetworkEventsPtr;
			createParams.NetworkVersion = 1u;
			OverlayEvents = default(global::Discord.OverlayManager.FFIEvents);
			OverlayEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(OverlayEvents));
			createParams.OverlayEvents = OverlayEventsPtr;
			createParams.OverlayVersion = 1u;
			StorageEvents = default(global::Discord.StorageManager.FFIEvents);
			StorageEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(StorageEvents));
			createParams.StorageEvents = StorageEventsPtr;
			createParams.StorageVersion = 1u;
			StoreEvents = default(global::Discord.StoreManager.FFIEvents);
			StoreEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(StoreEvents));
			createParams.StoreEvents = StoreEventsPtr;
			createParams.StoreVersion = 1u;
			VoiceEvents = default(global::Discord.VoiceManager.FFIEvents);
			VoiceEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(VoiceEvents));
			createParams.VoiceEvents = VoiceEventsPtr;
			createParams.VoiceVersion = 1u;
			AchievementEvents = default(global::Discord.AchievementManager.FFIEvents);
			AchievementEventsPtr = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(global::System.Runtime.InteropServices.Marshal.SizeOf(AchievementEvents));
			createParams.AchievementEvents = AchievementEventsPtr;
			createParams.AchievementVersion = 1u;
			InitEvents(EventsPtr, ref Events);
			global::Discord.Result result = DiscordCreate(2u, ref createParams, out MethodsPtr);
			if (result != global::Discord.Result.Ok)
			{
				Dispose();
				throw new global::Discord.ResultException(result);
			}
		}

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.Discord.FFIEvents events)
		{
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		public void Dispose()
		{
			if (MethodsPtr != global::System.IntPtr.Zero)
			{
				Methods.Destroy(MethodsPtr);
			}
			SelfHandle.Free();
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(EventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(ApplicationEventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(UserEventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(ImageEventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(ActivityEventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(RelationshipEventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(LobbyEventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(NetworkEventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(OverlayEventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(StorageEventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(StoreEventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(VoiceEventsPtr);
			global::System.Runtime.InteropServices.Marshal.FreeHGlobal(AchievementEventsPtr);
			if (setLogHook.HasValue)
			{
				setLogHook.Value.Free();
			}
		}

		public void RunCallbacks()
		{
			global::Discord.Result result = Methods.RunCallbacks(MethodsPtr);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void SetLogHookCallbackImpl(global::System.IntPtr ptr, global::Discord.LogLevel level, string message)
		{
			((global::Discord.Discord.SetLogHookHandler)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target)(level, message);
		}

		public void SetLogHook(global::Discord.LogLevel minLevel, global::Discord.Discord.SetLogHookHandler callback)
		{
			if (setLogHook.HasValue)
			{
				setLogHook.Value.Free();
			}
			setLogHook = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.SetLogHook(MethodsPtr, minLevel, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(setLogHook.Value), SetLogHookCallbackImpl);
		}

		public global::Discord.ApplicationManager GetApplicationManager()
		{
			if (ApplicationManagerInstance == null)
			{
				ApplicationManagerInstance = new global::Discord.ApplicationManager(Methods.GetApplicationManager(MethodsPtr), ApplicationEventsPtr, ref ApplicationEvents);
			}
			return ApplicationManagerInstance;
		}

		public global::Discord.UserManager GetUserManager()
		{
			if (UserManagerInstance == null)
			{
				UserManagerInstance = new global::Discord.UserManager(Methods.GetUserManager(MethodsPtr), UserEventsPtr, ref UserEvents);
			}
			return UserManagerInstance;
		}

		public global::Discord.ImageManager GetImageManager()
		{
			if (ImageManagerInstance == null)
			{
				ImageManagerInstance = new global::Discord.ImageManager(Methods.GetImageManager(MethodsPtr), ImageEventsPtr, ref ImageEvents);
			}
			return ImageManagerInstance;
		}

		public global::Discord.ActivityManager GetActivityManager()
		{
			if (ActivityManagerInstance == null)
			{
				ActivityManagerInstance = new global::Discord.ActivityManager(Methods.GetActivityManager(MethodsPtr), ActivityEventsPtr, ref ActivityEvents);
			}
			return ActivityManagerInstance;
		}

		public global::Discord.RelationshipManager GetRelationshipManager()
		{
			if (RelationshipManagerInstance == null)
			{
				RelationshipManagerInstance = new global::Discord.RelationshipManager(Methods.GetRelationshipManager(MethodsPtr), RelationshipEventsPtr, ref RelationshipEvents);
			}
			return RelationshipManagerInstance;
		}

		public global::Discord.LobbyManager GetLobbyManager()
		{
			if (LobbyManagerInstance == null)
			{
				LobbyManagerInstance = new global::Discord.LobbyManager(Methods.GetLobbyManager(MethodsPtr), LobbyEventsPtr, ref LobbyEvents);
			}
			return LobbyManagerInstance;
		}

		public global::Discord.NetworkManager GetNetworkManager()
		{
			if (NetworkManagerInstance == null)
			{
				NetworkManagerInstance = new global::Discord.NetworkManager(Methods.GetNetworkManager(MethodsPtr), NetworkEventsPtr, ref NetworkEvents);
			}
			return NetworkManagerInstance;
		}

		public global::Discord.OverlayManager GetOverlayManager()
		{
			if (OverlayManagerInstance == null)
			{
				OverlayManagerInstance = new global::Discord.OverlayManager(Methods.GetOverlayManager(MethodsPtr), OverlayEventsPtr, ref OverlayEvents);
			}
			return OverlayManagerInstance;
		}

		public global::Discord.StorageManager GetStorageManager()
		{
			if (StorageManagerInstance == null)
			{
				StorageManagerInstance = new global::Discord.StorageManager(Methods.GetStorageManager(MethodsPtr), StorageEventsPtr, ref StorageEvents);
			}
			return StorageManagerInstance;
		}

		public global::Discord.StoreManager GetStoreManager()
		{
			if (StoreManagerInstance == null)
			{
				StoreManagerInstance = new global::Discord.StoreManager(Methods.GetStoreManager(MethodsPtr), StoreEventsPtr, ref StoreEvents);
			}
			return StoreManagerInstance;
		}

		public global::Discord.VoiceManager GetVoiceManager()
		{
			if (VoiceManagerInstance == null)
			{
				VoiceManagerInstance = new global::Discord.VoiceManager(Methods.GetVoiceManager(MethodsPtr), VoiceEventsPtr, ref VoiceEvents);
			}
			return VoiceManagerInstance;
		}

		public global::Discord.AchievementManager GetAchievementManager()
		{
			if (AchievementManagerInstance == null)
			{
				AchievementManagerInstance = new global::Discord.AchievementManager(Methods.GetAchievementManager(MethodsPtr), AchievementEventsPtr, ref AchievementEvents);
			}
			return AchievementManagerInstance;
		}
	}
}
