namespace Interactables.Interobjects.DoorUtils
{
    public static class DoorLockUtils
    {
        public static global::Interactables.Interobjects.DoorUtils.DoorLockMode GetMode(global::Interactables.Interobjects.DoorUtils.DoorLockReason reason)
        {
            if (reason == global::Interactables.Interobjects.DoorUtils.DoorLockReason.None)
            {
                return global::Interactables.Interobjects.DoorUtils.DoorLockMode.CanOpen | global::Interactables.Interobjects.DoorUtils.DoorLockMode.CanClose;
            }
            if ((int)(reason & (global::Interactables.Interobjects.DoorUtils.DoorLockReason.AdminCommand | global::Interactables.Interobjects.DoorUtils.DoorLockReason.DecontLockdown | global::Interactables.Interobjects.DoorUtils.DoorLockReason.SpecialDoorFeature | global::Interactables.Interobjects.DoorUtils.DoorLockReason.NoPower | global::Interactables.Interobjects.DoorUtils.DoorLockReason.Lockdown2176)) > 0)
            {
                return global::Interactables.Interobjects.DoorUtils.DoorLockMode.FullLock;
            }
            if ((int)(reason & (global::Interactables.Interobjects.DoorUtils.DoorLockReason.Regular079 | global::Interactables.Interobjects.DoorUtils.DoorLockReason.Lockdown079)) > 0)
            {
                return global::Interactables.Interobjects.DoorUtils.DoorLockMode.ScpOverride;
            }
            if ((int)(reason & (global::Interactables.Interobjects.DoorUtils.DoorLockReason.Warhead | global::Interactables.Interobjects.DoorUtils.DoorLockReason.DecontEvacuate)) > 0)
            {
                return global::Interactables.Interobjects.DoorUtils.DoorLockMode.CanOpen;
            }
            if ((int)(reason & global::Interactables.Interobjects.DoorUtils.DoorLockReason.Isolation) > 0)
            {
                return global::Interactables.Interobjects.DoorUtils.DoorLockMode.CanClose;
            }
            return global::Interactables.Interobjects.DoorUtils.DoorLockMode.CanOpen | global::Interactables.Interobjects.DoorUtils.DoorLockMode.CanClose;
        }

        public static bool HasFlagFast(this global::Interactables.Interobjects.DoorUtils.DoorLockMode mode, global::Interactables.Interobjects.DoorUtils.DoorLockMode flag)
        {
            return (mode & flag) == flag;
        }

        public static bool HasFlagFast(this global::Interactables.Interobjects.DoorUtils.DoorLockReason res, global::Interactables.Interobjects.DoorUtils.DoorLockReason flag)
        {
            return (res & flag) == flag;
        }
    }
}
