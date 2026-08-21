namespace Discord
{
	public struct LobbyMemberTransaction
	{
		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SetMetadataMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string key, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string value);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result DeleteMetadataMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string key);

			internal global::Discord.LobbyMemberTransaction.FFIMethods.SetMetadataMethod SetMetadata;

			internal global::Discord.LobbyMemberTransaction.FFIMethods.DeleteMetadataMethod DeleteMetadata;
		}

		internal global::System.IntPtr MethodsPtr;

		internal object MethodsStructure;

		private global::Discord.LobbyMemberTransaction.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.LobbyMemberTransaction.FFIMethods));
				}
				return (global::Discord.LobbyMemberTransaction.FFIMethods)MethodsStructure;
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
	}
}
