using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using PlayerRoles;
using PlayerStatsSystem;
using Subtitles;
using UnityEngine;

public class NineTailedFoxAnnouncer : MonoBehaviour
{
	[Serializable]
	public class VoiceLine
	{
		public string apiName;

		public AudioClip clip;

		public float length;

		public string collection;

        public static bool IsYield(string s, out float value)
        {
            if (s.StartsWith("YIELD_", global::System.StringComparison.OrdinalIgnoreCase) || s.StartsWith("YD_", global::System.StringComparison.OrdinalIgnoreCase))
            {
                string[] array = s.Split('_');
                if (array.Length == 2)
                {
                    return global::Utils.CustomParser.TryParseFloat(array[1], out value) != global::Utils.CustomParser.ParseResult.Failed;
                }
                value = 0f;
                return false;
            }
            value = 0f;
            return false;
        }

        public static bool IsJam(string s, out int percent, out int amount)
        {
            bool num = s.StartsWith("JAM_", global::System.StringComparison.OrdinalIgnoreCase);
            percent = 0;
            amount = 0;
            if (!num)
            {
                return false;
            }
            string[] array = s.Split('_');
            if (array.Length != 3)
            {
                return false;
            }
            if (global::Utils.CustomParser.TryParseInt(array[1], out var val) == global::Utils.CustomParser.ParseResult.Failed)
            {
                return false;
            }
            percent = val;
            if (global::Utils.CustomParser.TryParseInt(array[2], out val) == global::Utils.CustomParser.ParseResult.Failed)
            {
                return false;
            }
            amount = val;
            return true;
        }

        public static bool IsPitch(string s, out float value)
        {
            bool num = s.StartsWith("PITCH_", global::System.StringComparison.OrdinalIgnoreCase) || s.StartsWith("PI_", global::System.StringComparison.OrdinalIgnoreCase);
            value = 1f;
            if (!num)
            {
                return false;
            }
            string[] array = s.Split('_');
            if (array.Length < 2)
            {
                return false;
            }
            if (global::Utils.CustomParser.TryParseFloat(array[1], out var val) == global::Utils.CustomParser.ParseResult.Failed)
            {
                return false;
            }
            if (val > 0f)
            {
                value = val;
            }
            return true;
        }

        public static bool IsRegular(string s)
        {
            if (!IsYield(s, out var value) && !IsJam(s, out var _, out var _))
            {
                return !IsPitch(s, out value);
            }
            return false;
        }

        public string GetName()
		{
			return apiName;
		}
	}

	[Serializable]
	public struct ScpDeath : IEquatable<ScpDeath>
	{
		public List<RoleTypeId> scpSubjects;

		public string announcement;

		public SubtitlePart[] subtitleParts;

        public bool Equals(NineTailedFoxAnnouncer.ScpDeath other)
        {
            if (scpSubjects == other.scpSubjects)
            {
                return string.Equals(announcement, other.announcement);
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is NineTailedFoxAnnouncer.ScpDeath other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (((scpSubjects != null) ? scpSubjects.GetHashCode() : 0) * 397) ^ ((announcement != null) ? announcement.GetHashCode() : 0);
        }

        public static bool operator ==(NineTailedFoxAnnouncer.ScpDeath left, NineTailedFoxAnnouncer.ScpDeath right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NineTailedFoxAnnouncer.ScpDeath left, NineTailedFoxAnnouncer.ScpDeath right)
        {
            return !left.Equals(right);
        }
    }

    private class ItemEqualityComparer : global::System.Collections.Generic.IEqualityComparer<NineTailedFoxAnnouncer.VoiceLine>
    {
        public bool Equals(NineTailedFoxAnnouncer.VoiceLine x, NineTailedFoxAnnouncer.VoiceLine y)
        {
            if (x != null)
            {
                if (x.clip != null)
                {
                    return x.clip == y.clip;
                }
                return false;
            }
            return false;
        }

        public int GetHashCode(NineTailedFoxAnnouncer.VoiceLine obj)
        {
            return obj.clip.GetHashCode();
        }
    }

    public VoiceLine[] voiceLines;

	public AudioClip[] backgroundLines;

	public AudioClip suffixPluralStandard;

	public AudioClip suffixPluralException;

	public AudioClip suffixPastStandard;

	public AudioClip suffixPastException;

	public AudioClip suffixContinuous;

    public readonly global::System.Collections.Generic.List<NineTailedFoxAnnouncer.VoiceLine> queue = new global::System.Collections.Generic.List<NineTailedFoxAnnouncer.VoiceLine>();

    private global::System.Collections.Generic.List<string> newWords = new global::System.Collections.Generic.List<string>();

    private readonly global::System.Collections.Generic.List<NineTailedFoxAnnouncer.VoiceLine> newLines = new global::System.Collections.Generic.List<NineTailedFoxAnnouncer.VoiceLine>();

    private static readonly global::System.Collections.Generic.List<NineTailedFoxAnnouncer.ScpDeath> scpDeaths = new global::System.Collections.Generic.List<NineTailedFoxAnnouncer.ScpDeath>();

    public AudioSource speakerSource;

	public AudioSource backgroundSource;

	private readonly Regex UniqueKeyRegex = new Regex("(jam_\\d{1,3}_\\d{1,3})|(pitch_[\\d\\.]{1,4})|(\\.g\\d{1,3})|(bell_start)|(bell_end)", RegexOptions.IgnoreCase);

	private readonly Regex WhiteSpaceRegex = new Regex("\\s+");

	public static NineTailedFoxAnnouncer singleton;

	private float scpListTimer;

    public static string ConvertNumber(int num)
    {
        if (num <= 0)
        {
            return " 0 ";
        }
        ushort num2 = 0;
        byte b = 0;
        byte b2 = 0;
        while ((float)num / 1000f >= 1f)
        {
            num2++;
            num -= 1000;
        }
        while ((float)num / 100f >= 1f)
        {
            b++;
            num -= 100;
        }
        if (num >= 20)
        {
            while ((float)num / 10f >= 1f)
            {
                b2++;
                num -= 10;
            }
        }
        string text = string.Empty;
        if (num2 > 0)
        {
            text = text + ConvertNumber(num2) + " thousand ";
        }
        if (b > 0)
        {
            text = text + b + " hundred ";
        }
        if (b + num2 > 0 && (num > 0 || b2 > 0))
        {
            text += " and ";
        }
        if (b2 > 0)
        {
            text = text + b2 + "0 ";
        }
        if (num > 0)
        {
            text = text + num + " ";
        }
        return text;
    }

    [global::UnityEngine.RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        global::PlayerStatsSystem.PlayerStats.OnAnyPlayerDied += AnnounceScpTermination;
    }

    public static void AnnounceScpTermination(ReferenceHub scp, global::PlayerStatsSystem.DamageHandlerBase hit)
    {
        singleton.scpListTimer = 0f;
        if (!global::PlayerRoles.PlayerRolesUtils.IsSCP(scp, includeZombies: false))
        {
            return;
        }
        string announcement = hit.CassieDeathAnnouncement.Announcement;
        if (string.IsNullOrEmpty(announcement))
        {
            return;
        }
        foreach (NineTailedFoxAnnouncer.ScpDeath scpDeath in scpDeaths)
        {
            if (!(scpDeath.announcement != announcement))
            {
                scpDeath.scpSubjects.Add(global::PlayerRoles.PlayerRolesUtils.GetRoleId(scp));
                return;
            }
        }
        scpDeaths.Add(new NineTailedFoxAnnouncer.ScpDeath
        {
            scpSubjects = new global::System.Collections.Generic.List<global::PlayerRoles.RoleTypeId>(new global::PlayerRoles.RoleTypeId[1] { global::PlayerRoles.PlayerRolesUtils.GetRoleId(scp) }),
            announcement = announcement,
            subtitleParts = hit.CassieDeathAnnouncement.SubtitleParts
        });
    }

    public float CalculateDuration(string tts, bool rawNumber = false, float speed = 1f)
    {
        float num = 0f;
        string[] array = tts.Split(' ');
        for (int i = 0; i < array.Length; i++)
        {
            string text = array[i];
            if (NineTailedFoxAnnouncer.VoiceLine.IsJam(text, out var j, out var amount))
            {
                num += 0.13f * (float)amount;
                continue;
            }
            if (NineTailedFoxAnnouncer.VoiceLine.IsRegular(text))
            {
                bool flag = false;
                for (int k = i + 1; k < array.Length && !NineTailedFoxAnnouncer.VoiceLine.IsRegular(array[k]); k++)
                {
                    if (NineTailedFoxAnnouncer.VoiceLine.IsYield(array[k], out var value))
                    {
                        num += value;
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    continue;
                }
            }
            if (NineTailedFoxAnnouncer.VoiceLine.IsPitch(text, out var value2))
            {
                speed = value2;
                continue;
            }
            if (int.TryParse(text, out var result) && !rawNumber)
            {
                num += CalculateDuration(ConvertNumber(result), rawNumber: true, speed);
                continue;
            }
            bool flag2 = false;
            NineTailedFoxAnnouncer.VoiceLine[] array2 = voiceLines;
            for (j = 0; j < array2.Length; j++)
            {
                NineTailedFoxAnnouncer.VoiceLine voiceLine = array2[j];
                if (string.Equals(text, voiceLine.apiName, global::System.StringComparison.OrdinalIgnoreCase))
                {
                    flag2 = true;
                    num += voiceLine.length / speed;
                }
            }
            if (flag2 || text.Length <= 3)
            {
                continue;
            }
            for (byte b = 1; b <= 3; b++)
            {
                array2 = voiceLines;
                for (j = 0; j < array2.Length; j++)
                {
                    NineTailedFoxAnnouncer.VoiceLine voiceLine2 = array2[j];
                    if (string.Equals(text.Remove(text.Length - b), voiceLine2.apiName, global::System.StringComparison.OrdinalIgnoreCase))
                    {
                        num += voiceLine2.length / speed;
                    }
                }
            }
        }
        return num;
    }

    public void ServerOnlyAddGlitchyPhrase(string tts, float glitchChance, float jamChance)
    {
        string[] array = tts.Split(' ');
        newWords.Clear();
        newWords.EnsureCapacity(array.Length);
        for (int i = 0; i < array.Length; i++)
        {
            newWords.Add(array[i]);
            if (i < array.Length - 1)
            {
                if (global::UnityEngine.Random.value < glitchChance)
                {
                    newWords.Add(".G" + global::UnityEngine.Random.Range(1, 7));
                }
                if (global::UnityEngine.Random.value < jamChance)
                {
                    newWords.Add("JAM_" + global::UnityEngine.Random.Range(0, 70).ToString("000") + "_" + global::UnityEngine.Random.Range(2, 6));
                }
            }
        }
        tts = "";
        foreach (string newWord in newWords)
        {
            tts = tts + newWord + " ";
        }
        global::Respawning.RespawnEffectsController.PlayCassieAnnouncement(tts, makeHold: false, makeNoise: true);
    }

    public void ClearQueue()
	{
		queue.Clear();
		backgroundSource.Stop();
		speakerSource.Stop();
	}
    public void AddPhraseToQueue(string tts, bool generateNoise, bool rawNumber = false, bool makeHold = false, bool customAnnouncement = false)
    {
        string[] array = tts.Split(' ');
        if (!rawNumber)
        {
            float num = CalculateDuration(tts);
            if (customAnnouncement)
            {
                string subtitleText = WhiteSpaceRegex.Replace(UniqueKeyRegex.Replace(tts, string.Empty), " ").Trim();
                SubtitleController.Singleton?.AddSubtitle(subtitleText, Mathf.Max(5f, num), new Subtitle
                {
                    SubtitleCategory = CassieAnnouncementType.Normal,
                    Delay = generateNoise ? 3f : 0f
                });
            }
            int num2 = 0;
            for (int i = 0; i < backgroundLines.Length - 1; i++)
            {
                if ((float)i < num)
                {
                    num2 = i + 1;
                }
            }
            if (generateNoise)
            {
                queue.Add(new NineTailedFoxAnnouncer.VoiceLine
                {
                    apiName = "BG_BACKGROUND",
                    clip = backgroundLines[num2],
                    length = 2.5f
                });
            }
        }
        float num3 = 1f;
        string[] array2 = array;
        foreach (string text in array2)
        {
            if (!NineTailedFoxAnnouncer.VoiceLine.IsRegular(text))
            {
                queue.Add(new NineTailedFoxAnnouncer.VoiceLine
                {
                    apiName = text.ToUpper()
                });
                continue;
            }
            if (!rawNumber && float.TryParse(text, out var result))
            {
                AddPhraseToQueue(ConvertNumber((int)result), generateNoise: false, rawNumber: true);
                continue;
            }
            bool flag = false;
            NineTailedFoxAnnouncer.VoiceLine[] array3 = voiceLines;
            foreach (NineTailedFoxAnnouncer.VoiceLine voiceLine in array3)
            {
                if (string.Equals(text, voiceLine.apiName, global::System.StringComparison.OrdinalIgnoreCase))
                {
                    queue.Add(new NineTailedFoxAnnouncer.VoiceLine
                    {
                        apiName = voiceLine.apiName,
                        clip = voiceLine.clip,
                        length = voiceLine.length / num3
                    });
                    flag = true;
                }
            }
            if (flag)
            {
                continue;
            }
            NineTailedFoxAnnouncer.VoiceLine voiceLine2 = null;
            if (text.Length > 3)
            {
                for (byte b = 1; b <= 4; b++)
                {
                    if (text.Length > b)
                    {
                        array3 = voiceLines;
                        foreach (NineTailedFoxAnnouncer.VoiceLine voiceLine3 in array3)
                        {
                            if ((string.Equals(text.Remove(text.Length - b), voiceLine3.apiName, global::System.StringComparison.OrdinalIgnoreCase) || (text.EndsWith("ING", global::System.StringComparison.OrdinalIgnoreCase) && string.Equals(text.Remove(text.Length - b) + "E", voiceLine3.apiName, global::System.StringComparison.OrdinalIgnoreCase))) && voiceLine2 == null)
                            {
                                voiceLine2 = new NineTailedFoxAnnouncer.VoiceLine
                                {
                                    apiName = voiceLine3.apiName,
                                    clip = voiceLine3.clip,
                                    length = voiceLine3.length / num3
                                };
                            }
                        }
                    }
                }
            }
            if (voiceLine2 != null)
            {
                global::UnityEngine.AudioClip audioClip = ((!text.EndsWith("TED", global::System.StringComparison.OrdinalIgnoreCase) && !text.EndsWith("DED", global::System.StringComparison.OrdinalIgnoreCase)) ? (text.EndsWith("D", global::System.StringComparison.OrdinalIgnoreCase) ? suffixPastStandard : (text.EndsWith("ING", global::System.StringComparison.OrdinalIgnoreCase) ? suffixContinuous : ((!voiceLine2.apiName.EndsWith("S") && !voiceLine2.apiName.EndsWith("SH") && !voiceLine2.apiName.EndsWith("CH") && !voiceLine2.apiName.EndsWith("X") && !voiceLine2.apiName.EndsWith("Z")) ? suffixPluralStandard : suffixPluralException))) : suffixPastException);
                queue.Add(new NineTailedFoxAnnouncer.VoiceLine
                {
                    apiName = voiceLine2.apiName,
                    clip = voiceLine2.clip,
                    length = (voiceLine2.length - (text.EndsWith("ING", global::System.StringComparison.OrdinalIgnoreCase) ? 0.1f : 0.06f)) / num3
                });
                queue.Add(new NineTailedFoxAnnouncer.VoiceLine
                {
                    apiName = "SUFFIX_" + audioClip.name,
                    clip = audioClip,
                    length = audioClip.length / num3
                });
            }
        }
        if (!rawNumber)
        {
            queue.Add(new NineTailedFoxAnnouncer.VoiceLine
            {
                apiName = "PITCH_1"
            });
            for (byte b2 = 0; b2 < ((!makeHold) ? 1 : 3); b2++)
            {
                queue.Add(new NineTailedFoxAnnouncer.VoiceLine
                {
                    apiName = "END_OF_MESSAGE"
                });
            }
        }
    }

    private global::System.Collections.IEnumerator Start()
    {
        scpDeaths.Clear();
        singleton = this;
        float speed = 1f;
        int jammed = 0;
        int jamSize = 0;
        global::UnityEngine.WaitForEndOfFrame wait = new global::UnityEngine.WaitForEndOfFrame();
        while (this != null)
        {
            if (queue.Count == 0)
            {
                speed = 1f;
                yield return wait;
                continue;
            }
            NineTailedFoxAnnouncer.VoiceLine line = queue[0];
            queue.RemoveAt(0);
            if (line.apiName == "END_OF_MESSAGE")
            {
                speakerSource.pitch = 1f;
                yield return new global::UnityEngine.WaitForSeconds(4f);
                continue;
            }
            bool flag = line.apiName.StartsWith("BG_") || line.apiName.StartsWith("BELL_");
            bool flag2 = line.apiName.StartsWith("SUFFIX_");
            float absoluteTimeAddition = 0f;
            float relativeTimeAddition = 0f;
            float value;
            int percent;
            int amount;
            if (line.clip != null)
            {
                if (flag)
                {
                    backgroundSource.PlayOneShot(line.clip);
                }
                else if (flag2)
                {
                    speakerSource.Stop();
                    speakerSource.PlayOneShot(line.clip);
                }
                else if (jammed > 0)
                {
                    speakerSource.Stop();
                    float timeToJam = line.length * ((float)jammed / 100f);
                    speakerSource.clip = line.clip;
                    speakerSource.time = 0f;
                    speakerSource.Play();
                    yield return new global::UnityEngine.WaitForSeconds(timeToJam);
                    float stepSize = 0.13f;
                    for (int i = 0; i < jamSize; i++)
                    {
                        absoluteTimeAddition -= stepSize * 3f;
                        speakerSource.time = timeToJam;
                        yield return new global::UnityEngine.WaitForSeconds(stepSize);
                    }
                    jammed = 0;
                }
                else
                {
                    speakerSource.PlayOneShot(line.clip);
                }
            }
            else if (NineTailedFoxAnnouncer.VoiceLine.IsPitch(line.apiName, out value))
            {
                speed = value;
                speakerSource.pitch = speed;
            }
            else if (NineTailedFoxAnnouncer.VoiceLine.IsJam(line.apiName, out percent, out amount))
            {
                jammed = percent;
                jamSize = amount;
            }
            if (!NineTailedFoxAnnouncer.VoiceLine.IsRegular(line.apiName))
            {
                continue;
            }
            float num = 0f;
            for (int j = 0; j < queue.Count && !NineTailedFoxAnnouncer.VoiceLine.IsRegular(queue[j].apiName); j++)
            {
                if (NineTailedFoxAnnouncer.VoiceLine.IsYield(queue[j].apiName, out var value2))
                {
                    num = value2;
                    break;
                }
            }
            if (num > 0f)
            {
                yield return new global::UnityEngine.WaitForSeconds(num);
                continue;
            }
            float num2 = (line.length + relativeTimeAddition) / speed + absoluteTimeAddition;
            if (num2 > 0f)
            {
                yield return new global::UnityEngine.WaitForSeconds(num2);
            }
        }
    }

    private void Update()
    {
        if (scpDeaths.Count <= 0)
        {
            return;
        }
        scpListTimer += global::UnityEngine.Time.deltaTime;
        if (scpListTimer <= 1f)
        {
            return;
        }
        for (int i = 0; i < scpDeaths.Count; i++)
        {
            NineTailedFoxAnnouncer.ScpDeath scpDeath = scpDeaths[i];
            global::System.Collections.Generic.List<global::Subtitles.SubtitlePart> list = new global::System.Collections.Generic.List<global::Subtitles.SubtitlePart>(1);
            string text = "";
            for (int j = 0; j < scpDeath.scpSubjects.Count; j++)
            {
                ConvertSCP(scpDeath.scpSubjects[j], out var withoutSpace, out var withSpace);
                text = ((j != 0) ? (text + ". SCP " + withSpace) : (text + "SCP " + withSpace));
                list.Add(new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.SCP, withoutSpace));
            }
            text += scpDeath.announcement;
            if (scpDeath.subtitleParts != null)
            {
                list.AddRange(scpDeath.subtitleParts);
            }
            float num = (AlphaWarheadController.Detonated ? 3.5f : 1f);
            ServerOnlyAddGlitchyPhrase(text, global::UnityEngine.Random.Range(0.1f, 0.14f) * num, global::UnityEngine.Random.Range(0.07f, 0.08f) * num);
            global::Utils.Networking.NetworkUtils.SendToAuthenticated(new global::Subtitles.SubtitleMessage(list.ToArray()));
        }
        scpListTimer = 0f;
        scpDeaths.Clear();
    }

    public static void ConvertSCP(global::PlayerRoles.RoleTypeId role, out string withoutSpace, out string withSpace)
    {
        if (!global::PlayerRoles.PlayerRoleLoader.TryGetRoleTemplate<global::PlayerRoles.PlayerRoleBase>(role, out var result))
        {
            withoutSpace = string.Empty;
            withSpace = string.Empty;
        }
        else
        {
            ConvertSCP(result.RoleName, out withoutSpace, out withSpace);
        }
    }

    public static void ConvertSCP(string roleName, out string withoutSpace, out string withSpace)
    {
        global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
        string[] array = roleName.Split('-');
        if (array.Length < 2)
        {
            global::UnityEngine.Debug.LogError("Cassie role cannot be split by '-'. Possibly missing translation.");
            withoutSpace = "404";
            withSpace = "4 0 4";
            return;
        }
        withoutSpace = array[1];
        string text = withoutSpace;
        foreach (char value in text)
        {
            stringBuilder.Append(value);
            stringBuilder.Append(" ");
        }
        withSpace = global::NorthwoodLib.Pools.StringBuilderPool.Shared.ToStringReturn(stringBuilder);
    }

    public static string ConvertTeam(global::PlayerRoles.Team team, string unitName)
    {
        string result = "CONTAINMENTUNIT UNKNOWN";
        switch (team)
        {
            case global::PlayerRoles.Team.ClassD:
                return "BY CLASSD PERSONNEL";
            case global::PlayerRoles.Team.ChaosInsurgency:
                return "BY CHAOSINSURGENCY";
            case global::PlayerRoles.Team.FoundationForces:
                {
                    if (!global::Respawning.NamingRules.UnitNamingRule.TryGetNamingRule(global::Respawning.SpawnableTeamType.NineTailedFox, out var rule))
                    {
                        return result;
                    }
                    string cassieUnitName = rule.GetCassieUnitName(unitName);
                    return "CONTAINMENTUNIT " + cassieUnitName;
                }
            case global::PlayerRoles.Team.Scientists:
                return "BY SCIENCE PERSONNEL";
            default:
                return result;
        }
    }
}
