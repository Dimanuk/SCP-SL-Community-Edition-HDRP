namespace Interactables.Interobjects.DoorUtils
{
    public static class DoorPermissionUtils
    {
        private static readonly global::System.Collections.Generic.Dictionary<string, global::Interactables.Interobjects.DoorUtils.KeycardPermissions> BackwardsCompatibilityPermissions = new global::System.Collections.Generic.Dictionary<string, global::Interactables.Interobjects.DoorUtils.KeycardPermissions>
        {
            {
                "CONT_LVL_1",
                global::Interactables.Interobjects.DoorUtils.KeycardPermissions.ContainmentLevelOne
            },
            {
                "CONT_LVL_2",
                global::Interactables.Interobjects.DoorUtils.KeycardPermissions.ContainmentLevelTwo
            },
            {
                "CONT_LVL_3",
                global::Interactables.Interobjects.DoorUtils.KeycardPermissions.ContainmentLevelThree
            },
            {
                "ARMORY_LVL_1",
                global::Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelOne
            },
            {
                "ARMORY_LVL_2",
                global::Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelTwo
            },
            {
                "ARMORY_LVL_3",
                global::Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelThree
            },
            {
                "INCOM_ACC",
                global::Interactables.Interobjects.DoorUtils.KeycardPermissions.Intercom
            },
            {
                "CHCKPOINT_ACC",
                global::Interactables.Interobjects.DoorUtils.KeycardPermissions.Checkpoints
            },
            {
                "EXIT_ACC",
                global::Interactables.Interobjects.DoorUtils.KeycardPermissions.ExitGates
            }
        };

        public static bool HasFlagFast(this global::Interactables.Interobjects.DoorUtils.KeycardPermissions perm, global::Interactables.Interobjects.DoorUtils.KeycardPermissions flag)
        {
            return (perm & flag) == flag;
        }

        public static global::Interactables.Interobjects.DoorUtils.KeycardPermissions TranslateObsoletePermissions(string[] obsoletePerms)
        {
            int num = 0;
            foreach (string key in obsoletePerms)
            {
                if (BackwardsCompatibilityPermissions.TryGetValue(key, out var value))
                {
                    num += (int)value;
                }
            }
            return (global::Interactables.Interobjects.DoorUtils.KeycardPermissions)num;
        }
    }
}
