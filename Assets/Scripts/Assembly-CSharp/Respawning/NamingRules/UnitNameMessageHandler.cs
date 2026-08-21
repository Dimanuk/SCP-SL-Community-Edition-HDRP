using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Mirror;
using UnityEngine;

namespace Respawning.NamingRules
{
	public static class UnitNameMessageHandler
	{
        public static global::System.Collections.Generic.Dictionary<global::Respawning.SpawnableTeamType, global::System.Collections.Generic.List<string>> ReceivedNames = new global::System.Collections.Generic.Dictionary<global::Respawning.SpawnableTeamType, global::System.Collections.Generic.List<string>>();

        private static readonly global::Mirror.NetworkWriter SendHistory = new global::Mirror.NetworkWriter();

        private static readonly global::Respawning.SpawnableTeamType[] PregeneratedNameTeams = new global::Respawning.SpawnableTeamType[1] { global::Respawning.SpawnableTeamType.NineTailedFox };

        public static event global::System.Action<global::Respawning.SpawnableTeamType, string, int> OnNameAdded;

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                SendHistory.Reset();
                ReceivedNames.Clear();
                global::Mirror.NetworkClient.ReplaceHandler<global::Respawning.NamingRules.UnitNameMessage>(ProcessMessage);
                if (global::Mirror.NetworkServer.active)
                {
                    global::Respawning.SpawnableTeamType[] pregeneratedNameTeams = PregeneratedNameTeams;
                    foreach (global::Respawning.SpawnableTeamType spawnableTeamType in pregeneratedNameTeams)
                    {
                        if (global::Respawning.NamingRules.UnitNamingRule.TryGetNamingRule(spawnableTeamType, out var rule))
                        {
                            SendNew(spawnableTeamType, rule);
                        }
                    }
                }
            };
            ReferenceHub.OnPlayerAdded = (global::System.Action<ReferenceHub>)global::System.Delegate.Combine(ReferenceHub.OnPlayerAdded, (global::System.Action<ReferenceHub>)delegate (ReferenceHub hub)
            {
                if (global::Mirror.NetworkServer.active && !hub.isLocalPlayer)
                {
                    NetworkReaderPooled reader = global::Mirror.NetworkReaderPool.Get(new global::System.ArraySegment<byte>(SendHistory.buffer, 0, SendHistory.buffer.Length));
                    while (reader.Position < reader.buffer.Array.Length)
                    {
                        global::Respawning.SpawnableTeamType spawnableTeamType = (global::Respawning.SpawnableTeamType)reader.ReadByte();
                        if (!global::Respawning.NamingRules.UnitNamingRule.TryGetNamingRule(spawnableTeamType, out var rule))
                        {
                            break;
                        }
                        hub.connectionToClient.Send(new global::Respawning.NamingRules.UnitNameMessage
                        {
                            Data = reader,
                            NamingRule = rule,
                            Team = spawnableTeamType
                        });
                    }
                    reader.Dispose();
                }
            });
        }


        private static void ProcessMessage(global::Respawning.NamingRules.UnitNameMessage msg)
        {
            if (ReceivedNames.TryGetValue(msg.Team, out var value))
            {
                value.Add(msg.UnitName);
            }
            else
            {
                value = new global::System.Collections.Generic.List<string> { msg.UnitName };
                ReceivedNames.Add(msg.Team, value);
            }
            int num = value.Count - 1;
            global::Respawning.NamingRules.UnitNameMessageHandler.OnNameAdded?.Invoke(msg.Team, msg.UnitName, num);
            if (global::Mirror.NetworkServer.active && (!PregeneratedNameTeams.Contains(msg.Team) || num != 0))
            {
                msg.NamingRule.PlayEntranceAnnouncement(msg.UnitName);
            }
        }


        public static void SendNew(global::Respawning.SpawnableTeamType team, global::Respawning.NamingRules.UnitNamingRule rule)
        {
            global::Utils.Networking.NetworkUtils.SendToHubsConditionally(new global::Respawning.NamingRules.UnitNameMessage
            {
                Team = team,
                NamingRule = rule
            }, (ReferenceHub x) => true);
        }


        public static global::Respawning.NamingRules.UnitNameMessage ReadUnitName(this global::Mirror.NetworkReader reader)
        {
            global::Respawning.SpawnableTeamType spawnableTeamType = (global::Respawning.SpawnableTeamType)reader.ReadByte();
            if (!global::Respawning.NamingRules.UnitNamingRule.TryGetNamingRule(spawnableTeamType, out var rule))
            {
                throw new global::System.InvalidOperationException($"No compatible decoder detected to read the name of spawnable team: {spawnableTeamType}.");
            }
            return new global::Respawning.NamingRules.UnitNameMessage
            {
                Team = spawnableTeamType,
                NamingRule = rule,
                UnitName = rule.ReadName(reader)
            };
        }

        public static void WriteUnitName(this global::Mirror.NetworkWriter writer, global::Respawning.NamingRules.UnitNameMessage msg)
        {
            byte team = (byte)msg.Team;
            writer.WriteByte(team);
            if (msg.Data == null)
            {
                int position = writer.Position;
                msg.NamingRule.GenerateNew(writer);
                SendHistory.WriteByte(team);
                SendHistory.WriteBytes(writer.buffer, position, writer.Position - position);
            }
            else
            {
                int position2 = msg.Data.Position;
                msg.NamingRule.ReadName(msg.Data);
                writer.WriteBytes(msg.Data.buffer.Array, position2, msg.Data.Position - position2);
            }
        }
    }
}
