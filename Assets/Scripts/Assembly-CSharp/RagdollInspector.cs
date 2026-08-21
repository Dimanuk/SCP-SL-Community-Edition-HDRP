using PlayerRoles;
using PlayerRoles.Ragdolls;
using TMPro;
using UnityEngine;

public class RagdollInspector : MonoBehaviour
{
    [SerializeField] private LayerMask _raycastMask;
    [SerializeField] private float _raycastDistance = 100f;

    [SerializeField] private TextMeshProUGUI _ragdollInspectText;

    private void Update()
    {
        if (!TryGetRagdoll(out BasicRagdoll ragdoll, out string coloredRoleName))
        {
            if (_ragdollInspectText != null)
                _ragdollInspectText.text = string.Empty;
            return;
        }

        string message = TranslationReader.Get("DeathReasons", 0,
            "It's <b>{0}</b>'s body - they were {1}!\n\nCause of death: {2}");

        RagdollData info = ragdoll.Info;
        string nickname = info.Nickname ?? "Unknown";
        string deathCause = info.Handler != null ? info.Handler.RagdollInspectText : "Unknown";

        string finalText = string.Format(message, nickname, coloredRoleName, deathCause);

        if (_ragdollInspectText != null)
            _ragdollInspectText.text = finalText;
    }

    private bool TryGetRagdoll(out BasicRagdoll ragdoll, out string coloredRoleName)
    {
        ragdoll = null;
        coloredRoleName = null;

        if (!ReferenceHub.TryGetLocalHub(out ReferenceHub localHub) ||
            !PlayerRolesUtils.IsAlive(localHub))
            return false;

        // Aim along the player's CAMERA, not the body root (which is at the feet with no pitch).
        Transform camera = localHub.PlayerCameraReference;
        if (camera == null)
            return false;

        Vector3 origin = camera.position;
        Vector3 direction = camera.forward;

        if (!Physics.Raycast(origin, direction, out RaycastHit hit, _raycastDistance, _raycastMask))
            return false;

        ragdoll = hit.transform.GetComponentInParent<BasicRagdoll>();

        if (ragdoll == null)
        {
            var part = hit.transform.GetComponent<RagdollPart>();
            if (part != null)
                ragdoll = part.ParentRagdoll;
        }

        if (ragdoll == null)
            return false;

        if (!PlayerRoleLoader.AllRoles.TryGetValue(ragdoll.Info.RoleType, out PlayerRoleBase role))
            return false;

        coloredRoleName = "<color=#" + ColorUtility.ToHtmlStringRGB(role.RoleColor) + ">" + role.RoleName + "</color>";
        return true;
    }
}
