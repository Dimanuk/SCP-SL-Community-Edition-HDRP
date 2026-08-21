namespace Discord
{
	public class RelationshipManager
	{
		internal struct FFIEvents
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void RefreshHandler(global::System.IntPtr ptr);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void RelationshipUpdateHandler(global::System.IntPtr ptr, ref global::Discord.Relationship relationship);

			internal global::Discord.RelationshipManager.FFIEvents.RefreshHandler OnRefresh;

			internal global::Discord.RelationshipManager.FFIEvents.RelationshipUpdateHandler OnRelationshipUpdate;
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate bool FilterCallback(global::System.IntPtr ptr, ref global::Discord.Relationship relationship);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void FilterMethod(global::System.IntPtr methodsPtr, global::System.IntPtr callbackData, global::Discord.RelationshipManager.FFIMethods.FilterCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result CountMethod(global::System.IntPtr methodsPtr, ref int count);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetMethod(global::System.IntPtr methodsPtr, long userId, ref global::Discord.Relationship relationship);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetAtMethod(global::System.IntPtr methodsPtr, uint index, ref global::Discord.Relationship relationship);

			internal global::Discord.RelationshipManager.FFIMethods.FilterMethod Filter;

			internal global::Discord.RelationshipManager.FFIMethods.CountMethod Count;

			internal global::Discord.RelationshipManager.FFIMethods.GetMethod Get;

			internal global::Discord.RelationshipManager.FFIMethods.GetAtMethod GetAt;
		}

		public delegate bool FilterHandler(ref global::Discord.Relationship relationship);

		public delegate void RefreshHandler();

		public delegate void RelationshipUpdateHandler(ref global::Discord.Relationship relationship);

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.RelationshipManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.RelationshipManager.FFIMethods));
				}
				return (global::Discord.RelationshipManager.FFIMethods)MethodsStructure;
			}
		}

		public event global::Discord.RelationshipManager.RefreshHandler OnRefresh;

		public event global::Discord.RelationshipManager.RelationshipUpdateHandler OnRelationshipUpdate;

		internal RelationshipManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.RelationshipManager.FFIEvents events)
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

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.RelationshipManager.FFIEvents events)
		{
			events.OnRefresh = OnRefreshImpl;
			events.OnRelationshipUpdate = OnRelationshipUpdateImpl;
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		[global::Discord.MonoPInvokeCallback]
		private static bool FilterCallbackImpl(global::System.IntPtr ptr, ref global::Discord.Relationship relationship)
		{
			return ((global::Discord.RelationshipManager.FilterHandler)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target)(ref relationship);
		}

		public void Filter(global::Discord.RelationshipManager.FilterHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.Filter(MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), FilterCallbackImpl);
			value.Free();
		}

		public int Count()
		{
			int count = 0;
			global::Discord.Result result = Methods.Count(MethodsPtr, ref count);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return count;
		}

		public global::Discord.Relationship Get(long userId)
		{
			global::Discord.Relationship relationship = default(global::Discord.Relationship);
			global::Discord.Result result = Methods.Get(MethodsPtr, userId, ref relationship);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return relationship;
		}

		public global::Discord.Relationship GetAt(uint index)
		{
			global::Discord.Relationship relationship = default(global::Discord.Relationship);
			global::Discord.Result result = Methods.GetAt(MethodsPtr, index, ref relationship);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return relationship;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnRefreshImpl(global::System.IntPtr ptr)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.RelationshipManagerInstance.OnRefresh != null)
			{
				discord.RelationshipManagerInstance.OnRefresh();
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnRelationshipUpdateImpl(global::System.IntPtr ptr, ref global::Discord.Relationship relationship)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.RelationshipManagerInstance.OnRelationshipUpdate != null)
			{
				discord.RelationshipManagerInstance.OnRelationshipUpdate(ref relationship);
			}
		}
	}
}
