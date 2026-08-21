using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Mirror
{
    public static class MessageNameResolver
    {
        private static readonly Dictionary<ushort, MessageInfo> _hashToInfo = new();
        private static bool _initialized;

        public readonly struct MessageInfo
        {
            public readonly string FullName;
            public readonly string AssemblyName;
            public readonly bool IsRegistered;

            public MessageInfo(string fullName, string assemblyName, bool registered)
            {
                FullName = fullName;
                AssemblyName = assemblyName;
                IsRegistered = registered;
            }

            public override string ToString()
            {
                string status = IsRegistered ? "" : " [NOT REGISTERED]";
                return $"{FullName} (from {AssemblyName}){status}";
            }
        }

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string asmName = assembly.GetName().Name;
                if (asmName.StartsWith("System") || asmName.StartsWith("Unity") || asmName.StartsWith("mscorlib"))
                    continue; 

                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!type.IsValueType || type.IsEnum || type.IsAbstract)
                            continue;
                        if (!typeof(NetworkMessage).IsAssignableFrom(type))
                            continue;

                        ushort hash = type.FullName.GetStableHashCode16();

                        if (_hashToInfo.TryGetValue(hash, out MessageInfo existing))
                        {
                            Debug.LogWarning($"[MessageNameResolver] Hash collision #{hash}: {type.FullName} vs {existing.FullName}");
                            continue;
                        }

                        _hashToInfo[hash] = new MessageInfo(type.FullName, asmName, false);
                    }
                }
                catch (ReflectionTypeLoadException) { }
                catch (Exception ex)
                {
                    Debug.LogError($"[MessageNameResolver] Scan failed for {asmName}: {ex.Message}");
                }
            }

            foreach (var kvp in NetworkMessages.Lookup)
            {
                if (_hashToInfo.TryGetValue(kvp.Key, out MessageInfo info))
                {
                    _hashToInfo[kvp.Key] = new MessageInfo(info.FullName, info.AssemblyName, true);
                }
            }

            Debug.Log($"[MessageNameResolver] Indexed {_hashToInfo.Count} message types.");
        }

        public static MessageInfo Resolve(ushort msgType)
        {
            if (!_initialized) Initialize();

            if (NetworkMessages.Lookup.TryGetValue(msgType, out Type registeredType))
            {
                return new MessageInfo(
                    registeredType.FullName,
                    registeredType.Assembly.GetName().Name,
                    true
                );
            }

            if (_hashToInfo.TryGetValue(msgType, out MessageInfo info))
                return info;

            return new MessageInfo($"UnknownType_{msgType}", "???", false);
        }

        public static string GetMessageName(ushort msgType) => Resolve(msgType).ToString();
    }
}