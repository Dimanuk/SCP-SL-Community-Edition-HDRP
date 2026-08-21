using GameCore;
using Mirror;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using Console = GameCore.Console;
using Debug = UnityEngine.Debug;

namespace MapGeneration
{
    public class SeedSynchronizer : NetworkBehaviour
    {
        private const string SeedConfigKey = "map_seed";
        private const string DebugLogChannel = "MAPGEN";
        private const string WarningLogFormat = "<color=orange>Warning:</color> {0}";
        private const string ErrorLogFormat = "<color=red>Fatal Error:</color> {0}";

        private static readonly string[] MapAliases = new string[3] { "LCZ", "HCZ", "EZ" };

        public static bool MapGenerated;

        [SyncVar]
        private int _syncSeed;

        private static SeedSynchronizer _singleton;
        private static readonly Stopwatch _stopwatch = new Stopwatch();

        public static float TimeSinceMapGeneration => (float)_stopwatch.Elapsed.TotalSeconds;

        public static int Seed
        {
            get
            {
                if (!(_singleton == null))
                {
                    return _singleton._syncSeed;
                }
                return 0;
            }
        }

        public static event Action OnMapGenerated = delegate { };

        private void Awake()
        {
            _singleton = this;
        }

        private void OnDestroy()
        {
            MapGenerated = false;
        }

        private void Start()
        {
            if (NetworkServer.active)
            {
                int num = ConfigFile.ServerConfig.GetInt("map_seed", -1);
                int randomSeed = UnityEngine.Random.Range(1, int.MaxValue);

                DebugInfo("Server has successfully generated a random seed: " + randomSeed, MessageImportance.Normal);

                if (num < 1)
                {
                    num = randomSeed;
                }
                else
                {
                    DebugInfo("Server has successfully loaded a seed from config: " + num, MessageImportance.Normal);
                }

                _syncSeed = Mathf.Clamp(num, 1, int.MaxValue);
            }
        }

        private void Update()
        {
            if (Seed <= 0 || MapGenerated || ReferenceHub.LocalHub == null)
            {
                return;
            }

            GenerateLevel();

            HashSet<RoomIdentifier> hashSet = new HashSet<RoomIdentifier>();
            foreach (RoomIdentifier allRoomIdentifier in RoomIdentifier.AllRoomIdentifiers)
            {
                if (allRoomIdentifier == null || !allRoomIdentifier.TryAssignId())
                {
                    hashSet.Add(allRoomIdentifier);
                }
            }
            foreach (RoomIdentifier item in hashSet)
            {
                RoomIdentifier.AllRoomIdentifiers.Remove(item);
            }

            try
            {
                OnMapGenerated();
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to call the OnMapGenerated event, error: " + ex.Message);
                Debug.LogError("List of methods that can cause this issue:");
                Delegate[] invocationList = OnMapGenerated.GetInvocationList();
                foreach (Delegate obj in invocationList)
                {
                    try
                    {
                        obj.Method.Invoke(obj.Target, null);
                    }
                    catch
                    {
                        Debug.LogError("- " + obj.Method.Name);
                    }
                }
            }

            MapGenerated = true;
            _stopwatch.Restart();
        }

        private void GenerateLevel()
        {
            for (int i = 0; i < ImageGenerator.ZoneGenerators.Length; i++)
            {
                string text = MapAliases[i];
                if (ImageGenerator.ZoneGenerators[i].GenerateMap(_syncSeed - i, text, out var blackbox))
                {
                    DebugInfo(text + " generator tasks completed, no fatal errors to report.", MessageImportance.LessImportant);
                }
                else
                {
                    DebugError(isFatal: true, text + " generator tasks failed, blackbox message: " + blackbox);
                }
            }
            DebugInfo("Sequence of procedural level generation completed.", MessageImportance.Normal);

            if (NetworkServer.active)
            {
                DoorSpawnpoint.SetupAllDoors();
            }
        }

        internal static void DebugInfo(string txt, MessageImportance importance, bool nospace = false)
        {
            Console.AddDebugLog("MAPGEN", txt, importance, nospace);
        }

        internal static void DebugError(bool isFatal, string txt)
        {
            DebugInfo(string.Format(isFatal ? ErrorLogFormat : WarningLogFormat, txt), MessageImportance.MostImportant);
            Debug.LogError("Map generation error for seed " + Seed + ": " + txt);
        }
    }
}