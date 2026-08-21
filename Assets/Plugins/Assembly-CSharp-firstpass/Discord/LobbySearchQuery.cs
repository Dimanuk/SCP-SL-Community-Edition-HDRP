namespace Discord
{
	public struct LobbySearchQuery
	{
		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result FilterMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string key, global::Discord.LobbySearchComparison comparison, global::Discord.LobbySearchCast cast, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string value);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result SortMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string key, global::Discord.LobbySearchCast cast, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string value);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result LimitMethod(global::System.IntPtr methodsPtr, uint limit);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result DistanceMethod(global::System.IntPtr methodsPtr, global::Discord.LobbySearchDistance distance);

			internal global::Discord.LobbySearchQuery.FFIMethods.FilterMethod Filter;

			internal global::Discord.LobbySearchQuery.FFIMethods.SortMethod Sort;

			internal global::Discord.LobbySearchQuery.FFIMethods.LimitMethod Limit;

			internal global::Discord.LobbySearchQuery.FFIMethods.DistanceMethod Distance;
		}

		internal global::System.IntPtr MethodsPtr;

		internal object MethodsStructure;

		private global::Discord.LobbySearchQuery.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.LobbySearchQuery.FFIMethods));
				}
				return (global::Discord.LobbySearchQuery.FFIMethods)MethodsStructure;
			}
		}

		public void Filter(string key, global::Discord.LobbySearchComparison comparison, global::Discord.LobbySearchCast cast, string value)
		{
			if (MethodsPtr != global::System.IntPtr.Zero)
			{
				global::Discord.Result result = Methods.Filter(MethodsPtr, key, comparison, cast, value);
				if (result != global::Discord.Result.Ok)
				{
					throw new global::Discord.ResultException(result);
				}
			}
		}

		public void Sort(string key, global::Discord.LobbySearchCast cast, string value)
		{
			if (MethodsPtr != global::System.IntPtr.Zero)
			{
				global::Discord.Result result = Methods.Sort(MethodsPtr, key, cast, value);
				if (result != global::Discord.Result.Ok)
				{
					throw new global::Discord.ResultException(result);
				}
			}
		}

		public void Limit(uint limit)
		{
			if (MethodsPtr != global::System.IntPtr.Zero)
			{
				global::Discord.Result result = Methods.Limit(MethodsPtr, limit);
				if (result != global::Discord.Result.Ok)
				{
					throw new global::Discord.ResultException(result);
				}
			}
		}

		public void Distance(global::Discord.LobbySearchDistance distance)
		{
			if (MethodsPtr != global::System.IntPtr.Zero)
			{
				global::Discord.Result result = Methods.Distance(MethodsPtr, distance);
				if (result != global::Discord.Result.Ok)
				{
					throw new global::Discord.ResultException(result);
				}
			}
		}
	}
}
