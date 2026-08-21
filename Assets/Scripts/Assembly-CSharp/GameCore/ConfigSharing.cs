namespace GameCore
{
    public static class ConfigSharing
    {
        public enum ConfigShare
        {
            Bans = 0,
            Mutes = 1,
            Whitelist = 2,
            ReservedSlots = 3,
            Groups = 4,
            GroupsMembers = 5,
            GameplayDatabase = 6
        }

        public static readonly string[] Shares;

        public static readonly string[] Paths;

        static ConfigSharing()
        {
            Shares = new string[7];
            Paths = new string[7];
            Reload();
        }

        internal static void Reload()
        {
            Shares[0] = global::GameCore.ConfigFile.SharingConfig.GetString("bans");
            Shares[1] = global::GameCore.ConfigFile.SharingConfig.GetString("mutes");
            Shares[2] = global::GameCore.ConfigFile.SharingConfig.GetString("whitelist");
            Shares[3] = global::GameCore.ConfigFile.SharingConfig.GetString("reserved_slots");
            Shares[4] = global::GameCore.ConfigFile.SharingConfig.GetString("groups");
            Shares[5] = global::GameCore.ConfigFile.SharingConfig.GetString("groups_members");
            Shares[6] = global::GameCore.ConfigFile.SharingConfig.GetString("gameplay_database");
            for (ushort num = 0; num < Shares.Length; num++)
            {
                if (Shares[num] == "disable")
                {
                    Paths[num] = ((num == 4 || num == 5) ? null : FileManager.GetAppFolder(addSeparator: true, serverConfig: true));
                }
                else
                {
                    Paths[num] = FileManager.GetAppFolder(addSeparator: true, serverConfig: true, Shares[num]);
                    string appFolder = FileManager.GetAppFolder(addSeparator: true, serverConfig: true, Shares[num]);
                    if (!global::System.IO.Directory.Exists(appFolder))
                    {
                        global::System.IO.Directory.CreateDirectory(appFolder);
                    }
                    switch (num)
                    {
                        case 5:
                            if (num == 5 && !global::System.IO.File.Exists(Paths[num] + "shared_groups_members.txt"))
                            {
                                global::System.IO.File.Copy("ConfigTemplates/shared_groups_members.template.txt", Paths[num] + "shared_groups_members.txt");
                            }
                            break;
                        case 4:
                            if (!global::System.IO.File.Exists(Paths[num] + "shared_groups.txt"))
                            {
                                global::System.IO.File.Copy("ConfigTemplates/shared_groups.template.txt", Paths[num] + "shared_groups.txt");
                            }
                            break;
                    }
                }
            }
            ServerConsole.AddLog("Config sharing loaded.");
        }
    }
}
