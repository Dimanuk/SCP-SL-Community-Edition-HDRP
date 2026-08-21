namespace Discord
{
	public struct LobbyTransaction
	{
		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SetTypeMethod(global::System.IntPtr methodsPtr, global::Discord.LobbyType type);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SetOwnerMethod(global::System.IntPtr methodsPtr, long ownerId);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SetCapacityMethod(global::System.IntPtr methodsPtr, uint capacity);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SetMetadataMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string key, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string value);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result DeleteMetadataMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string key);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SetLockedMethod(global::System.IntPtr methodsPtr, bool locked);

			internal global::Discord.LobbyTransaction.FFIMethods.SetTypeMethod SetType;

			internal global::Discord.LobbyTransaction.FFIMethods.SetOwnerMethod SetOwner;

			internal global::Discord.LobbyTransaction.FFIMethods.SetCapacityMethod SetCapacity;

			internal global::Discord.LobbyTransaction.FFIMethods.SetMetadataMethod SetMetadata;

			internal global::Discord.LobbyTransaction.FFIMethods.DeleteMetadataMethod DeleteMetadata;

			internal global::Discord.LobbyTransaction.FFIMethods.SetLockedMethod SetLocked;
		}

		internal global::System.IntPtr MethodsPtr;

		internal object MethodsStructure;

		private global::Discord.LobbyTransaction.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.LobbyTransaction.FFIMethods));
				}
				return (global::Discord.LobbyTransaction.FFIMethods)MethodsStructure;
			}
		}

		public void SetType(global::Discord.LobbyType type)
		{
			if (MethodsPtr != global::System.IntPtr.Zero)
			{
				global::Discord.Result result = Methods.SetType(MethodsPtr, type);
				if (result != global::Discord.Result.Ok)
				{
					throw new global::Discord.ResultException(result);
				}
			}
		}

		public void SetOwner(long ownerId)
		{
			if (MethodsPtr != global::System.IntPtr.Zero)
			{
				global::Discord.Result result = Methods.SetOwner(MethodsPtr, ownerId);
				if (result != global::Discord.Result.Ok)
				{
					throw new global::Discord.ResultException(result);
				}
			}
		}

		public void SetCapacity(uint capacity)
		{
			if (MethodsPtr != global::System.IntPtr.Zero)
			{
				global::Discord.Result result = Methods.SetCapacity(MethodsPtr, capacity);
				if (result != global::Discord.Result.Ok)
				{
					throw new global::Discord.ResultException(result);
				}
			}
		}

		public void SetMetadata(string key, string value)
		{
			if (MethodsPtr != global::System.IntPtr.Zero)
			{
				global::Discord.Result result = Methods.SetMetadata(MethodsPtr, key, value);
				if (result != global::Discord.Result.Ok)
				{
					throw new global::Discord.ResultException(result);
				}
			}
		}

		public void DeleteMetadata(string key)
		{
			if (MethodsPtr != global::System.IntPtr.Zero)
			{
				global::Discord.Result result = Methods.DeleteMetadata(MethodsPtr, key);
				if (result != global::Discord.Result.Ok)
				{
					throw new global::Discord.ResultException(result);
				}
			}
		}

		public void SetLocked(bool locked)
		{
			if (MethodsPtr != global::System.IntPtr.Zero)
			{
				global::Discord.Result result = Methods.SetLocked(MethodsPtr, locked);
				if (result != global::Discord.Result.Ok)
				{
					throw new global::Discord.ResultException(result);
				}
			}
		}
	}
}
