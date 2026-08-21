namespace Discord
{
	public class StorageManager
	{
		internal struct FFIEvents
		{
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result ReadMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string name, byte[] data, int dataLen, ref uint read);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ReadAsyncCallback(global::System.IntPtr ptr, global::Discord.Result result, global::System.IntPtr dataPtr, int dataLen);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ReadAsyncMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string name, global::System.IntPtr callbackData, global::Discord.StorageManager.FFIMethods.ReadAsyncCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ReadAsyncPartialCallback(global::System.IntPtr ptr, global::Discord.Result result, global::System.IntPtr dataPtr, int dataLen);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ReadAsyncPartialMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string name, ulong offset, ulong length, global::System.IntPtr callbackData, global::Discord.StorageManager.FFIMethods.ReadAsyncPartialCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result WriteMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string name, byte[] data, int dataLen);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void WriteAsyncCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void WriteAsyncMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string name, byte[] data, int dataLen, global::System.IntPtr callbackData, global::Discord.StorageManager.FFIMethods.WriteAsyncCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result DeleteMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string name);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result ExistsMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string name, ref bool exists);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void CountMethod(global::System.IntPtr methodsPtr, ref int count);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result StatMethod(global::System.IntPtr methodsPtr, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] string name, ref global::Discord.FileStat stat);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result StatAtMethod(global::System.IntPtr methodsPtr, int index, ref global::Discord.FileStat stat);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetPathMethod(global::System.IntPtr methodsPtr, global::System.Text.StringBuilder path);

			internal global::Discord.StorageManager.FFIMethods.ReadMethod Read;

			internal global::Discord.StorageManager.FFIMethods.ReadAsyncMethod ReadAsync;

			internal global::Discord.StorageManager.FFIMethods.ReadAsyncPartialMethod ReadAsyncPartial;

			internal global::Discord.StorageManager.FFIMethods.WriteMethod Write;

			internal global::Discord.StorageManager.FFIMethods.WriteAsyncMethod WriteAsync;

			internal global::Discord.StorageManager.FFIMethods.DeleteMethod Delete;

			internal global::Discord.StorageManager.FFIMethods.ExistsMethod Exists;

			internal global::Discord.StorageManager.FFIMethods.CountMethod Count;

			internal global::Discord.StorageManager.FFIMethods.StatMethod Stat;

			internal global::Discord.StorageManager.FFIMethods.StatAtMethod StatAt;

			internal global::Discord.StorageManager.FFIMethods.GetPathMethod GetPath;
		}

		public delegate void ReadAsyncHandler(global::Discord.Result result, byte[] data);

		public delegate void ReadAsyncPartialHandler(global::Discord.Result result, byte[] data);

		public delegate void WriteAsyncHandler(global::Discord.Result result);

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.StorageManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.StorageManager.FFIMethods));
				}
				return (global::Discord.StorageManager.FFIMethods)MethodsStructure;
			}
		}

		internal StorageManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.StorageManager.FFIEvents events)
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

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.StorageManager.FFIEvents events)
		{
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		public uint Read(string name, byte[] data)
		{
			uint read = 0u;
			global::Discord.Result result = Methods.Read(MethodsPtr, name, data, data.Length, ref read);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return read;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void ReadAsyncCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result, global::System.IntPtr dataPtr, int dataLen)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.StorageManager.ReadAsyncHandler obj = (global::Discord.StorageManager.ReadAsyncHandler)gCHandle.Target;
			gCHandle.Free();
			byte[] array = new byte[dataLen];
			global::System.Runtime.InteropServices.Marshal.Copy(dataPtr, array, 0, dataLen);
			obj(result, array);
		}

		public void ReadAsync(string name, global::Discord.StorageManager.ReadAsyncHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.ReadAsync(MethodsPtr, name, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), ReadAsyncCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void ReadAsyncPartialCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result, global::System.IntPtr dataPtr, int dataLen)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.StorageManager.ReadAsyncPartialHandler obj = (global::Discord.StorageManager.ReadAsyncPartialHandler)gCHandle.Target;
			gCHandle.Free();
			byte[] array = new byte[dataLen];
			global::System.Runtime.InteropServices.Marshal.Copy(dataPtr, array, 0, dataLen);
			obj(result, array);
		}

		public void ReadAsyncPartial(string name, ulong offset, ulong length, global::Discord.StorageManager.ReadAsyncPartialHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.ReadAsyncPartial(MethodsPtr, name, offset, length, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), ReadAsyncPartialCallbackImpl);
		}

		public void Write(string name, byte[] data)
		{
			global::Discord.Result result = Methods.Write(MethodsPtr, name, data, data.Length);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void WriteAsyncCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.StorageManager.WriteAsyncHandler obj = (global::Discord.StorageManager.WriteAsyncHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void WriteAsync(string name, byte[] data, global::Discord.StorageManager.WriteAsyncHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.WriteAsync(MethodsPtr, name, data, data.Length, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), WriteAsyncCallbackImpl);
		}

		public void Delete(string name)
		{
			global::Discord.Result result = Methods.Delete(MethodsPtr, name);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public bool Exists(string name)
		{
			bool exists = false;
			global::Discord.Result result = Methods.Exists(MethodsPtr, name, ref exists);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return exists;
		}

		public int Count()
		{
			int count = 0;
			Methods.Count(MethodsPtr, ref count);
			return count;
		}

		public global::Discord.FileStat Stat(string name)
		{
			global::Discord.FileStat stat = default(global::Discord.FileStat);
			global::Discord.Result result = Methods.Stat(MethodsPtr, name, ref stat);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return stat;
		}

		public global::Discord.FileStat StatAt(int index)
		{
			global::Discord.FileStat stat = default(global::Discord.FileStat);
			global::Discord.Result result = Methods.StatAt(MethodsPtr, index, ref stat);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return stat;
		}

		public string GetPath()
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(4096);
			global::Discord.Result result = Methods.GetPath(MethodsPtr, stringBuilder);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return stringBuilder.ToString();
		}

		public global::System.Collections.Generic.IEnumerable<global::Discord.FileStat> Files()
		{
			int num = Count();
			global::System.Collections.Generic.List<global::Discord.FileStat> list = new global::System.Collections.Generic.List<global::Discord.FileStat>();
			for (int i = 0; i < num; i++)
			{
				list.Add(StatAt(i));
			}
			return list;
		}
	}
}
