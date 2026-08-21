namespace PlayerRoles
{
    public static class RoleTranslations
    {
        private static readonly global::System.Collections.Generic.Dictionary<global::PlayerRoles.RoleTypeId, string> TranslatedNames = new global::System.Collections.Generic.Dictionary<global::PlayerRoles.RoleTypeId, string>();

        public const string RoleNamesFile = "Class_Names";

        public static string GetRoleName(global::PlayerRoles.RoleTypeId rt)
        {
            if (!TranslatedNames.TryGetValue(rt, out var value))
            {
                return rt.ToString();
            }
            return value;
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            TranslationReader.OnTranslationsRefreshed += ReloadNames;
        }

        private static void ReloadNames()
        {
            foreach (global::System.Collections.Generic.KeyValuePair<global::PlayerRoles.RoleTypeId, global::PlayerRoles.PlayerRoleBase> allRole in global::PlayerRoles.PlayerRoleLoader.AllRoles)
            {
                int key = (int)allRole.Key;
                if (key >= 0 && TranslationReader.TryGet("Class_Names", key, out var val))
                {
                    TranslatedNames[allRole.Key] = val;
                }
            }
        }
    }
}
