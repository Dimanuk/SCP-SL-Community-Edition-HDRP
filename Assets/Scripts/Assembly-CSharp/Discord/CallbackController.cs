using System;
using System.Runtime.InteropServices;
using AOT;
using Discord.Basic;
using UnityEngine;

namespace Discord
{
    public static class CallbackController
    {
        public delegate void OnDisconnectedInfo(int errorCode, string message);
        public delegate void OnErrorInfo(int errorCode, string message);
        public delegate void OnJoinInfo(string secret);
        public delegate void OnReadyInfo(DiscordUser connectedUser);
        public delegate void OnRequestInfo(DiscordUser request);
        public delegate void OnSpectateInfo(string secret);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ReadyNative(ref DiscordUser connectedUser);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DisconnectedNative(int errorCode, [MarshalAs(UnmanagedType.LPStr)] string message);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ErrorNative(int errorCode, [MarshalAs(UnmanagedType.LPStr)] string message);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void JoinNative([MarshalAs(UnmanagedType.LPStr)] string secret);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SpectateNative([MarshalAs(UnmanagedType.LPStr)] string secret);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RequestNative(ref DiscordUser request);

        [StructLayout(LayoutKind.Sequential)]
        internal struct EventHandlers
        {
            public IntPtr readyCallback;
            public IntPtr disconnectedCallback;
            public IntPtr errorCallback;
            public IntPtr joinCallback;
            public IntPtr spectateCallback;
            public IntPtr requestCallback;
        }

        private static ReadyNative _readyNative;
        private static DisconnectedNative _disconnectedNative;
        private static ErrorNative _errorNative;
        private static JoinNative _joinNative;
        private static SpectateNative _spectateNative;
        private static RequestNative _requestNative;

        public static OnReadyInfo _onReady;
        public static OnDisconnectedInfo _onDisconnected;
        public static OnErrorInfo _onError;
        public static OnJoinInfo _onJoin;
        public static OnSpectateInfo _onSpectate;
        public static OnRequestInfo _onRequest;

        private static EventHandlers _nativeHandlers;

        public static bool IsConnected { get; private set; }

        public static void Initialize(string applicationId, bool autoRegister, string optionalSteamId)
        {
            _readyNative = ReadyCallbackNative;
            _disconnectedNative = DisconnectedCallbackNative;
            _errorNative = ErrorCallbackNative;
            _joinNative = JoinCallbackNative;
            _spectateNative = SpectateCallbackNative;
            _requestNative = RequestCallbackNative;

            _nativeHandlers = new EventHandlers
            {
                readyCallback = Marshal.GetFunctionPointerForDelegate(_readyNative),
                disconnectedCallback = Marshal.GetFunctionPointerForDelegate(_disconnectedNative),
                errorCallback = Marshal.GetFunctionPointerForDelegate(_errorNative),
                joinCallback = Marshal.GetFunctionPointerForDelegate(_joinNative),
                spectateCallback = Marshal.GetFunctionPointerForDelegate(_spectateNative),
                requestCallback = Marshal.GetFunctionPointerForDelegate(_requestNative)
            };

            InitializeInternal(applicationId, ref _nativeHandlers, autoRegister, optionalSteamId);
        }

        public static void SubscribeReady(OnReadyInfo handler) => _onReady = handler;
        public static void SubscribeDisconnected(OnDisconnectedInfo handler) => _onDisconnected = handler;
        public static void SubscribeError(OnErrorInfo handler) => _onError = handler;
        public static void SubscribeJoin(OnJoinInfo handler) => _onJoin = handler;
        public static void SubscribeSpectate(OnSpectateInfo handler) => _onSpectate = handler;
        public static void SubscribeRequest(OnRequestInfo handler) => _onRequest = handler;

        public static void UpdatePresence(RichPresence presence)
        {
            try
            {
                RichPresenceStruct presenceStruct = presence.GetStruct();
                UpdatePresenceInternal(ref presenceStruct);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public static void RunCallbacks()
        {
            try { RunCallbacksInternal(); }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        public static void Shutdown()
        {
            IsConnected = false;
            try { ShutdownInternal(); }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        public static void Respond(string userId, Reply reply)
        {
            try { RespondInternal(userId, reply); }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        [MonoPInvokeCallback(typeof(ReadyNative))]
        private static void ReadyCallbackNative(ref DiscordUser connectedUser)
        {
            IsConnected = true;
            _onReady?.Invoke(connectedUser);
        }

        [MonoPInvokeCallback(typeof(DisconnectedNative))]
        private static void DisconnectedCallbackNative(int errorCode, string message)
        {
            IsConnected = false;
            _onDisconnected?.Invoke(errorCode, message);
        }

        [MonoPInvokeCallback(typeof(ErrorNative))]
        private static void ErrorCallbackNative(int errorCode, string message)
        {
            _onError?.Invoke(errorCode, message);
        }

        [MonoPInvokeCallback(typeof(JoinNative))]
        private static void JoinCallbackNative(string secret)
        {
            _onJoin?.Invoke(secret);
        }

        [MonoPInvokeCallback(typeof(SpectateNative))]
        private static void SpectateCallbackNative(string secret)
        {
            _onSpectate?.Invoke(secret);
        }

        [MonoPInvokeCallback(typeof(RequestNative))]
        private static void RequestCallbackNative(ref DiscordUser request)
        {
            _onRequest?.Invoke(request);
        }

        private const string DLL_NAME = "discord-rpc";

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_Initialize")]
        private static extern void InitializeInternal(string applicationId, ref EventHandlers handlers, bool autoRegister, string optionalSteamId);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_Shutdown")]
        private static extern void ShutdownInternal();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_RunCallbacks")]
        private static extern void RunCallbacksInternal();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_UpdatePresence")]
        private static extern void UpdatePresenceInternal(ref RichPresenceStruct presence);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_ClearPresence")]
        private static extern void ClearPresenceInternal();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_Respond")]
        private static extern void RespondInternal(string userId, Reply reply);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_UpdateHandlers")]
        private static extern void UpdateHandlersInternal(ref EventHandlers handlers);
    }
}
