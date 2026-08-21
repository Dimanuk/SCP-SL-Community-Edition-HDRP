namespace Discord
{
	public class ImageManager
	{
		internal struct FFIEvents
		{
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void FetchCallback(global::System.IntPtr ptr, global::Discord.Result result, global::Discord.ImageHandle handleResult);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void FetchMethod(global::System.IntPtr methodsPtr, global::Discord.ImageHandle handle, bool refresh, global::System.IntPtr callbackData, global::Discord.ImageManager.FFIMethods.FetchCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetDimensionsMethod(global::System.IntPtr methodsPtr, global::Discord.ImageHandle handle, ref global::Discord.ImageDimensions dimensions);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetDataMethod(global::System.IntPtr methodsPtr, global::Discord.ImageHandle handle, byte[] data, int dataLen);

			internal global::Discord.ImageManager.FFIMethods.FetchMethod Fetch;

			internal global::Discord.ImageManager.FFIMethods.GetDimensionsMethod GetDimensions;

			internal global::Discord.ImageManager.FFIMethods.GetDataMethod GetData;
		}

		public delegate void FetchHandler(global::Discord.Result result, global::Discord.ImageHandle handleResult);

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.ImageManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.ImageManager.FFIMethods));
				}
				return (global::Discord.ImageManager.FFIMethods)MethodsStructure;
			}
		}

		internal ImageManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.ImageManager.FFIEvents events)
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

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.ImageManager.FFIEvents events)
		{
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void FetchCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result, global::Discord.ImageHandle handleResult)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.ImageManager.FetchHandler obj = (global::Discord.ImageManager.FetchHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result, handleResult);
		}

		public void Fetch(global::Discord.ImageHandle handle, bool refresh, global::Discord.ImageManager.FetchHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.Fetch(MethodsPtr, handle, refresh, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), FetchCallbackImpl);
		}

		public global::Discord.ImageDimensions GetDimensions(global::Discord.ImageHandle handle)
		{
			global::Discord.ImageDimensions dimensions = default(global::Discord.ImageDimensions);
			global::Discord.Result result = Methods.GetDimensions(MethodsPtr, handle, ref dimensions);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return dimensions;
		}

		public void GetData(global::Discord.ImageHandle handle, byte[] data)
		{
			global::Discord.Result result = Methods.GetData(MethodsPtr, handle, data, data.Length);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
		}

		public void Fetch(global::Discord.ImageHandle handle, global::Discord.ImageManager.FetchHandler callback)
		{
			Fetch(handle, refresh: false, callback);
		}

		public byte[] GetData(global::Discord.ImageHandle handle)
		{
			global::Discord.ImageDimensions dimensions = GetDimensions(handle);
			byte[] array = new byte[dimensions.Width * dimensions.Height * 4];
			GetData(handle, array);
			return array;
		}

		public global::UnityEngine.Texture2D GetTexture(global::Discord.ImageHandle handle)
		{
			global::Discord.ImageDimensions dimensions = GetDimensions(handle);
			global::UnityEngine.Texture2D texture2D = new global::UnityEngine.Texture2D((int)dimensions.Width, (int)dimensions.Height, global::UnityEngine.TextureFormat.RGBA32, mipChain: false, linear: true);
			texture2D.LoadRawTextureData(GetData(handle));
			texture2D.Apply();
			return texture2D;
		}
	}
}
