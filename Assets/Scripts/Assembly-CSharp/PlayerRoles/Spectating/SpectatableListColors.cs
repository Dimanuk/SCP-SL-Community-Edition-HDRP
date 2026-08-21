namespace PlayerRoles.Spectating
{
    public static class SpectatableListColors
    {
        public static readonly global::UnityEngine.Color BgRegular = new global::UnityEngine.Color(0.161f, 0.161f, 0.161f, 0.761f);

        public static readonly global::UnityEngine.Color BgSelected = new global::UnityEngine.Color(0.161f, 0.161f, 0.161f, 1f);

        public static readonly global::UnityEngine.Color Nickname = new global::UnityEngine.Color(0.518f, 0.518f, 0.518f, 1f);

        public static readonly global::UnityEngine.Color Shield = new global::UnityEngine.Color(0.682f, 0.682f, 0.682f, 1f);

        public static global::UnityEngine.Color MixAvatarColor(global::UnityEngine.Color roleColor)
        {
            return global::UnityEngine.Color.Lerp(b: new global::UnityEngine.Color(0.443f, 0.443f, 0.443f, 1f), a: roleColor.linear, t: 0.63f);
        }
    }
}
