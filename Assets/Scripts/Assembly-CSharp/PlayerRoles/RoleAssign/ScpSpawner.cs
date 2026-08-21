using System.Collections.Generic;
using PlayerRoles.PlayableScps;
using UnityEngine;

namespace PlayerRoles.RoleAssign
{
    public static class ScpSpawner
    {
        private static readonly Dictionary<ReferenceHub, float> SelectedSpawnChances = new Dictionary<ReferenceHub, float>();

        private static readonly Dictionary<ReferenceHub, float> ChancesBuffer = new Dictionary<ReferenceHub, float>();

        private static readonly List<RoleTypeId> BackupScps = new List<RoleTypeId>(8);

        private static readonly List<RoleTypeId> EnqueuedScps = new List<RoleTypeId>(8);

        private static PlayerRoleBase[] _cachedSpawnableScps;

        private static float[] _chancesArray;

        private static bool _cacheSet;

        private static PlayerRoleBase[] SpawnableScps
        {
            get
            {
                if (_cacheSet)
                {
                    return _cachedSpawnableScps;
                }
                List<PlayerRoleBase> list = new List<PlayerRoleBase>();
                foreach (KeyValuePair<RoleTypeId, PlayerRoleBase> allRole in PlayerRoleLoader.AllRoles)
                {
                    if (allRole.Value is ISpawnableScp)
                    {
                        list.Add(allRole.Value);
                    }
                }
                _cacheSet = true;
                _chancesArray = new float[list.Count];
                return _cachedSpawnableScps = list.ToArray();
            }
        }

        private static RoleTypeId NextScp
        {
            get
            {
                float num = 0f;
                int num2 = SpawnableScps.Length;
                for (int i = 0; i < num2; i++)
                {
                    PlayerRoleBase playerRoleBase = SpawnableScps[i];
                    if (EnqueuedScps.Contains(playerRoleBase.RoleTypeId))
                    {
                        _chancesArray[i] = 0f;
                        continue;
                    }
                    float spawnChance = (SpawnableScps[i] as ISpawnableScp).GetSpawnChance(EnqueuedScps);
                    spawnChance = Mathf.Max(spawnChance, 0f);
                    num += spawnChance;
                    _chancesArray[i] = spawnChance;
                }
                if (num == 0f)
                {
                    return RandomLeastFrequentScp;
                }
                float num3 = Random.Range(0f, num);
                for (int j = 0; j < num2; j++)
                {
                    num3 -= _chancesArray[j];
                    if (!(num3 >= 0f))
                    {
                        return SpawnableScps[j].RoleTypeId;
                    }
                }
                return SpawnableScps[num2 - 1].RoleTypeId;
            }
        }


        private static RoleTypeId RandomLeastFrequentScp
        {
            get
            {
                int num = SpawnableScps.Length;
                int num2 = int.MaxValue;
                for (int i = 0; i < num; i++)
                {
                    RoleTypeId roleTypeId = SpawnableScps[i].RoleTypeId;
                    int num3 = 0;
                    foreach (RoleTypeId enqueuedScp in EnqueuedScps)
                    {
                        if (enqueuedScp == roleTypeId)
                        {
                            num3++;
                        }
                    }
                    if (num3 <= num2)
                    {
                        if (num3 < num2)
                        {
                            BackupScps.Clear();
                        }
                        BackupScps.Add(roleTypeId);
                        num2 = num3;
                    }
                }
                return BackupScps.RandomItem();
            }
        }

        public static void SpawnScps(int targetScpNumber)
        {
            EnqueuedScps.Clear();
            for (int i = 0; i < targetScpNumber; i++)
            {
                EnqueuedScps.Add(NextScp);
            }
            List<ReferenceHub> chosenPlayers = ScpPlayerPicker.ChoosePlayers(targetScpNumber);
            while (EnqueuedScps.Count > 0)
            {
                RoleTypeId scp = EnqueuedScps[0];
                EnqueuedScps.RemoveAt(0);
                if (scp == RoleTypeId.None) continue;
                AssignScp(chosenPlayers, scp, EnqueuedScps);
            }
        }

        private static void AssignScp(List<ReferenceHub> chosenPlayers, RoleTypeId scp, List<RoleTypeId> otherScps)
        {
            ChancesBuffer.Clear();
            int num = 1;
            int num2 = 0;
            foreach (ReferenceHub chosenPlayer in chosenPlayers)
            {
                int num3 = GetPreferenceOfPlayer(chosenPlayer, scp);
                foreach (RoleTypeId otherScp in otherScps)
                {
                    num3 -= GetPreferenceOfPlayer(chosenPlayer, otherScp);
                }
                num2++;
                ChancesBuffer[chosenPlayer] = num3;
                num = Mathf.Min(num3, num);
            }
            float num4 = 0f;
            SelectedSpawnChances.Clear();
            foreach (KeyValuePair<ReferenceHub, float> item in ChancesBuffer)
            {
                float num5 = Mathf.Pow(item.Value - (float)num + 1f, num2);
                SelectedSpawnChances[item.Key] = num5;
                num4 += num5;
            }
            float num6 = num4 * Random.value;
            float num7 = 0f;
            foreach (KeyValuePair<ReferenceHub, float> selectedSpawnChance in SelectedSpawnChances)
            {
                num7 += selectedSpawnChance.Value;
                if (!(num7 < num6))
                {
                    ReferenceHub key = selectedSpawnChance.Key;
                    chosenPlayers.Remove(key);
                    key.roleManager.ServerSetRole(scp, RoleChangeReason.RoundStart);
                    break;
                }
            }
        }

        private static int GetPreferenceOfPlayer(ReferenceHub ply, RoleTypeId scp)
        {
            if (!ScpSpawnPreferences.Preferences.TryGetValue(ply.connectionToClient, out var value))
            {
                return 0;
            }
            if (!value.Preferences.TryGetValue(scp, out var value2))
            {
                return 0;
            }
            return value2;
        }
    }
}
