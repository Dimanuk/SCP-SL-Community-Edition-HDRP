using System.Collections.Generic;
using GameObjectPools;
using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;

namespace PlayerRoles.PlayableScps.Scp079.Rewards
{
    public class Scp079RewardManager : SubroutineBase, IPoolResettable
    {
        private const float MarkDuration = 12f;

        private readonly Dictionary<RoomIdentifier, double> _markedRooms;

        private static int _cachedRewardSubrtId;
        private static int _cachedTierSubrtId;
        private static bool _cacheSet;

        private static double CurTime => NetworkTime.time;

        private static void RefreshCache()
        {
            if (_cacheSet)
                return;

            if (!PlayerRoleLoader.TryGetRoleTemplate<Scp079Role>(RoleTypeId.Scp079, out Scp079Role template))
                return;

            SubroutineBase[] subroutines = template.SubroutineModule.AllSubroutines;
            for (int i = 0; i < subroutines.Length; i++)
            {
                if (subroutines[i] is Scp079RewardManager)
                    _cachedRewardSubrtId = i;
                else if (subroutines[i] is Scp079TierManager)
                    _cachedTierSubrtId = i;
            }

            _cacheSet = true;
        }

        public void MarkRoom(RoomIdentifier room)
        {
            _markedRooms[room] = CurTime;
        }

        public void MarkRooms(RoomIdentifier[] rooms)
        {
            foreach (RoomIdentifier room in rooms)
            {
                MarkRoom(room);
            }
        }

        public void ResetObject()
        {
            _markedRooms.Clear();
        }

        public static bool GrantExpForRoom(RoomIdentifier room, int reward, Scp079HudTranslation gainReason)
        {
            RefreshCache();

            bool granted = false;
            foreach (Scp079Role instance in Scp079Role.ActiveInstances)
            {
                Scp079RewardManager rewardManager = instance.SubroutineModule.AllSubroutines[_cachedRewardSubrtId] as Scp079RewardManager;
                
                if (rewardManager._markedRooms.TryGetValue(room, out double markTime) && !(CurTime - markTime > MarkDuration))
                {
                    granted = true;
                    GrantExp(instance, reward, gainReason);
                }
            }

            return granted;
        }

        public static void GrantExp(Scp079Role instance, int reward, Scp079HudTranslation gainReason)
        {
            RefreshCache();

            if (instance.SubroutineModule.AllSubroutines[_cachedTierSubrtId] is Scp079TierManager tierManager)
            {
                tierManager.ServerGrantExperience(reward, gainReason);
            }
        }

        public Scp079RewardManager()
        {
            _markedRooms = new Dictionary<RoomIdentifier, double>();
        }
    }
}
