public static class WhiteList
{
    private static readonly global::System.Collections.Generic.HashSet<string> Users;

    static WhiteList()
    {
        Users = new global::System.Collections.Generic.HashSet<string>();
        Reload();
    }

    public static void Reload()
    {
        string path = global::GameCore.ConfigSharing.Paths[2] + "UserIDWhitelist.txt";
        Users.Clear();
        if (!global::System.IO.File.Exists(path))
        {
            FileManager.WriteStringToFile("#Put one UserID (eg. 76561198071934271@steam or 274613382353518592@discord) per line. Lines prefixed with \"#\" are ignored.", path);
            return;
        }
        using (global::System.IO.StreamReader streamReader = new global::System.IO.StreamReader(path))
        {
            string text;
            while ((text = streamReader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(text) && !text.TrimStart().StartsWith("#") && text.Contains("@"))
                {
                    Users.Add(text.Trim());
                }
            }
        }
        ServerConsole.AddLog("Whitelist has been loaded.");
    }

    public static bool IsOnWhitelist(string userId)
    {
        return Users.Contains(userId.Trim());
    }

    public static bool IsWhitelisted(string userId)
    {
        if (!Users.Contains(userId.Trim()) && global::GameCore.ConfigFile.ServerConfig.GetBool("enable_whitelist"))
        {
            return !CharacterClassManager.OnlineMode;
        }
        return true;
    }
}
