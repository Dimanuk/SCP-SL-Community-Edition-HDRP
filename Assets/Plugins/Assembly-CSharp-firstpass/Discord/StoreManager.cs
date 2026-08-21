namespace Discord
{
	public class StoreManager
	{
		internal struct FFIEvents
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void EntitlementCreateHandler(global::System.IntPtr ptr, ref global::Discord.Entitlement entitlement);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void EntitlementDeleteHandler(global::System.IntPtr ptr, ref global::Discord.Entitlement entitlement);

			internal global::Discord.StoreManager.FFIEvents.EntitlementCreateHandler OnEntitlementCreate;

			internal global::Discord.StoreManager.FFIEvents.EntitlementDeleteHandler OnEntitlementDelete;
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void FetchSkusCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void FetchSkusMethod(global::System.IntPtr methodsPtr, global::System.IntPtr callbackData, global::Discord.StoreManager.FFIMethods.FetchSkusCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void CountSkusMethod(global::System.IntPtr methodsPtr, ref int count);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetSkuMethod(global::System.IntPtr methodsPtr, long skuId, ref global::Discord.Sku sku);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetSkuAtMethod(global::System.IntPtr methodsPtr, int index, ref global::Discord.Sku sku);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void FetchEntitlementsCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void FetchEntitlementsMethod(global::System.IntPtr methodsPtr, global::System.IntPtr callbackData, global::Discord.StoreManager.FFIMethods.FetchEntitlementsCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void CountEntitlementsMethod(global::System.IntPtr methodsPtr, ref int count);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetEntitlementMethod(global::System.IntPtr methodsPtr, long entitlementId, ref global::Discord.Entitlement entitlement);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result GetEntitlementAtMethod(global::System.IntPtr methodsPtr, int index, ref global::Discord.Entitlement entitlement);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate global::Discord.Result HasSkuEntitlementMethod(global::System.IntPtr methodsPtr, long skuId, ref bool hasEntitlement);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void StartPurchaseCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void StartPurchaseMethod(global::System.IntPtr methodsPtr, long skuId, global::System.IntPtr callbackData, global::Discord.StoreManager.FFIMethods.StartPurchaseCallback callback);

			internal global::Discord.StoreManager.FFIMethods.FetchSkusMethod FetchSkus;

			internal global::Discord.StoreManager.FFIMethods.CountSkusMethod CountSkus;

			internal global::Discord.StoreManager.FFIMethods.GetSkuMethod GetSku;

			internal global::Discord.StoreManager.FFIMethods.GetSkuAtMethod GetSkuAt;

			internal global::Discord.StoreManager.FFIMethods.FetchEntitlementsMethod FetchEntitlements;

			internal global::Discord.StoreManager.FFIMethods.CountEntitlementsMethod CountEntitlements;

			internal global::Discord.StoreManager.FFIMethods.GetEntitlementMethod GetEntitlement;

			internal global::Discord.StoreManager.FFIMethods.GetEntitlementAtMethod GetEntitlementAt;

			internal global::Discord.StoreManager.FFIMethods.HasSkuEntitlementMethod HasSkuEntitlement;

			internal global::Discord.StoreManager.FFIMethods.StartPurchaseMethod StartPurchase;
		}

		public delegate void FetchSkusHandler(global::Discord.Result result);

		public delegate void FetchEntitlementsHandler(global::Discord.Result result);

		public delegate void StartPurchaseHandler(global::Discord.Result result);

		public delegate void EntitlementCreateHandler(ref global::Discord.Entitlement entitlement);

		public delegate void EntitlementDeleteHandler(ref global::Discord.Entitlement entitlement);

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.StoreManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.StoreManager.FFIMethods));
				}
				return (global::Discord.StoreManager.FFIMethods)MethodsStructure;
			}
		}

		public event global::Discord.StoreManager.EntitlementCreateHandler OnEntitlementCreate;

		public event global::Discord.StoreManager.EntitlementDeleteHandler OnEntitlementDelete;

		internal StoreManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.StoreManager.FFIEvents events)
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

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.StoreManager.FFIEvents events)
		{
			events.OnEntitlementCreate = OnEntitlementCreateImpl;
			events.OnEntitlementDelete = OnEntitlementDeleteImpl;
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void FetchSkusCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.StoreManager.FetchSkusHandler obj = (global::Discord.StoreManager.FetchSkusHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void FetchSkus(global::Discord.StoreManager.FetchSkusHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.FetchSkus(MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), FetchSkusCallbackImpl);
		}

		public int CountSkus()
		{
			int count = 0;
			Methods.CountSkus(MethodsPtr, ref count);
			return count;
		}

		public global::Discord.Sku GetSku(long skuId)
		{
			global::Discord.Sku sku = default(global::Discord.Sku);
			global::Discord.Result result = Methods.GetSku(MethodsPtr, skuId, ref sku);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return sku;
		}

		public global::Discord.Sku GetSkuAt(int index)
		{
			global::Discord.Sku sku = default(global::Discord.Sku);
			global::Discord.Result result = Methods.GetSkuAt(MethodsPtr, index, ref sku);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return sku;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void FetchEntitlementsCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.StoreManager.FetchEntitlementsHandler obj = (global::Discord.StoreManager.FetchEntitlementsHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void FetchEntitlements(global::Discord.StoreManager.FetchEntitlementsHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.FetchEntitlements(MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), FetchEntitlementsCallbackImpl);
		}

		public int CountEntitlements()
		{
			int count = 0;
			Methods.CountEntitlements(MethodsPtr, ref count);
			return count;
		}

		public global::Discord.Entitlement GetEntitlement(long entitlementId)
		{
			global::Discord.Entitlement entitlement = default(global::Discord.Entitlement);
			global::Discord.Result result = Methods.GetEntitlement(MethodsPtr, entitlementId, ref entitlement);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return entitlement;
		}

		public global::Discord.Entitlement GetEntitlementAt(int index)
		{
			global::Discord.Entitlement entitlement = default(global::Discord.Entitlement);
			global::Discord.Result result = Methods.GetEntitlementAt(MethodsPtr, index, ref entitlement);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return entitlement;
		}

		public bool HasSkuEntitlement(long skuId)
		{
			bool hasEntitlement = false;
			global::Discord.Result result = Methods.HasSkuEntitlement(MethodsPtr, skuId, ref hasEntitlement);
			if (result != global::Discord.Result.Ok)
			{
				throw new global::Discord.ResultException(result);
			}
			return hasEntitlement;
		}

		[global::Discord.MonoPInvokeCallback]
		private static void StartPurchaseCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.StoreManager.StartPurchaseHandler obj = (global::Discord.StoreManager.StartPurchaseHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void StartPurchase(long skuId, global::Discord.StoreManager.StartPurchaseHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.StartPurchase(MethodsPtr, skuId, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), StartPurchaseCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnEntitlementCreateImpl(global::System.IntPtr ptr, ref global::Discord.Entitlement entitlement)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.StoreManagerInstance.OnEntitlementCreate != null)
			{
				discord.StoreManagerInstance.OnEntitlementCreate(ref entitlement);
			}
		}

		[global::Discord.MonoPInvokeCallback]
		private static void OnEntitlementDeleteImpl(global::System.IntPtr ptr, ref global::Discord.Entitlement entitlement)
		{
			global::Discord.Discord discord = (global::Discord.Discord)global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr).Target;
			if (discord.StoreManagerInstance.OnEntitlementDelete != null)
			{
				discord.StoreManagerInstance.OnEntitlementDelete(ref entitlement);
			}
		}

		public global::System.Collections.Generic.IEnumerable<global::Discord.Entitlement> GetEntitlements()
		{
			int num = CountEntitlements();
			global::System.Collections.Generic.List<global::Discord.Entitlement> list = new global::System.Collections.Generic.List<global::Discord.Entitlement>();
			for (int i = 0; i < num; i++)
			{
				list.Add(GetEntitlementAt(i));
			}
			return list;
		}

		public global::System.Collections.Generic.IEnumerable<global::Discord.Sku> GetSkus()
		{
			int num = CountSkus();
			global::System.Collections.Generic.List<global::Discord.Sku> list = new global::System.Collections.Generic.List<global::Discord.Sku>();
			for (int i = 0; i < num; i++)
			{
				list.Add(GetSkuAt(i));
			}
			return list;
		}
	}
}
