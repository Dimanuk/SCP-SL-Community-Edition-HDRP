namespace Discord
{
	public class LobbyManager
	{
		internal struct FFIEvents
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void LobbyUpdateHandler(global::System.IntPtr ptr, long lobbyId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void LobbyDeleteHandler(global::System.IntPtr ptr, long lobbyId, uint reason);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void MemberConnectHandler(global::System.IntPtr ptr, long lobbyId, long userId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void MemberUpdateHandler(global::System.IntPtr ptr, long lobbyId, long userId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void MemberDisconnectHandler(global::System.IntPtr ptr, long lobbyId, long userId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void LobbyMessageHandler(global::System.IntPtr ptr, long lobbyId, long userId, global::System.IntPtr dataPtr, int dataLen);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SpeakingHandler(global::System.IntPtr ptr, long lobbyId, long userId, bool speaking);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void NetworkMessageHandler(global::System.IntPtr ptr, long lobbyId, long userId, byte channelId, global::System.IntPtr dataPtr, int dataLen);

			internal global::Discord.LobbyManager.FFIEvents.LobbyUpdateHandler OnLobbyUpdate;

			internal global::Discord.LobbyManager.FFIEvents.LobbyDeleteHandler OnLobbyDelete;

			internal global::Discord.LobbyManager.FFIEvents.MemberConnectHandler OnMemberConnect;

			internal global::Discord.LobbyManager.FFIEvents.MemberUpdateHandler OnMemberUpdate;

			internal global::Discord.LobbyManager.FFIEvents.MemberDisconnectHandler OnMemberDisconnect;

			internal global::Discord.LobbyManager.FFIEvents.LobbyMessageHandler OnLobbyMessage;

			internal global::Discord.LobbyManager.FFIEvents.SpeakingHandler OnSpeaking;

			internal global::Discord.LobbyManager.FFIEvents.NetworkMessageHandler OnNetworkMessage;
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetLobbyCreateTransactionMethod(global::System.IntPtr methodsPtr, ref global::System.IntPtr transaction);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetLobbyUpdateTransactionMethod(global::System.IntPtr methodsPtr, long lobbyId, ref global::System.IntPtr transaction);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetMemberUpdateTransactionMethod(global::System.IntPtr methodsPtr, long lobbyId, long userId, ref global::System.IntPtr transaction);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void CreateLobbyCallback(global::System.IntPtr ptr, global::Discord.Result result, ref global::Discord.Lobby lobby);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void CreateLobbyMethod(global::System.IntPtr methodsPtr, global::System.IntPtr transaction, global::System.IntPtr callbackData, global::Discord.LobbyManager.FFIMethods.CreateLobbyCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void UpdateLobbyCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void UpdateLobbyMethod(global::System.IntPtr methodsPtr, long lobbyId, global::System.IntPtr transaction, global::System.IntPtr callbackData, global::Discord.LobbyManager.FFIMethods.UpdateLobbyCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void DeleteLobbyCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void DeleteLobbyMethod(global::System.IntPtr methodsPtr, long lobbyId, global::System.IntPtr callbackData, global::Discord.LobbyManager.FFIMethods.DeleteLobbyCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ConnectLobbyCallback(global::System.IntPtr ptr, global::Discord.Result result, ref global::Discord.Lobby lobby);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ConnectLobbyMethod(global::System.IntPtr methodsPtr, long lobbyId, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string secret, global::System.IntPtr callbackData, global::Discord.LobbyManager.FFIMethods.ConnectLobbyCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ConnectLobbyWithActivitySecretCallback(global::System.IntPtr ptr, global::Discord.Result result, ref global::Discord.Lobby lobby);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ConnectLobbyWithActivitySecretMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string activitySecret, global::System.IntPtr callbackData, global::Discord.LobbyManager.FFIMethods.ConnectLobbyWithActivitySecretCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void DisconnectLobbyCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void DisconnectLobbyMethod(global::System.IntPtr methodsPtr, long lobbyId, global::System.IntPtr callbackData, global::Discord.LobbyManager.FFIMethods.DisconnectLobbyCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetLobbyMethod(global::System.IntPtr methodsPtr, long lobbyId, ref global::Discord.Lobby lobby);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetLobbyActivitySecretMethod(global::System.IntPtr methodsPtr, long lobbyId, global::System.Text.StringBuilder secret);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetLobbyMetadataValueMethod(global::System.IntPtr methodsPtr, long lobbyId, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string key, global::System.Text.StringBuilder value);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetLobbyMetadataKeyMethod(global::System.IntPtr methodsPtr, long lobbyId, int index, global::System.Text.StringBuilder key);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result LobbyMetadataCountMethod(global::System.IntPtr methodsPtr, long lobbyId, ref int count);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result MemberCountMethod(global::System.IntPtr methodsPtr, long lobbyId, ref int count);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetMemberUserIdMethod(global::System.IntPtr methodsPtr, long lobbyId, int index, ref long userId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetMemberUserMethod(global::System.IntPtr methodsPtr, long lobbyId, long userId, ref global::Discord.User user);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetMemberMetadataValueMethod(global::System.IntPtr methodsPtr, long lobbyId, long userId, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string key, global::System.Text.StringBuilder value);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetMemberMetadataKeyMethod(global::System.IntPtr methodsPtr, long lobbyId, long userId, int index, global::System.Text.StringBuilder key);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result MemberMetadataCountMethod(global::System.IntPtr methodsPtr, long lobbyId, long userId, ref int count);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void UpdateMemberCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void UpdateMemberMethod(global::System.IntPtr methodsPtr, long lobbyId, long userId, global::System.IntPtr transaction, global::System.IntPtr callbackData, global::Discord.LobbyManager.FFIMethods.UpdateMemberCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SendLobbyMessageCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SendLobbyMessageMethod(global::System.IntPtr methodsPtr, long lobbyId, byte[] data, int dataLen, global::System.IntPtr callbackData, global::Discord.LobbyManager.FFIMethods.SendLobbyMessageCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetSearchQueryMethod(global::System.IntPtr methodsPtr, ref global::System.IntPtr query);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SearchCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void SearchMethod(global::System.IntPtr methodsPtr, global::System.IntPtr query, global::System.IntPtr callbackData, global::Discord.LobbyManager.FFIMethods.SearchCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void LobbyCountMethod(global::System.IntPtr methodsPtr, ref int count);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetLobbyIdMethod(global::System.IntPtr methodsPtr, int index, ref long lobbyId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ConnectVoiceCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ConnectVoiceMethod(global::System.IntPtr methodsPtr, long lobbyId, global::System.IntPtr callbackData, global::Discord.LobbyManager.FFIMethods.ConnectVoiceCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void DisconnectVoiceCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void DisconnectVoiceMethod(global::System.IntPtr methodsPtr, long lobbyId, global::System.IntPtr callbackData, global::Discord.LobbyManager.FFIMethods.DisconnectVoiceCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result ConnectNetworkMethod(global::System.IntPtr methodsPtr, long lobbyId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result DisconnectNetworkMethod(global::System.IntPtr methodsPtr, long lobbyId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result FlushNetworkMethod(global::System.IntPtr methodsPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result OpenNetworkChannelMethod(global::System.IntPtr methodsPtr, long lobbyId, byte channelId, bool reliable);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SendNetworkMessageMethod(global::System.IntPtr methodsPtr, long lobbyId, long userId, byte channelId, byte[] data, int dataLen);

			internal global::Discord.LobbyManager.FFIMethods.GetLobbyCreateTransactionMethod GetLobbyCreateTransaction;

			internal global::Discord.LobbyManager.FFIMethods.GetLobbyUpdateTransactionMethod GetLobbyUpdateTransaction;

			internal global::Discord.LobbyManager.FFIMethods.GetMemberUpdateTransactionMethod GetMemberUpdateTransaction;

			internal global::Discord.LobbyManager.FFIMethods.CreateLobbyMethod CreateLobby;

			internal global::Discord.LobbyManager.FFIMethods.UpdateLobbyMethod UpdateLobby;

			internal global::Discord.LobbyManager.FFIMethods.DeleteLobbyMethod DeleteLobby;

			internal global::Discord.LobbyManager.FFIMethods.ConnectLobbyMethod ConnectLobby;

			internal global::Discord.LobbyManager.FFIMethods.ConnectLobbyWithActivitySecretMethod ConnectLobbyWithActivitySecret;

			internal global::Discord.LobbyManager.FFIMethods.DisconnectLobbyMethod DisconnectLobby;

			internal global::Discord.LobbyManager.FFIMethods.GetLobbyMethod GetLobby;

			internal global::Discord.LobbyManager.FFIMethods.GetLobbyActivitySecretMethod GetLobbyActivitySecret;

			internal global::Discord.LobbyManager.FFIMethods.GetLobbyMetadataValueMethod GetLobbyMetadataValue;

			internal global::Discord.LobbyManager.FFIMethods.GetLobbyMetadataKeyMethod GetLobbyMetadataKey;

			internal global::Discord.LobbyManager.FFIMethods.LobbyMetadataCountMethod LobbyMetadataCount;

			internal global::Discord.LobbyManager.FFIMethods.MemberCountMethod MemberCount;

			internal global::Discord.LobbyManager.FFIMethods.GetMemberUserIdMethod GetMemberUserId;

			internal global::Discord.LobbyManager.FFIMethods.GetMemberUserMethod GetMemberUser;

			internal global::Discord.LobbyManager.FFIMethods.GetMemberMetadataValueMethod GetMemberMetadataValue;

			internal global::Discord.LobbyManager.FFIMethods.GetMemberMetadataKeyMethod GetMemberMetadataKey;

			internal global::Discord.LobbyManager.FFIMethods.MemberMetadataCountMethod MemberMetadataCount;

			internal global::Discord.LobbyManager.FFIMethods.UpdateMemberMethod UpdateMember;

			internal global::Discord.LobbyManager.FFIMethods.SendLobbyMessageMethod SendLobbyMessage;

			internal global::Discord.LobbyManager.FFIMethods.GetSearchQueryMethod GetSearchQuery;

			internal global::Discord.LobbyManager.FFIMethods.SearchMethod Search;

			internal global::Discord.LobbyManager.FFIMethods.LobbyCountMethod LobbyCount;

			internal global::Discord.LobbyManager.FFIMethods.GetLobbyIdMethod GetLobbyId;

			internal global::Discord.LobbyManager.FFIMethods.ConnectVoiceMethod ConnectVoice;

			internal global::Discord.LobbyManager.FFIMethods.DisconnectVoiceMethod DisconnectVoice;

			internal global::Discord.LobbyManager.FFIMethods.ConnectNetworkMethod ConnectNetwork;

			internal global::Discord.LobbyManager.FFIMethods.DisconnectNetworkMethod DisconnectNetwork;

			internal global::Discord.LobbyManager.FFIMethods.FlushNetworkMethod FlushNetwork;

			internal global::Discord.LobbyManager.FFIMethods.OpenNetworkChannelMethod OpenNetworkChannel;

			internal global::Discord.LobbyManager.FFIMethods.SendNetworkMessageMethod SendNetworkMessage;
		}

		public delegate void CreateLobbyHandler(global::Discord.Result result, ref global::Discord.Lobby lobby);

		public delegate void UpdateLobbyHandler(global::Discord.Result result);

		public delegate void DeleteLobbyHandler(global::Discord.Result result);

		public delegate void ConnectLobbyHandler(global::Discord.Result result, ref global::Discord.Lobby lobby);

		public delegate void ConnectLobbyWithActivitySecretHandler(global::Discord.Result result, ref global::Discord.Lobby lobby);

		public delegate void DisconnectLobbyHandler(global::Discord.Result result);

		public delegate void UpdateMemberHandler(global::Discord.Result result);

		public delegate void SendLobbyMessageHandler(global::Discord.Result result);

		public delegate void SearchHandler(global::Discord.Result result);

		public delegate void ConnectVoiceHandler(global::Discord.Result result);

		public delegate void DisconnectVoiceHandler(global::Discord.Result result);

		public delegate void LobbyUpdateHandler(long lobbyId);

		public delegate void LobbyDeleteHandler(long lobbyId, uint reason);

		public delegate void MemberConnectHandler(long lobbyId, long userId);

		public delegate void MemberUpdateHandler(long lobbyId, long userId);

		public delegate void MemberDisconnectHandler(long lobbyId, long userId);

		public delegate void LobbyMessageHandler(long lobbyId, long userId, byte[] data);

		public delegate void SpeakingHandler(long lobbyId, long userId, bool speaking);

		public delegate void NetworkMessageHandler(long lobbyId, long userId, byte channelId, byte[] data);

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.LobbyManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.LobbyManager.FFIMethods));
				}
				return (global::Discord.LobbyManager.FFIMethods)MethodsStructure;
			}
		}

		public event global::Discord.LobbyManager.LobbyUpdateHandler OnLobbyUpdate;

		public event global::Discord.LobbyManager.LobbyDeleteHandler OnLobbyDelete;

		public event global::Discord.LobbyManager.MemberConnectHandler OnMemberConnect;

		public event global::Discord.LobbyManager.MemberUpdateHandler OnMemberUpdate;

		public event global::Discord.LobbyManager.MemberDisconnectHandler OnMemberDisconnect;

		public event global::Discord.LobbyManager.LobbyMessageHandler OnLobbyMessage;

		public event global::Discord.LobbyManager.SpeakingHandler OnSpeaking;

		public event global::Discord.LobbyManager.NetworkMessageHandler OnNetworkMessage;

		internal LobbyManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.LobbyManager.FFIEvents events)
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

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.LobbyManager.FFIEvents events)
		{
			events.OnLobbyUpdate = OnLobbyUpdateImpl;
			events.OnLobbyDelete = OnLobbyDeleteImpl;
			events.OnMemberConnect = OnMemberConnectImpl;
			events.OnMemberUpdate = OnMemberUpdateImpl;
			events.OnMemberDisconnect = OnMemberDisconnectImpl;
			events.OnLobbyMessage = OnLobbyMessageImpl;
			events.OnSpeaking = OnSpeakingImpl;
			events.OnNetworkMessage = OnNetworkMessageImpl;
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		public global::Discord.LobbyTransaction GetLobbyCreateTransaction()
		{
			global::Discord.LobbyTransaction result = default(global::Discord.LobbyTransaction);
			global::Discord.Result result2 = Methods.GetLobbyCreateTransaction(MethodsPtr, ref result.MethodsPtr);
			if (result2 != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result2);
			}
			return result;
		}

		public global::Discord.LobbyTransaction GetLobbyUpdateTransaction(long lobbyId)
		{
			global::Discord.LobbyTransaction result = default(global::Discord.LobbyTransaction);
			global::Discord.Result result2 = Methods.GetLobbyUpdateTransaction(MethodsPtr, lobbyId, ref result.MethodsPtr);
			if (result2 != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result2);
			}
			return result;
		}

		public global::Discord.LobbyMemberTransaction GetMemberUpdateTransaction(long lobbyId, long userId)
		{
			global::Discord.LobbyMemberTransaction result = default(global::Discord.LobbyMemberTransaction);
			global::Discord.Result result2 = Methods.GetMemberUpdateTransaction(MethodsPtr, lobbyId, userId, ref result.MethodsPtr);
			if (result2 != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result2);
			}
			return result;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void CreateLobbyCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result, ref global::Discord.Lobby lobby)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.LobbyManager.CreateLobbyHandler obj = (global::Discord.LobbyManager.CreateLobbyHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result, ref lobby);
		}

		public void CreateLobby(global::Discord.LobbyTransaction transaction, global::Discord.LobbyManager.CreateLobbyHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.CreateLobby(MethodsPtr, transaction.MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), CreateLobbyCallbackImpl);
			transaction.MethodsPtr = global::System.IntPtr.Zero;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void UpdateLobbyCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.LobbyManager.UpdateLobbyHandler obj = (global::Discord.LobbyManager.UpdateLobbyHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void UpdateLobby(long lobbyId, global::Discord.LobbyTransaction transaction, global::Discord.LobbyManager.UpdateLobbyHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.UpdateLobby(MethodsPtr, lobbyId, transaction.MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), UpdateLobbyCallbackImpl);
			transaction.MethodsPtr = global::System.IntPtr.Zero;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void DeleteLobbyCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.LobbyManager.DeleteLobbyHandler obj = (global::Discord.LobbyManager.DeleteLobbyHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void DeleteLobby(long lobbyId, global::Discord.LobbyManager.DeleteLobbyHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.DeleteLobby(MethodsPtr, lobbyId, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), DeleteLobbyCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void ConnectLobbyCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result, ref global::Discord.Lobby lobby)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.LobbyManager.ConnectLobbyHandler obj = (global::Discord.LobbyManager.ConnectLobbyHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result, ref lobby);
		}

		public void ConnectLobby(long lobbyId, string secret, global::Discord.LobbyManager.ConnectLobbyHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.ConnectLobby(MethodsPtr, lobbyId, secret, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), ConnectLobbyCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void ConnectLobbyWithActivitySecretCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result, ref global::Discord.Lobby lobby)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.LobbyManager.ConnectLobbyWithActivitySecretHandler obj = (global::Discord.LobbyManager.ConnectLobbyWithActivitySecretHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result, ref lobby);
		}

		public void ConnectLobbyWithActivitySecret(string activitySecret, global::Discord.LobbyManager.ConnectLobbyWithActivitySecretHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.ConnectLobbyWithActivitySecret(MethodsPtr, activitySecret, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), ConnectLobbyWithActivitySecretCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void DisconnectLobbyCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.LobbyManager.DisconnectLobbyHandler obj = (global::Discord.LobbyManager.DisconnectLobbyHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void DisconnectLobby(long lobbyId, global::Discord.LobbyManager.DisconnectLobbyHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.DisconnectLobby(MethodsPtr, lobbyId, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), DisconnectLobbyCallbackImpl);
		}

		public global::Discord.Lobby GetLobby(long lobbyId)
		{
			global::Discord.Lobby lobby = default(global::Discord.Lobby);
			global::Discord.Result result = Methods.GetLobby(MethodsPtr, lobbyId, ref lobby);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return lobby;
		}

		public string GetLobbyActivitySecret(long lobbyId)
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(128);
			global::Discord.Result result = Methods.GetLobbyActivitySecret(MethodsPtr, lobbyId, stringBuilder);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return stringBuilder.ToString();
		}

		public string GetLobbyMetadataValue(long lobbyId, string key)
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(4096);
			global::Discord.Result result = Methods.GetLobbyMetadataValue(MethodsPtr, lobbyId, key, stringBuilder);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return stringBuilder.ToString();
		}

		public string GetLobbyMetadataKey(long lobbyId, int index)
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(256);
			global::Discord.Result result = Methods.GetLobbyMetadataKey(MethodsPtr, lobbyId, index, stringBuilder);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return stringBuilder.ToString();
		}

		public int LobbyMetadataCount(long lobbyId)
		{
			int count = 0;
			global::Discord.Result result = Methods.LobbyMetadataCount(MethodsPtr, lobbyId, ref count);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return count;
		}

		public int MemberCount(long lobbyId)
		{
			int count = 0;
			global::Discord.Result result = Methods.MemberCount(MethodsPtr, lobbyId, ref count);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return count;
		}

		public long GetMemberUserId(long lobbyId, int index)
		{
			long userId = 0L;
			global::Discord.Result result = Methods.GetMemberUserId(MethodsPtr, lobbyId, index, ref userId);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return userId;
		}

		public global::Discord.User GetMemberUser(long lobbyId, long userId)
		{
			global::Discord.User user = default(global::Discord.User);
			global::Discord.Result result = Methods.GetMemberUser(MethodsPtr, lobbyId, userId, ref user);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return user;
		}

		public string GetMemberMetadataValue(long lobbyId, long userId, string key)
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(4096);
			global::Discord.Result result = Methods.GetMemberMetadataValue(MethodsPtr, lobbyId, userId, key, stringBuilder);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return stringBuilder.ToString();
		}

		public string GetMemberMetadataKey(long lobbyId, long userId, int index)
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(256);
			global::Discord.Result result = Methods.GetMemberMetadataKey(MethodsPtr, lobbyId, userId, index, stringBuilder);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return stringBuilder.ToString();
		}

		public int MemberMetadataCount(long lobbyId, long userId)
		{
			int count = 0;
			global::Discord.Result result = Methods.MemberMetadataCount(MethodsPtr, lobbyId, userId, ref count);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return count;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void UpdateMemberCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.LobbyManager.UpdateMemberHandler obj = (global::Discord.LobbyManager.UpdateMemberHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void UpdateMember(long lobbyId, long userId, global::Discord.LobbyMemberTransaction transaction, global::Discord.LobbyManager.UpdateMemberHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.UpdateMember(MethodsPtr, lobbyId, userId, transaction.MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), UpdateMemberCallbackImpl);
			transaction.MethodsPtr = global::System.IntPtr.Zero;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void SendLobbyMessageCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.LobbyManager.SendLobbyMessageHandler obj = (global::Discord.LobbyManager.SendLobbyMessageHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void SendLobbyMessage(long lobbyId, byte[] data, global::Discord.LobbyManager.SendLobbyMessageHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.SendLobbyMessage(MethodsPtr, lobbyId, data, data.Length, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), SendLobbyMessageCallbackImpl);
		}

		public global::Discord.LobbySearchQuery GetSearchQuery()
		{
			global::Discord.LobbySearchQuery result = default(global::Discord.LobbySearchQuery);
			global::Discord.Result result2 = Methods.GetSearchQuery(MethodsPtr, ref result.MethodsPtr);
			if (result2 != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result2);
			}
			return result;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void SearchCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.LobbyManager.SearchHandler obj = (global::Discord.LobbyManager.SearchHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void Search(global::Discord.LobbySearchQuery query, global::Discord.LobbyManager.SearchHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.Search(MethodsPtr, query.MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), SearchCallbackImpl);
			query.MethodsPtr = global::System.IntPtr.Zero;
		}

		public int LobbyCount()
		{
			int count = 0;
			Methods.LobbyCount(MethodsPtr, ref count);
			return count;
		}

		public long GetLobbyId(int index)
		{
			long lobbyId = 0L;
			global::Discord.Result result = Methods.GetLobbyId(MethodsPtr, index, ref lobbyId);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return lobbyId;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void ConnectVoiceCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.LobbyManager.ConnectVoiceHandler obj = (global::Discord.LobbyManager.ConnectVoiceHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void ConnectVoice(long lobbyId, global::Discord.LobbyManager.ConnectVoiceHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.ConnectVoice(MethodsPtr, lobbyId, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), ConnectVoiceCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void DisconnectVoiceCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.LobbyManager.DisconnectVoiceHandler obj = (global::Discord.LobbyManager.DisconnectVoiceHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void DisconnectVoice(long lobbyId, global::Discord.LobbyManager.DisconnectVoiceHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.DisconnectVoice(MethodsPtr, lobbyId, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), DisconnectVoiceCallbackImpl);
		}

		public void ConnectNetwork(long lobbyId)
		{
			global::Discord.Result result = Methods.ConnectNetwork(MethodsPtr, lobbyId);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void DisconnectNetwork(long lobbyId)
		{
			global::Discord.Result result = Methods.DisconnectNetwork(MethodsPtr, lobbyId);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void FlushNetwork()
		{
			global::Discord.Result result = Methods.FlushNetwork(MethodsPtr);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void OpenNetworkChannel(long lobbyId, byte channelId, bool reliable)
		{
			global::Discord.Result result = Methods.OpenNetworkChannel(MethodsPtr, lobbyId, channelId, reliable);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void SendNetworkMessage(long lobbyId, long userId, byte channelId, byte[] data)
		{
			global::Discord.Result result = Methods.SendNetworkMessage(MethodsPtr, lobbyId, userId, channelId, data, data.Length);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnLobbyUpdateImpl(global::System.IntPtr ptr, long lobbyId)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnLobbyUpdate != null)
			{
				discord.LobbyManagerInstance.OnLobbyUpdate(lobbyId);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnLobbyDeleteImpl(global::System.IntPtr ptr, long lobbyId, uint reason)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnLobbyDelete != null)
			{
				discord.LobbyManagerInstance.OnLobbyDelete(lobbyId, reason);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnMemberConnectImpl(global::System.IntPtr ptr, long lobbyId, long userId)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnMemberConnect != null)
			{
				discord.LobbyManagerInstance.OnMemberConnect(lobbyId, userId);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnMemberUpdateImpl(global::System.IntPtr ptr, long lobbyId, long userId)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnMemberUpdate != null)
			{
				discord.LobbyManagerInstance.OnMemberUpdate(lobbyId, userId);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnMemberDisconnectImpl(global::System.IntPtr ptr, long lobbyId, long userId)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnMemberDisconnect != null)
			{
				discord.LobbyManagerInstance.OnMemberDisconnect(lobbyId, userId);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnLobbyMessageImpl(global::System.IntPtr ptr, long lobbyId, long userId, global::System.IntPtr dataPtr, int dataLen)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnLobbyMessage != null)
			{
				byte[] array = new byte[dataLen];
				global::System.Runtime.InteropServices.Marshal.Copy(dataPtr, array, 0, dataLen);
				discord.LobbyManagerInstance.OnLobbyMessage(lobbyId, userId, array);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnSpeakingImpl(global::System.IntPtr ptr, long lobbyId, long userId, bool speaking)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnSpeaking != null)
			{
				discord.LobbyManagerInstance.OnSpeaking(lobbyId, userId, speaking);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnNetworkMessageImpl(global::System.IntPtr ptr, long lobbyId, long userId, byte channelId, global::System.IntPtr dataPtr, int dataLen)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnNetworkMessage != null)
			{
				byte[] array = new byte[dataLen];
				global::System.Runtime.InteropServices.Marshal.Copy(dataPtr, array, 0, dataLen);
				discord.LobbyManagerInstance.OnNetworkMessage(lobbyId, userId, channelId, array);
			}
		}

		public global::System.Collections.Generic.IEnumerable<global::Discord.User> GetMemberUsers(long lobbyID)
		{
			int num = MemberCount(lobbyID);
			global::System.Collections.Generic.List<global::Discord.User> list = new global::System.Collections.Generic.List<global::Discord.User>();
			for (int i = 0; i < num; i++)
			{
				list.Add(GetMemberUser(lobbyID, GetMemberUserId(lobbyID, i)));
			}
			return list;
		}

		public void SendLobbyMessage(long lobbyID, string data, global::Discord.LobbyManager.SendLobbyMessageHandler handler)
		{
			SendLobbyMessage(lobbyID, global::System.Text.Encoding.UTF8.GetBytes(data), handler);
		}
	}
}
