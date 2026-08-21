[global::System.Serializable]
public class RoleAccentColor
{
    [global::UnityEngine.SerializeField]
    private global::UnityEngine.Color _savedColor = global::UnityEngine.Color.white;

    [global::UnityEngine.Range(0f, 2f)]
    public float RoleColorInfluence = 1f;

    private global::UnityEngine.Color _prevMixed = global::UnityEngine.Color.white;

    private global::PlayerRoles.RoleTypeId _prevType = global::PlayerRoles.RoleTypeId.None;

    public global::UnityEngine.Color Color
    {
        get
        {
            if (RoleColorInfluence == 0f || !ReferenceHub.TryGetLocalHub(out var hub))
            {
                return _savedColor;
            }
            global::PlayerRoles.PlayerRoleBase currentRole = hub.roleManager.CurrentRole;
            if (RoleColorInfluence == 2f)
            {
                return currentRole.RoleColor;
            }
            if (_prevType != currentRole.RoleTypeId)
            {
                global::UnityEngine.Color roleColor = currentRole.RoleColor;
                global::UnityEngine.Color color = new global::UnityEngine.Color(_savedColor.r * roleColor.r, _savedColor.g * roleColor.g, _savedColor.b * roleColor.b, _savedColor.a * roleColor.a);
                if (RoleColorInfluence == 1f)
                {
                    _prevMixed = color;
                }
                else if (RoleColorInfluence < 1f)
                {
                    _prevMixed = global::UnityEngine.Color.Lerp(_savedColor, color, RoleColorInfluence);
                }
                else
                {
                    _prevMixed = global::UnityEngine.Color.Lerp(color, roleColor, RoleColorInfluence - 1f);
                }
                _prevType = currentRole.RoleTypeId;
            }
            return _prevMixed;
        }
        set
        {
            _savedColor = value;
        }
    }
}
