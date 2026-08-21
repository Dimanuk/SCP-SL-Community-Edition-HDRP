using InventorySystem.Disarming;
using Mirror;
using PlayerRoles;
using Respawning;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Escape
{
    private enum EscapeScenarioType
    {
        None = 0,
        ClassD = 1,
        CuffedClassD = 2,
        Scientist = 3,
        CuffedScientist = 4
    }

    private readonly struct EscapeScenarioText
    {
        private readonly int _id;
        private readonly string _def;

        public string Text => TranslationReader.Get(TranslationKey, _id, _def);

        public EscapeScenarioText(int translationKey, string defaultText)
        {
            _id = translationKey;
            _def = defaultText;
        }
    }

    public struct EscapeMessage : NetworkMessage
    {
        public byte ScenarioId;
        public ushort EscapeTime;
    }

    private static readonly Dictionary<EscapeScenarioType, EscapeScenarioText> Scenarios = new Dictionary<EscapeScenarioType, EscapeScenarioText>
    {
        [EscapeScenarioType.ClassD] = new EscapeScenarioText(30, "You escaped as a Class D and joined the Chaos Insurgency."),
        [EscapeScenarioType.CuffedClassD] = new EscapeScenarioText(36, "You were recaptured by the Nine-Tailed Fox.\nWith one less threat in the facility, they were able to reinforce."),
        [EscapeScenarioType.Scientist] = new EscapeScenarioText(29, "You escaped as a Scientist and joined the MTF units."),
        [EscapeScenarioType.CuffedScientist] = new EscapeScenarioText(37, "You were taken prisoner as a scientist by the Chaos Insurgency.\nThey were able to gain an advantage from the information you gave them.")
    };

    private static readonly Vector3 WorldPos = new Vector3(124f, 989f, 31f);
    private const float RadiusSqr = 156.5f;
    private const float MinAliveTime = 10f;
    private const string TranslationKey = "Facility";
    private const float InsurgencyEscapeReward = 4f;
    private const float FoundationEscapeReward = 3f;

    public static event Action<ReferenceHub> OnServerPlayerEscape;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        NetworkServer.RegisterHandler<EscapeMessage>((conn, msg) => {
        });

        CustomNetworkManager.OnClientReady += delegate
        {
            NetworkClient.ReplaceHandler<EscapeMessage>(ClientReceiveMessage, true);
        };

        StaticUnityMethods.OnUpdate += delegate
        {
            if (NetworkServer.active)
            {
                Utils.NonAllocLINQ.HashsetExtensions.ForEach(ReferenceHub.AllHubs, ServerHandlePlayer);
            }
        };
    }

    private static void ServerHandlePlayer(ReferenceHub hub)
    {
        EscapeScenarioType escapeScenarioType = ServerGetScenario(hub);
        RoleTypeId roleTypeId = RoleTypeId.None;

        switch (escapeScenarioType)
        {
            case EscapeScenarioType.None:
                return;
            case EscapeScenarioType.ClassD:
            case EscapeScenarioType.CuffedScientist:
                roleTypeId = RoleTypeId.ChaosConscript;
                RespawnTokensManager.GrantTokens(SpawnableTeamType.ChaosInsurgency, InsurgencyEscapeReward);
                break;
            case EscapeScenarioType.CuffedClassD:
                roleTypeId = RoleTypeId.NtfPrivate;
                RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, FoundationEscapeReward);
                break;
            case EscapeScenarioType.Scientist:
                roleTypeId = RoleTypeId.NtfSpecialist;
                RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, FoundationEscapeReward);
                break;
        }

        hub.connectionToClient.Send(new EscapeMessage
        {
            ScenarioId = (byte)escapeScenarioType,
            EscapeTime = (ushort)Mathf.CeilToInt(hub.roleManager.CurrentRole.ActiveTime)
        });

        OnServerPlayerEscape?.Invoke(hub);
        hub.roleManager.ServerSetRole(roleTypeId, RoleChangeReason.Escaped);
    }

    private static EscapeScenarioType ServerGetScenario(ReferenceHub hub)
    {
        if (hub.roleManager.CurrentRole is not HumanRole humanRole)
            return EscapeScenarioType.None;

        if ((humanRole.FpcModule.Position - WorldPos).sqrMagnitude > RadiusSqr)
            return EscapeScenarioType.None;

        if (humanRole.ActiveTime < MinAliveTime)
            return EscapeScenarioType.None;

        bool isDisarmed = DisarmedPlayers.IsDisarmed(hub.inventory);
        if (isDisarmed && !CharacterClassManager.CuffedChangeTeam)
            return EscapeScenarioType.None;

        switch (humanRole.RoleTypeId)
        {
            case RoleTypeId.Scientist:
                return isDisarmed ? EscapeScenarioType.CuffedScientist : EscapeScenarioType.Scientist;
            case RoleTypeId.ClassD:
                return isDisarmed ? EscapeScenarioType.CuffedClassD : EscapeScenarioType.ClassD;
            default:
                return EscapeScenarioType.None;
        }
    }

    private static void ClientReceiveMessage(EscapeMessage msg)
    {
        if (Scenarios.TryGetValue((EscapeScenarioType)msg.ScenarioId, out var scenario))
        {
            MEC.Timing.RunCoroutine(PlayEscapeAnim(scenario.Text, msg.EscapeTime));
        }
    }

    private static IEnumerator<float> PlayEscapeAnim(string txt, int seconds)
    {
        int minutes = 0;
        while (seconds >= 60)
        {
            seconds -= 60;
            minutes++;
        }

        string fullMessage = txt + "\n" + TranslationReader.Get(TranslationKey, 32, "Escape time: {0} minutes and {1} seconds");
        GameObject respawnText = GameObject.Find("Respawn Text");

        if (respawnText == null) yield break;

        UnityEngine.UI.Text textComp = respawnText.GetComponent<UnityEngine.UI.Text>();
        CanvasRenderer canvasRenderer = textComp.GetComponent<CanvasRenderer>();

        canvasRenderer.SetAlpha(0f);
        textComp.text = string.Format(fullMessage, minutes, seconds);

        for (int i = 0; i < 50; i++)
        {
            canvasRenderer.SetAlpha(canvasRenderer.GetAlpha() + 0.02f);
            yield return 0f;
        }

        for (int i = 0; i < 100; i++)
        {
            yield return 0f;
        }

        for (int i = 0; i < 50; i++)
        {
            canvasRenderer.SetAlpha(canvasRenderer.GetAlpha() - 0.02f);
            yield return 0f;
        }

        canvasRenderer.SetAlpha(0f);
    }
}