using System.Collections.Generic;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class YouWereKilled : MonoBehaviour
{
    public static YouWereKilled Singleton;
    [Space]
    public GameObject _root;
    public Text _info;

    private void Awake()
    {
        Singleton = this;
    }

    public void PlayRegular(DamageHandlerBase hitInfo)
    {
        Timing.RunCoroutine(_Play(hitInfo.RagdollInspectText, null, null), Segment.FixedUpdate);
    }

    public void PlayAttacker(string nickname, RoleTypeId role)
    {
        string attackerRole = null;
        if (PlayerRoleLoader.TryGetRoleTemplate(role, out PlayerRoleBase roleBase))
        {
            attackerRole = roleBase.RoleName;
        }
        Timing.RunCoroutine(_Play(null, nickname, attackerRole), Segment.FixedUpdate);
    }

    private IEnumerator<float> _Play(string reason, string attacker, string attackerRole)
    {
        _root.SetActive(true);
        CanvasRenderer[] renderers = _root.GetComponentsInChildren<CanvasRenderer>();

        if (string.IsNullOrEmpty(reason))
        {
            string killedByText = TranslationReader.Get("Legacy_Interfaces", 15, "NO_TRANSLATION");
            string withText = TranslationReader.Get("Legacy_Interfaces", 16, "NO_TRANSLATION");

            _info.text = $"<size=20>{killedByText}</size>\n{attacker}<size=20>\n{withText}\n</size>{attackerRole}";
        }
        else
        {
            _info.text = reason;
        }

        int tick = 0;

        while (tick < 50)
        {
            float alpha = tick / 50f;
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].SetAlpha(alpha);
                }
            }
            yield return Timing.WaitForOneFrame;
            tick++;
        }

        while (tick < 150)
        {
            yield return Timing.WaitForOneFrame;
            tick++;
        }

        while (tick < 200)
        {
            float alpha = 1f - ((tick - 150) / 50f);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].SetAlpha(alpha);
                }
            }
            yield return Timing.WaitForOneFrame;
            tick++;
        }

        foreach (var cr in renderers)
        {
            if (cr != null) cr.SetAlpha(0f);
        }
    }
}