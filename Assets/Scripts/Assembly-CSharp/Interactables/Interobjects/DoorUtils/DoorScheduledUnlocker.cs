namespace Interactables.Interobjects.DoorUtils
{
    public static class DoorScheduledUnlocker
    {
        private class ScheduledUnlock
        {
            public readonly global::Interactables.Interobjects.DoorUtils.DoorVariant Door;

            private float _targetTime;

            private readonly global::Interactables.Interobjects.DoorUtils.DoorLockReason _flagsToUnset;

            public ScheduledUnlock(global::Interactables.Interobjects.DoorUtils.DoorVariant targetDoor, float time, global::Interactables.Interobjects.DoorUtils.DoorLockReason flags)
            {
                Door = targetDoor;
                _targetTime = global::UnityEngine.Time.timeSinceLevelLoad + time;
                _flagsToUnset = flags;
            }

            public bool Refresh()
            {
                if (global::UnityEngine.Time.timeSinceLevelLoad < _targetTime)
                {
                    return false;
                }
                if (Door != null)
                {
                    Door.ServerChangeLock(_flagsToUnset, newState: false);
                }
                return true;
            }
        }

        private static readonly global::System.Collections.Generic.List<global::Interactables.Interobjects.DoorUtils.DoorScheduledUnlocker.ScheduledUnlock> Entries = new global::System.Collections.Generic.List<global::Interactables.Interobjects.DoorUtils.DoorScheduledUnlocker.ScheduledUnlock>();

        private static readonly global::System.Collections.Generic.List<int> EntriesToRemove = new global::System.Collections.Generic.List<int>();

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            StaticUnityMethods.OnUpdate += UnityStaticUpdate;
            global::InventorySystem.Inventory.OnServerStarted += Entries.Clear;
            global::InventorySystem.Inventory.OnServerStarted += EntriesToRemove.Clear;
        }

        private static void UnityStaticUpdate()
        {
            if (!StaticUnityMethods.IsPlaying)
            {
                return;
            }
            bool flag = false;
            for (int i = 0; i < Entries.Count; i++)
            {
                if (Entries[i].Refresh())
                {
                    flag = true;
                    EntriesToRemove.Add(i);
                }
            }
            if (flag)
            {
                for (int j = 0; j < EntriesToRemove.Count; j++)
                {
                    Entries.RemoveAt(EntriesToRemove[j] - j);
                }
                EntriesToRemove.Clear();
            }
        }

        public static void UnlockLater(this global::Interactables.Interobjects.DoorUtils.DoorVariant door, float time, global::Interactables.Interobjects.DoorUtils.DoorLockReason flagsToUnlock)
        {
            global::Interactables.Interobjects.DoorUtils.DoorScheduledUnlocker.ScheduledUnlock scheduledUnlock = new global::Interactables.Interobjects.DoorUtils.DoorScheduledUnlocker.ScheduledUnlock(door, time, flagsToUnlock);
            for (int i = 0; i < Entries.Count; i++)
            {
                if (!(Entries[i].Door != door))
                {
                    Entries[i] = scheduledUnlock;
                }
            }
            Entries.Add(scheduledUnlock);
        }
    }
}
