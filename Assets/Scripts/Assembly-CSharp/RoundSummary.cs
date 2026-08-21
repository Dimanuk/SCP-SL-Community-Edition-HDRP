using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundSummary : NetworkBehaviour
{
    public enum LeadingTeam : byte
    {
        FacilityForces = 0,
        ChaosInsurgency = 1,
        Anomalies = 2,
        Draw = 3
    }

    [Serializable]
    public struct SumInfo_ClassList : IEquatable<SumInfo_ClassList>
    {
        public int class_ds;
        public int scientists;
        public int chaos_insurgents;
        public int mtf_and_guards;
        public int scps_except_zombies;
        public int zombies;
        public int warhead_kills;

        public bool Equals(SumInfo_ClassList other)
        {
            if (class_ds == other.class_ds && scientists == other.scientists && chaos_insurgents == other.chaos_insurgents
                && mtf_and_guards == other.mtf_and_guards && scps_except_zombies == other.scps_except_zombies && zombies == other.zombies)
            {
                return warhead_kills == other.warhead_kills;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is SumInfo_ClassList other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (((((((((((class_ds * 397) ^ scientists) * 397) ^ chaos_insurgents) * 397) ^ mtf_and_guards) * 397) ^ scps_except_zombies) * 397) ^ zombies) * 397) ^ warhead_kills;
        }

        public static bool operator ==(SumInfo_ClassList left, SumInfo_ClassList right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SumInfo_ClassList left, SumInfo_ClassList right)
        {
            return !left.Equals(right);
        }
    }

    private bool _roundEnded;
    private bool _summaryActive;

    public bool KeepRoundOnOne;

    public SumInfo_ClassList classlistStart;

    public GameObject ui_root;

    public static bool RoundLock;

    public Image fadeOutImage;

    public TextMeshProUGUI ui_text_header;

    public TextMeshProUGUI ui_text_who_won;

    public TextMeshProUGUI ui_text_info;

    public static RoundSummary singleton;

    private static bool _singletonSet;

    public static int roundTime;

    public static bool SummaryActive
    {
        get
        {
            if (_singletonSet)
            {
                return singleton._summaryActive;
            }
            return false;
        }
    }

    public static int Kills { get; private set; }

    public static int EscapedClassD { get; private set; }

    public static int EscapedScientists { get; private set; }

    public static int SurvivingSCPs { get; private set; }

    public static int KilledBySCPs { get; private set; }

    public static int ChangedIntoZombies { get; private set; }

    private void Start()
    {
        singleton = this;
        _singletonSet = true;
        if (NetworkServer.active)
        {
            roundTime = 0;
            KeepRoundOnOne = !GameCore.ConfigFile.ServerConfig.GetBool("end_round_on_one_player");
            MEC.Timing.RunCoroutine(_ProcessServerSideCode(), MEC.Segment.FixedUpdate);
            KilledBySCPs = 0;
            EscapedClassD = 0;
            EscapedScientists = 0;
            ChangedIntoZombies = 0;
            Kills = 0;
            PlayerRoleManager.OnServerRoleSet += OnRoleChanged;
            PlayerStats.OnAnyPlayerDied += OnAnyPlayerDied;
        }
    }

    private void OnDestroy()
    {
        _singletonSet = false;
        PlayerRoleManager.OnServerRoleSet -= OnRoleChanged;
        PlayerStats.OnAnyPlayerDied -= OnAnyPlayerDied;
    }

    private void OnAnyPlayerDied(ReferenceHub ply, DamageHandlerBase handler)
    {
        Kills++;
        if (handler is UniversalDamageHandler universalDamageHandler)
        {
            if (universalDamageHandler.TranslationId != DeathTranslations.PocketDecay.Id)
            {
                return;
            }
        }
        else if (!(handler is AttackerDamageHandler attackerDamageHandler) || !PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(attackerDamageHandler.Attacker.Role, out PlayerRoleBase result) || result.Team != Team.SCPs)
        {
            return;
        }
        KilledBySCPs++;
    }

    private void OnRoleChanged(ReferenceHub userHub, RoleTypeId newRole, RoleChangeReason reason)
    {
        switch (reason)
        {
            case RoleChangeReason.RoundStart:
            case RoleChangeReason.LateJoin:
                AddSpawnedTeam(PlayerRolesUtils.GetTeam(newRole));
                break;
            case RoleChangeReason.Escaped:
                if (!InventorySystem.Disarming.DisarmedPlayers.IsDisarmed(userHub.inventory))
                {
                    switch (PlayerRolesUtils.GetTeam(newRole))
                    {
                        case Team.FoundationForces:
                            EscapedScientists++;
                            break;
                        case Team.ChaosInsurgency:
                            EscapedClassD++;
                            break;
                    }
                }
                break;
            case RoleChangeReason.Revived:
                ChangedIntoZombies++;
                classlistStart.zombies++;
                break;
            case RoleChangeReason.Respawn:
            case RoleChangeReason.Died:
                break;
        }
    }

    private void AddSpawnedTeam(Team t)
    {
        switch (t)
        {
            case Team.ChaosInsurgency:
                classlistStart.chaos_insurgents++;
                break;
            case Team.ClassD:
                classlistStart.class_ds++;
                break;
            case Team.FoundationForces:
                classlistStart.mtf_and_guards++;
                break;
            case Team.Scientists:
                classlistStart.scientists++;
                break;
            case Team.SCPs:
                classlistStart.scps_except_zombies++;
                break;
        }
    }

    public void ForceEnd()
    {
        _roundEnded = true;
    }

    public int CountRole(RoleTypeId role)
    {
        return ReferenceHub.AllHubs.Count((ReferenceHub x) => PlayerRolesUtils.GetRoleId(x) == role);
    }

    public int CountTeam(Team team)
    {
        return ReferenceHub.AllHubs.Count((ReferenceHub x) => PlayerRolesUtils.GetTeam(x) == team);
    }

    private IEnumerator<float> _ProcessServerSideCode()
    {
        float time = Time.unscaledTime;
        while (this != null)
        {
            yield return MEC.Timing.WaitForSeconds(2.5f);
            if (RoundLock || (KeepRoundOnOne && ReferenceHub.AllHubs.Count((ReferenceHub x) => x.characterClassManager.InstanceMode != ClientInstanceMode.DedicatedServer) < 2) || !RoundInProgress() || Time.unscaledTime - time < 15f)
            {
                continue;
            }

            SumInfo_ClassList newList = default;
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                switch (PlayerRolesUtils.GetTeam(allHub))
                {
                    case Team.ClassD:
                        newList.class_ds++;
                        break;
                    case Team.ChaosInsurgency:
                        newList.chaos_insurgents++;
                        break;
                    case Team.FoundationForces:
                        newList.mtf_and_guards++;
                        break;
                    case Team.Scientists:
                        newList.scientists++;
                        break;
                    case Team.SCPs:
                        if (PlayerRolesUtils.GetRoleId(allHub) == RoleTypeId.Scp0492)
                        {
                            newList.zombies++;
                        }
                        else
                        {
                            newList.scps_except_zombies++;
                        }
                        break;
                }
            }

            yield return float.NegativeInfinity;
            newList.warhead_kills = (AlphaWarheadController.Detonated ? AlphaWarheadController.Singleton.WarheadKills : (-1));
            yield return float.NegativeInfinity;

            int facilityForces = newList.mtf_and_guards + newList.scientists;
            int chaosInsurgency = newList.chaos_insurgents + newList.class_ds;
            int anomalies = newList.scps_except_zombies + newList.zombies;
            int num = newList.class_ds + EscapedClassD;
            int num2 = newList.scientists + EscapedScientists;
            SurvivingSCPs = newList.scps_except_zombies;

            float dEscapePercentage = ((classlistStart.class_ds != 0) ? ((float)num / classlistStart.class_ds) : 0f);
            float sEscapePercentage = ((classlistStart.scientists == 0) ? 1f : ((float)num2 / classlistStart.scientists));

            bool flag;
            if (newList.class_ds <= 0 && facilityForces <= 0)
            {
                flag = true;
            }
            else
            {
                int num3 = 0;
                if (facilityForces > 0)
                {
                    num3++;
                }
                if (chaosInsurgency > 0)
                {
                    num3++;
                }
                if (anomalies > 0)
                {
                    num3++;
                }
                flag = num3 <= 1;
            }

            if (!_roundEnded)
            {
                if (flag)
                {
                    _roundEnded = true;
                }
            }

            if (!_roundEnded)
            {
                continue;
            }

            bool num5 = facilityForces > 0;
            bool flag2 = chaosInsurgency > 0;
            bool flag3 = anomalies > 0;
            LeadingTeam leadingTeam = LeadingTeam.Draw;

            if (num5)
            {
                leadingTeam = ((EscapedScientists < EscapedClassD) ? LeadingTeam.Draw : LeadingTeam.FacilityForces);
            }
            else if (flag3 || (flag3 && flag2))
            {
                leadingTeam = ((EscapedClassD > SurvivingSCPs) ? LeadingTeam.ChaosInsurgency : ((SurvivingSCPs > EscapedScientists) ? LeadingTeam.Anomalies : LeadingTeam.Draw));
            }
            else if (flag2)
            {
                leadingTeam = ((EscapedClassD >= EscapedScientists) ? LeadingTeam.ChaosInsurgency : LeadingTeam.Draw);
            }

            FriendlyFireConfig.PauseDetector = true;
            string text = "Round finished! Anomalies: " + anomalies + " | Chaos: " + chaosInsurgency + " | Facility Forces: " + facilityForces + " | D escaped percentage: " + dEscapePercentage + " | S escaped percentage: : " + sEscapePercentage;
            GameCore.Console.AddLog(text, Color.gray);
            ServerLogs.AddLog(ServerLogs.Modules.Logger, text, ServerLogs.ServerLogType.GameEvent);

            yield return MEC.Timing.WaitForSeconds(1.5f);
            int num6 = Mathf.Clamp(GameCore.ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);

            if (this != null)
            {
                RpcShowRoundSummary(classlistStart, newList, leadingTeam, EscapedClassD, EscapedScientists, KilledBySCPs, num6, (int)GameCore.RoundStart.RoundLength.TotalSeconds);
            }

            yield return MEC.Timing.WaitForSeconds(num6 - 1);
            if (this != null)
            {
                RpcDimScreen();
            }
            yield return MEC.Timing.WaitForSeconds(1f);
            RoundRestarting.RoundRestart.InitiateRoundRestart();
        }
    }

    [ClientRpc]
    private void RpcShowRoundSummary(SumInfo_ClassList listStart, SumInfo_ClassList listFinish, LeadingTeam leadingTeam, int eDS, int eSc, int scpKills, int roundCd, int seconds)
    {
        _summaryActive = true;
        MEC.Timing.RunCoroutine(_ShowRoundSummary(listStart, listFinish, leadingTeam, eDS, eSc, scpKills, roundCd, seconds), MEC.Segment.Update);
    }

    private IEnumerator<float> _ShowRoundSummary(SumInfo_ClassList list_start, SumInfo_ClassList list_finish, LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd, int seconds)
    {
        if (this == null)
            yield break;

        _roundEnded = true;
        string text = string.Empty;

        switch (leadingTeam)
        {
            case LeadingTeam.Draw:
                text = "<color=#FEFEFE> " + TranslationReader.Get("Summary", 4, "NO_TRANSLATION");
                break;
            case LeadingTeam.FacilityForces:
                text = "<color=#0096FF> " + TranslationReader.Get("Summary", 2, "NO_TRANSLATION");
                break;
            case LeadingTeam.ChaosInsurgency:
                text = "<color=#008F1E> " + TranslationReader.Get("Summary", 3, "NO_TRANSLATION");
                break;
            case LeadingTeam.Anomalies:
                text = "<color=red> " + TranslationReader.Get("Summary", 1, "NO_TRANSLATION");
                break;
        }

        text += "</color>";
        ui_text_who_won.text = text;
        ui_text_who_won.RecalculateMasking();

        int minutes = seconds / 60;
        seconds = seconds % 60;

        int totalZombies = list_finish.zombies + list_start.zombies;
        int totalSCPs = list_finish.scps_except_zombies + list_finish.zombies;

        string warheadText = (list_finish.warhead_kills == -1)
            ? TranslationReader.Get("Summary", 12, "NO_TRANSLATION")
            : list_finish.warhead_kills.ToString();

        string dEscaped = string.Format("<color=red>{0}</color>/<color=red>{1}</color>",
            list_finish.class_ds + e_ds, list_start.class_ds);
        string dEscapedFormatted = TranslationReader.GetFormatted("Summary", 9, "", dEscaped);

        string sEscaped = string.Format("<color=red>{0}</color>/<color=red>{1}</color>",
            list_finish.scientists + e_sc, list_start.scientists);
        string sEscapedFormatted = TranslationReader.GetFormatted("Summary", 10, "", sEscaped);

        string scpKilled = string.Format("<color=red>{0}</color>/<color=red>{1}</color>",
            totalSCPs - totalZombies, totalZombies);
        string scpKilledFormatted = TranslationReader.GetFormatted("Summary", 14, "", scpKilled);

        string warheadFormatted = string.Format("<color=red>{0}</color>", warheadText);
        string warheadResult = TranslationReader.GetFormatted("Summary", 11, "", warheadFormatted);

        string timeFormatted = string.Format("{0}:{1:D2}", minutes, seconds);

        string result = dEscapedFormatted + "\n" + sEscapedFormatted + "\n" + scpKilledFormatted;
        result = TranslationReader.GetFormatted("Summary", 13, "", result, warheadResult);

        string finalText = result + "\n\n" + TranslationReader.GetFormatted("Summary", 15, "",
            string.Format("<color=red>{0}</color>", timeFormatted));
        finalText += "\n" + TranslationReader.GetFormatted("Summary", 16, "",
            string.Format("<color=red>{0}</color>", scp_kills));

        ui_text_info.text = finalText;

        if (ui_root != null)
            ui_root.SetActive(true);

        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
            rect.localPosition = Vector3.zero;

        if (ui_root != null)
        {
            RectTransform rootRect = ui_root.GetComponent<RectTransform>();
            if (rootRect != null)
                rootRect.localPosition = Vector3.zero;
        }

        float deltaTime = 0f;
        while (deltaTime < 1f)
        {
            deltaTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(deltaTime);

            if (ui_text_header != null)
                ui_text_header.alpha = alpha;
            if (ui_text_info != null)
                ui_text_info.alpha = alpha;
            if (ui_text_who_won != null)
                ui_text_who_won.alpha = alpha;

            yield return float.NegativeInfinity;
        }
    }

    [ClientRpc]
    private void RpcDimScreen()
    {
        MEC.Timing.RunCoroutine(_FadeScreenOut(), MEC.Segment.Update);
    }

    private IEnumerator<float> _FadeScreenOut()
    {
        if (fadeOutImage == null)
            yield break;

        float elapsed = 0f;
        Color startColor = Color.black;
        startColor.a = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed);
            fadeOutImage.color = Color.Lerp(startColor, Color.black, t);
            yield return float.NegativeInfinity;
        }

        fadeOutImage.color = Color.black;
    }

    public static bool RoundInProgress()
    {
        ReferenceHub localHub = ReferenceHub.LocalHub;
        if (localHub == null || localHub.characterClassManager == null)
            return false;

        return !singleton._roundEnded && GameCore.RoundStart.RoundStarted;
    }
}