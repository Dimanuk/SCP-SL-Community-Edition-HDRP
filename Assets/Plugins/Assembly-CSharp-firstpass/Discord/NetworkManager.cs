namespace Discord
{
	public class NetworkManager
	{
		internal struct FFIEvents
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void MessageHandler(global::System.IntPtr ptr, ulong peerId, byte channelId, global::System.IntPtr dataPtr, int dataLen);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void RouteUpdateHandler(global::System.IntPtr ptr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string routeData);

			internal global::Discord.NetworkManager.FFIEvents.MessageHandler OnMessage;

			internal global::Discord.NetworkManager.FFIEvents.RouteUpdateHandler OnRouteUpdate;
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void GetPeerIdMethod(global::System.IntPtr methodsPtr, ref ulong peerId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result FlushMethod(global::System.IntPtr methodsPtr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result OpenPeerMethod(global::System.IntPtr methodsPtr, ulong peerId, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string routeData);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result UpdatePeerMethod(global::System.IntPtr methodsPtr, ulong peerId, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string routeData);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result ClosePeerMethod(global::System.IntPtr methodsPtr, ulong peerId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result OpenChannelMethod(global::System.IntPtr methodsPtr, ulong peerId, byte channelId, bool reliable);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result CloseChannelMethod(global::System.IntPtr methodsPtr, ulong peerId, byte channelId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SendMessageMethod(global::System.IntPtr methodsPtr, ulong peerId, byte channelId, byte[] data, int dataLen);

			internal global::Discord.NetworkManager.FFIMethods.GetPeerIdMethod GetPeerId;

			internal global::Discord.NetworkManager.FFIMethods.FlushMethod Flush;

			internal global::Discord.NetworkManager.FFIMethods.OpenPeerMethod OpenPeer;

			internal global::Discord.NetworkManager.FFIMethods.UpdatePeerMethod UpdatePeer;

			internal global::Discord.NetworkManager.FFIMethods.ClosePeerMethod ClosePeer;

			internal global::Discord.NetworkManager.FFIMethods.OpenChannelMethod OpenChannel;

			internal global::Discord.NetworkManager.FFIMethods.CloseChannelMethod CloseChannel;

			internal global::Discord.NetworkManager.FFIMethods.SendMessageMethod SendMessage;
		}

		public delegate void MessageHandler(ulong peerId, byte channelId, byte[] data);

		public delegate void RouteUpdateHandler(string routeData);

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.NetworkManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.NetworkManager.FFIMethods));
				}
				return (global::Discord.NetworkManager.FFIMethods)MethodsStructure;
			}
		}

		public event global::Discord.NetworkManager.MessageHandler OnMessage;

		public event global::Discord.NetworkManager.RouteUpdateHandler OnRouteUpdate;

		internal NetworkManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.NetworkManager.FFIEvents events)
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

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.NetworkManager.FFIEvents events)
		{
			events.OnMessage = OnMessageImpl;
			events.OnRouteUpdate = OnRouteUpdateImpl;
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		public ulong GetPeerId()
		{
			ulong peerId = 0uL;
			Methods.GetPeerId(MethodsPtr, ref peerId);
			return peerId;
		}

		public void Flush()
		{
			global::Discord.Result result = Methods.Flush(MethodsPtr);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void OpenPeer(ulong peerId, string routeData)
		{
			global::Discord.Result result = Methods.OpenPeer(MethodsPtr, peerId, routeData);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void UpdatePeer(ulong peerId, string routeData)
		{
			global::Discord.Result result = Methods.UpdatePeer(MethodsPtr, peerId, routeData);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void ClosePeer(ulong peerId)
		{
			global::Discord.Result result = Methods.ClosePeer(MethodsPtr, peerId);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void OpenChannel(ulong peerId, byte channelId, bool reliable)
		{
			global::Discord.Result result = Methods.OpenChannel(MethodsPtr, peerId, channelId, reliable);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void CloseChannel(ulong peerId, byte channelId)
		{
			global::Discord.Result result = Methods.CloseChannel(MethodsPtr, peerId, channelId);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void SendMessage(ulong peerId, byte channelId, byte[] data)
		{
			global::Discord.Result result = Methods.SendMessage(MethodsPtr, peerId, channelId, data, data.Length);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnMessageImpl(global::System.IntPtr ptr, ulong peerId, byte channelId, global::System.IntPtr dataPtr, int dataLen)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.NetworkManagerInstance.OnMessage != null)
			{
				byte[] array = new byte[dataLen];
				global::System.Runtime.InteropServices.Marshal.Copy(dataPtr, array, 0, dataLen);
				discord.NetworkManagerInstance.OnMessage(peerId, channelId, array);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnRouteUpdateImpl(global::System.IntPtr ptr, string routeData)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.NetworkManagerInstance.OnRouteUpdate != null)
			{
				discord.NetworkManagerInstance.OnRouteUpdate(routeData);
			}
		}
	}
}
