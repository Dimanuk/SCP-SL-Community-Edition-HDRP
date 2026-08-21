using System.Collections.Generic;

using Mirror;

namespace Respawning.NamingRules
{
	public class NineTailedFoxNamingRule : UnitNamingRule
	{
        private readonly global::System.Collections.Generic.HashSet<int> _usedCombos = new global::System.Collections.Generic.HashSet<int>();

        private static readonly string[] PossibleCodes = new string[26]
        {
            "ALPHA", "BRAVO", "CHARLIE", "DELTA", "ECHO", "FOXTROT", "GOLF", "HOTEL", "INDIA", "JULIETT",
            "KILO", "LIMA", "MIKE", "NOVEMBER", "OSCAR", "PAPA", "QUEBEC", "ROMEO", "SIERRA", "TANGO",
            "UNIFORM", "VICTOR", "WHISKEY", "XRAY", "YANKEE", "ZULU"
        };

        public override void GenerateNew(global::Mirror.NetworkWriter writer)
        {
            int num;
            int num2;
            do
            {
                num = global::UnityEngine.Random.Range(0, PossibleCodes.Length - 1);
                num2 = global::UnityEngine.Random.Range(1, 19);
            }
            while (!_usedCombos.Add(num * 255 + num2));
            writer.WriteByte((byte)num);
            writer.WriteByte((byte)num2);
        }

        public override string ReadName(global::Mirror.NetworkReader reader)
        {
            return PossibleCodes[reader.ReadByte()] + "-" + reader.ReadByte().ToString("00");
        }

        public override void PlayEntranceAnnouncement(string regular)
        {
            string cassieUnitName = GetCassieUnitName(regular);
            int num = global::System.Linq.Enumerable.Count(ReferenceHub.AllHubs, (ReferenceHub x) => global::PlayerRoles.PlayerRolesUtils.IsSCP(x, includeZombies: false));
            global::System.Text.StringBuilder sb = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
            if (ClutterSpawner.IsHolidayActive(Holidays.Christmas))
            {
                sb.Append("XMAS_EPSILON11 ");
                sb.Append(cassieUnitName);
                sb.Append(" XMAS_HASENTERED ");
                sb.Append(num);
                sb.Append(" XMAS_SCPSUBJECTS");
            }
            else
            {
                sb.Append("MTFUNIT EPSILON 11 DESIGNATED ");
                sb.Append(cassieUnitName);
                sb.Append(" HASENTERED ALLREMAINING ");
                if (num == 0)
                {
                    sb.Append("NOSCPSLEFT");
                }
                else
                {
                    sb.Append("AWAITINGRECONTAINMENT ");
                    sb.Append(num);
                    if (num == 1)
                    {
                        sb.Append(" SCPSUBJECT");
                    }
                    else
                    {
                        sb.Append(" SCPSUBJECTS");
                    }
                }
            }
            global::System.Collections.Generic.List<global::Subtitles.SubtitlePart> list = new global::System.Collections.Generic.List<global::Subtitles.SubtitlePart>();
            list.Add(new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.NTFEntrance, regular));
            global::System.Collections.Generic.List<global::Subtitles.SubtitlePart> list2 = list;
            switch (num)
            {
                case 0:
                    list2.Add(new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.ThreatRemains, (string[])null));
                    break;
                case 1:
                    list2.Add(new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.AwaitContainSingle, (string[])null));
                    break;
                default:
                    list2.Add(new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.AwaitContainPlural, num.ToString()));
                    break;
            }
            global::Utils.Networking.NetworkUtils.SendToAuthenticated(new global::Subtitles.SubtitleMessage(list2.ToArray()));
            ConfirmAnnouncement(ref sb);
            global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(sb);
        }

        public override string GetCassieUnitName(string regular)
        {
            try
            {
                string[] array = regular.Split('-');
                return "NATO_" + array[0][0] + " " + array[1];
            }
            catch
            {
                ServerConsole.AddLog("Error, couldn't convert '" + regular + "' into a CASSIE-readable form.");
                return "ERROR";
            }
        }
	}
}
