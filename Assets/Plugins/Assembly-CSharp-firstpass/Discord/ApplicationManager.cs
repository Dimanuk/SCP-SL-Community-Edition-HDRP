namespace Discord
{
	public class ApplicationManager
	{
		internal struct FFIEvents
		{
		}

		internal struct FFIMethods
		{
			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ValidateOrExitCallback(global::System.IntPtr ptr, global::Discord.Result result);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void ValidateOrExitMethod(global::System.IntPtr methodsPtr, global::System.IntPtr callbackData, global::Discord.ApplicationManager.FFIMethods.ValidateOrExitCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void GetCurrentLocaleMethod(global::System.IntPtr methodsPtr, global::System.Text.StringBuilder locale);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void GetCurrentBranchMethod(global::System.IntPtr methodsPtr, global::System.Text.StringBuilder branch);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void GetOAuth2TokenCallback(global::System.IntPtr ptr, global::Discord.Result result, ref global::Discord.OAuth2Token oauth2Token);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void GetOAuth2TokenMethod(global::System.IntPtr methodsPtr, global::System.IntPtr callbackData, global::Discord.ApplicationManager.FFIMethods.GetOAuth2TokenCallback callback);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void GetTicketCallback(global::System.IntPtr ptr, global::Discord.Result result, [global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)] ref string data);

			[global::System.Runtime.InteropServices.UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
			internal delegate void GetTicketMethod(global::System.IntPtr methodsPtr, global::System.IntPtr callbackData, global::Discord.ApplicationManager.FFIMethods.GetTicketCallback callback);

			internal global::Discord.ApplicationManager.FFIMethods.ValidateOrExitMethod ValidateOrExit;

			internal global::Discord.ApplicationManager.FFIMethods.GetCurrentLocaleMethod GetCurrentLocale;

			internal global::Discord.ApplicationManager.FFIMethods.GetCurrentBranchMethod GetCurrentBranch;

			internal global::Discord.ApplicationManager.FFIMethods.GetOAuth2TokenMethod GetOAuth2Token;

			internal global::Discord.ApplicationManager.FFIMethods.GetTicketMethod GetTicket;
		}

		public delegate void ValidateOrExitHandler(global::Discord.Result result);

		public delegate void GetOAuth2TokenHandler(global::Discord.Result result, ref global::Discord.OAuth2Token oauth2Token);

		public delegate void GetTicketHandler(global::Discord.Result result, ref string data);

		private global::System.IntPtr MethodsPtr;

		private object MethodsStructure;

		private global::Discord.ApplicationManager.FFIMethods Methods
		{
			get
			{
				if (MethodsStructure == null)
				{
					MethodsStructure = global::System.Runtime.InteropServices.Marshal.PtrToStructure(MethodsPtr, typeof(global::Discord.ApplicationManager.FFIMethods));
				}
				return (global::Discord.ApplicationManager.FFIMethods)MethodsStructure;
			}
		}

		internal ApplicationManager(global::System.IntPtr ptr, global::System.IntPtr eventsPtr, ref global::Discord.ApplicationManager.FFIEvents events)
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

		private void InitEvents(global::System.IntPtr eventsPtr, ref global::Discord.ApplicationManager.FFIEvents events)
		{
			global::System.Runtime.InteropServices.Marshal.StructureToPtr(events, eventsPtr, fDeleteOld: false);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void ValidateOrExitCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.ApplicationManager.ValidateOrExitHandler obj = (global::Discord.ApplicationManager.ValidateOrExitHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result);
		}

		public void ValidateOrExit(global::Discord.ApplicationManager.ValidateOrExitHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.ValidateOrExit(MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), ValidateOrExitCallbackImpl);
		}

		public string GetCurrentLocale()
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(128);
			Methods.GetCurrentLocale(MethodsPtr, stringBuilder);
			return stringBuilder.ToString();
		}

		public string GetCurrentBranch()
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(4096);
			Methods.GetCurrentBranch(MethodsPtr, stringBuilder);
			return stringBuilder.ToString();
		}

		[global::Discord.MonoPInvokeCallback]
		private static void GetOAuth2TokenCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result, ref global::Discord.OAuth2Token oauth2Token)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.ApplicationManager.GetOAuth2TokenHandler obj = (global::Discord.ApplicationManager.GetOAuth2TokenHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result, ref oauth2Token);
		}

		public void GetOAuth2Token(global::Discord.ApplicationManager.GetOAuth2TokenHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.GetOAuth2Token(MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), GetOAuth2TokenCallbackImpl);
		}

		[global::Discord.MonoPInvokeCallback]
		private static void GetTicketCallbackImpl(global::System.IntPtr ptr, global::Discord.Result result, ref string data)
		{
			global::System.Runtime.InteropServices.GCHandle gCHandle = global::System.Runtime.InteropServices.GCHandle.FromIntPtr(ptr);
			global::Discord.ApplicationManager.GetTicketHandler obj = (global::Discord.ApplicationManager.GetTicketHandler)gCHandle.Target;
			gCHandle.Free();
			obj(result, ref data);
		}

		public void GetTicket(global::Discord.ApplicationManager.GetTicketHandler callback)
		{
			global::System.Runtime.InteropServices.GCHandle value = global::System.Runtime.InteropServices.GCHandle.Alloc(callback);
			Methods.GetTicket(MethodsPtr, global::System.Runtime.InteropServices.GCHandle.ToIntPtr(value), GetTicketCallbackImpl);
		}
	}
}
