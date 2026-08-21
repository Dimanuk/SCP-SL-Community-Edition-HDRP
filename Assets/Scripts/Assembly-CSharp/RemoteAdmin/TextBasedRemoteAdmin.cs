using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NorthwoodLib;
using NorthwoodLib.Pools;
using PlayerRoles;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RemoteAdmin
{
    public class TextBasedRemoteAdmin : MonoBehaviour, IPointerClickHandler
    {
        internal readonly struct CommandDictionaryEntry
        {
            internal string Usage { get; }
            internal string Description { get; }

            public CommandDictionaryEntry(string usage, string description)
            {
                Usage = usage;
                Description = description;
            }
        }

        private class CommandDistanceComparer : IComparer<CommandDistance>
        {
            int IComparer<CommandDistance>.Compare(CommandDistance x, CommandDistance y) => x.CompareTo(y);
        }

        private struct CommandDistance : IComparable<CommandDistance>, IEquatable<CommandDistance>
        {
            public QueryProcessor.CommandData command;
            public int _distance;

            public CommandDistance(QueryProcessor.CommandData command, int distance)
            {
                this.command = command;
                this._distance = distance;
            }

            public int CompareTo(CommandDistance other)
            {
                int distCompare = this._distance.CompareTo(other._distance);
                if (distCompare != 0)
                    return distCompare;
                return string.Compare(this.command.Command, other.command.Command, StringComparison.Ordinal);
            }

            public bool Equals(CommandDistance other)
            {
                return this._distance == other._distance && object.Equals(this.command, other.command);
            }

            public override bool Equals(object obj) => obj is CommandDistance other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((this.command.Command != null ? this.command.Command.GetHashCode() : 0) * 397) ^ this._distance;
                }
            }

            public static bool operator <(CommandDistance left, CommandDistance right) => left.CompareTo(right) < 0;
            public static bool operator >(CommandDistance left, CommandDistance right) => left.CompareTo(right) > 0;
            public static bool operator <=(CommandDistance left, CommandDistance right) => left.CompareTo(right) <= 0;
            public static bool operator >=(CommandDistance left, CommandDistance right) => left.CompareTo(right) >= 0;
            public static bool operator ==(CommandDistance left, CommandDistance right) => left.Equals(right);
            public static bool operator !=(CommandDistance left, CommandDistance right) => !left.Equals(right);
        }

        public static TextBasedRemoteAdmin singleton;

        public TextMeshProUGUI consoleWindow;
        public TMP_InputField commandField;

        private UIController _ui;

        public GameObject commandSuggest;
        public TextMeshProUGUI commandSuggestion;

        private List<KeyValuePair<string, CommandDictionaryEntry>> _clientLastCommandSearchResults = new List<KeyValuePair<string, CommandDictionaryEntry>>();
        private int _clientLastCommandIndex;
        private int _clientCommandPosition;
        private readonly List<string> _clientCommandLogs = new List<string>();

        public static readonly List<QueryProcessor.CommandData> Commands = new List<QueryProcessor.CommandData>();
        private static readonly string[] AllAliases = new string[] { "@all", "*", "@everyone", "@a" };
        private static readonly string[] SelfAliases = new string[] { "@me", "@self" };
        private static readonly HashSet<string> ClearCommands = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "clr", "cls", "clearconsole", "clean" };
        private static readonly CommandDistanceComparer _cdComparer = new CommandDistanceComparer();
        private static readonly List<CommandDistance> _distances = new List<CommandDistance>();
        private static readonly Dictionary<string, CommandDistance> _addedDistances = new Dictionary<string, CommandDistance>();

        private readonly List<string> _logs = new List<string>();

        private void Start()
        {
            _ui = base.GetComponent<UIController>();
            _logs.Add($"[{DateTime.Now:HH:mm:ss}] Remote Admin console initialized.");
            RefreshConsole();

            commandField.onValueChanged.AddListener(_ => CommandFieldOnValueChanged());
            // Enter/KeypadEnter deactivates the TMP field before our Update() can poll it
            // (EventSystem runs first under Unity 6), so submit straight from the field too.
            commandField.onSubmit.AddListener(_ => SendCommand());
        }

        private void Awake()
        {
            Commands.Clear();
            singleton = this;
            commandSuggestion.gameObject.SetActive(false);
        }

        public static void AddLog(string log)
        {
            if (singleton == null)
                return;

            if (Menus.RaSettings.Singleton != null && Menus.RaSettings.Singleton.ToggleTimestamps.Value)
            {
                log = $"[{DateTime.Now:HH:mm:ss}] {log}";
            }

            singleton._logs.Add(log);
            singleton.RefreshConsole();
        }

        private void RefreshConsole()
        {
            var sb = StringBuilderPool.Shared.Rent();

            foreach (var log in _logs)
                sb.AppendLine(log);

            consoleWindow.text = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
        }

        private void Update()
        {
            if (!_ui.LoggedIn || !_ui.LockMovement || !_ui.root_tbra.activeSelf)
                return;

            // ��������� ������ �� ������������ � �������
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(consoleWindow, Input.mousePosition, null);
            if (linkIndex != -1)
            {
                var linkInfo = consoleWindow.textInfo.linkInfo[linkIndex];
                string linkId = linkInfo.GetLinkID();

                if (!string.IsNullOrEmpty(linkId))
                {
                    if (linkId == "TBRA_CommandFail")
                        UIController.Singleton?.SetToolTip("The command failed to execute! Check the error message for details.", 0.1f, false);
                    else if (linkId == "TBRA_CommandSuccess")
                        UIController.Singleton?.SetToolTip("The command executed successfully.", 0.1f, false);
                    else if (linkId == "TBRA_EncryptionError")
                        UIController.Singleton?.SetToolTip("This response was not encrypted and cannot be verified as a legitimate message.", 0.1f, false);
                    else if (linkId == "TBRA_InternalError")
                        UIController.Singleton?.SetToolTip("Internal system error within RA. This is usually a security violation/check.", 0.1f, false);
                    else
                        UIController.Singleton?.SetToolTip("Click to copy.", 0.1f, false);
                }
            }

            // ��������� �� ������� (������� �����/����)
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (_clientCommandPosition < _clientCommandLogs.Count - 1)
                {
                    _clientCommandPosition++;
                    UpdateCommandFieldFromHistory();
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (_clientCommandPosition > 0)
                {
                    _clientCommandPosition--;
                    UpdateCommandFieldFromHistory();
                }
                else
                {
                    _clientCommandPosition = 0;
                    commandField.text = string.Empty;
                }
            }

            // Tab � ��������������
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                HandleTabCompletion();
            }

            // Enter � ��������
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (!string.IsNullOrEmpty(commandField.text))
                    SendCommand();
            }
        }

        private void UpdateCommandFieldFromHistory()
        {
            if (_clientCommandLogs.Count == 0)
                return;

            int index = _clientCommandLogs.Count - 1 - _clientCommandPosition;
            if (index >= 0 && index < _clientCommandLogs.Count)
            {
                commandField.text = _clientCommandLogs[index];
                commandField.MoveToEndOfLine(false, false);
            }
        }

        private void HandleTabCompletion()
        {
            string text = commandField.text;
            if (string.IsNullOrEmpty(text))
                return;

            // ���� ������� ���� ����������� � � ��� >1 �������� � ����������� �� ���
            if (commandSuggest.activeSelf && _clientLastCommandSearchResults.Count > 1)
            {
                bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                if (shift)
                {
                    _clientLastCommandIndex--;
                    if (_clientLastCommandIndex < 0)
                        _clientLastCommandIndex = _clientLastCommandSearchResults.Count - 1;
                }
                else
                {
                    _clientLastCommandIndex++;
                    if (_clientLastCommandIndex >= _clientLastCommandSearchResults.Count)
                        _clientLastCommandIndex = 0;
                }

                SetCommandSuggest(_clientLastCommandSearchResults, _clientLastCommandIndex);

                int lastSpace = text.LastIndexOf(' ');
                string prefix = lastSpace >= 0 ? text.Substring(0, lastSpace + 1) : string.Empty;
                string suggestion = _clientLastCommandSearchResults[_clientLastCommandIndex].Key;
                commandField.SetTextWithoutNotify(prefix + suggestion);
                commandField.MoveToEndOfLine(false, false);
                return;
            }

            // ����� �������� ���������� ����� ��� ����� ������
            int lastSpaceIdx = text.LastIndexOf(' ');
            string lastWord = lastSpaceIdx >= 0 ? text.Substring(lastSpaceIdx + 1) : text;

            if (ShouldProcessAllPlayers(lastWord))
            {
                ReplaceLastWord("@all.");
                return;
            }

            if (ShouldProcessSelf(lastWord))
            {
                ReplaceLastWord("@me.");
                return;
            }

            if (!string.IsNullOrEmpty(lastWord))
                LookupPlayer(lastWord);
        }

        private void ReplaceLastWord(string replacement)
        {
            string text = commandField.text;
            int lastSpace = text.LastIndexOf(' ');
            string prefix = lastSpace >= 0 ? text.Substring(0, lastSpace + 1) : string.Empty;
            commandField.text = prefix + replacement;
            commandField.MoveToEndOfLine(false, false);
        }

        public void SendCommand()
        {
            string text = commandField.text;
            if (string.IsNullOrEmpty(text))
                return;

            _clientCommandLogs.Add(text);
            _clientCommandPosition = 0;

            string lower = text.ToLowerInvariant();

            if (ClearCommands.Contains(lower))
            {
                _logs.Clear();
                _logs.Add($"[{DateTime.Now:HH:mm:ss}] Console cleared.");
                commandField.text = string.Empty;
                RefreshConsole();
            }
            else
            {
                string query = text.TrimStart('$');
                if (!string.IsNullOrEmpty(query))
                {
                    ReferenceHub.LocalHub.queryProcessor.CmdSendQuery(query, false);
                }
                commandField.text = string.Empty;
            }

            commandField.ActivateInputField();
        }

        private void CommandFieldOnValueChanged()
        {
            if (Menus.RaSettings.Singleton == null || !Menus.RaSettings.Singleton.ToggleSuggestions.Value)
                return;

            string text = commandField.text;
            if (string.IsNullOrEmpty(text))
            {
                HideSuggestions();
                return;
            }

            string[] parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                HideSuggestions();
                return;
            }

            string cmd = parts[0].ToLowerInvariant();

            // ���� ������� ������ ������� (��� ����������) � �������� �����
            if (parts.Length == 1)
            {
                FuzzySearchCommand(cmd);
                if (_distances.Count > 0)
                {
                    var suggestions = GetDictionaryFromCommands(_distances);
                    SetCommandSuggest(suggestions, 0);
                    _distances.Clear();
                    return;
                }
                HideSuggestions();
                return;
            }

            // ���� ��������� � ���� ������ ������� (�������� ��� �����)
            var found = Commands.FirstOrDefault(c => c.Command.Equals(cmd, StringComparison.OrdinalIgnoreCase));

            // ���� ����� ����� � ����������� �������� ������� ��� Usage/Description
            if (found.Command != null && !string.IsNullOrEmpty(found.AliasOf))
            {
                string mainName = found.AliasOf.ToLowerInvariant();
                found = Commands.FirstOrDefault(c => c.Command.Equals(mainName, StringComparison.OrdinalIgnoreCase));
            }

            if (found.Command == null)
            {
                // ��������, �������� ����� � ��� ������ �������, �� ��� � ����������
                FuzzySearchCommand(cmd);
                if (_distances.Count > 0)
                {
                    var suggestions = GetDictionaryFromCommands(_distances);
                    SetCommandSuggest(suggestions, 0);
                    _distances.Clear();
                }
                else
                {
                    HideSuggestions();
                }
                return;
            }

            // ��������� ����������
            int argId = parts.Length - 1; // ������ �������� ��������� (0-based ��� Usage)

            // ����������� ������ %player%
            if (parts.Length == 2 && parts[1] == "%player%")
            {
                var players = Utils.RAUtils.ProcessPlayerIdOrNamesList(new[] { parts[1] }, 0, out _, true);
                if (players != null && players.Count > 0)
                {
                    var dict = GetDictionaryFromArg(found, 0);
                    if (dict.Count > 0)
                    {
                        SetCommandSuggest(dict, 0);
                        return;
                    }
                }
            }

            // ����������� ��������� ��� ���������
            var argSuggestions = GetDictionaryFromArg(found, argId);
            if (argSuggestions != null && argSuggestions.Count > 0)
            {
                SetCommandSuggest(argSuggestions, 0);
                return;
            }

            HideSuggestions();
        }

        private void HideSuggestions()
        {
            commandSuggest.SetActive(false);
            commandSuggestion.gameObject.SetActive(false);
            _clientLastCommandSearchResults.Clear();
            _clientLastCommandIndex = 0;
        }

        private void SetCommandSuggest(List<KeyValuePair<string, CommandDictionaryEntry>> display, int highlightId = -1)
        {
            _clientLastCommandSearchResults = display ?? new List<KeyValuePair<string, CommandDictionaryEntry>>();

            if (_clientLastCommandSearchResults.Count == 0)
            {
                HideSuggestions();
                return;
            }

            // ������������ �� 22 ���������
            var limited = _clientLastCommandSearchResults.Take(22).ToList();
            var sb = StringBuilderPool.Shared.Rent();

            for (int i = 0; i < limited.Count; i++)
            {
                bool highlighted = i == highlightId;
                var entry = limited[i];

                if (highlighted)
                    sb.Append("<color=yellow>");
                else
                    sb.Append("<color=white>");

                sb.Append(entry.Key);
                sb.Append("</color>");

                if (!string.IsNullOrWhiteSpace(entry.Value.Usage))
                    sb.AppendFormat(" <color=grey>{0}</color>", entry.Value.Usage);

                // Only the highlighted entry gets its description on a second line (matches the shipped
                // client). Showing a description under EVERY command doubled the list height and made it
                // overflow past the top of the RA window / screen.
                if (highlighted && !string.IsNullOrWhiteSpace(entry.Value.Description))
                    sb.AppendFormat("\n  <size=8>{0}</size>", entry.Value.Description);

                sb.AppendLine();
            }

            commandSuggestion.text = sb.ToString();
            StringBuilderPool.Shared.Return(sb);

            commandSuggest.SetActive(true);
            commandSuggestion.gameObject.SetActive(true);
        }

        private List<KeyValuePair<string, CommandDictionaryEntry>> GetDictionaryFromArg(QueryProcessor.CommandData command, int argId)
        {
            var list = new List<KeyValuePair<string, CommandDictionaryEntry>>();
            string currentArg = string.Empty;

            string text = commandField.text;
            int lastSpace = text.LastIndexOf(' ');
            if (lastSpace >= 0)
                currentArg = text.Substring(lastSpace + 1);

            if (command.Usage == null || argId >= command.Usage.Length)
                return list;

            string usageHint = command.Usage[argId];

            // ��������� �����
            if (usageHint.IndexOf("role", StringComparison.OrdinalIgnoreCase) >= 0 ||
                usageHint == "[Role Name]")
            {
                foreach (var role in PlayerRoleLoader.AllRoles)
                {
                    string roleName = role.Key.ToString();
                    if (StringUtils.Contains(roleName, currentArg, StringComparison.OrdinalIgnoreCase))
                    {
                        list.Add(new KeyValuePair<string, CommandDictionaryEntry>(
                            roleName,
                            new CommandDictionaryEntry(roleName, role.Value?.GetType().Name ?? "Player Role")));
                    }
                }
            }

            if (usageHint.IndexOf("door", StringComparison.OrdinalIgnoreCase) >= 0 ||
                usageHint == "[Door Name]")
            {
                foreach (var door in Interactables.Interobjects.DoorUtils.DoorNametagExtension.NamedDoors.Values)
                {
                    if (StringUtils.Contains(door.GetName, currentArg, StringComparison.OrdinalIgnoreCase))
                    {
                        list.Add(new KeyValuePair<string, CommandDictionaryEntry>(
                            door.GetName,
                            new CommandDictionaryEntry(door.GetName, "Door")));
                    }
                }
            }

            return list;
        }

        private static List<KeyValuePair<string, CommandDictionaryEntry>> GetDictionaryFromCommands(List<CommandDistance> commands)
        {
            var list = new List<KeyValuePair<string, CommandDictionaryEntry>>();
            foreach (var cd in commands.OrderBy(d => d._distance).ThenBy(d => d.command.Command))
            {
                string usage = UsageArrayToHumanReadableUsage(cd.command.Usage);
                list.Add(new KeyValuePair<string, CommandDictionaryEntry>(
                    cd.command.Command,
                    new CommandDictionaryEntry(usage, cd.command.Description)));
            }
            return list;
        }

        private static List<KeyValuePair<string, CommandDictionaryEntry>> GetDictionaryFromCommands(List<QueryProcessor.CommandData> commands)
        {
            var list = new List<KeyValuePair<string, CommandDictionaryEntry>>();
            foreach (var cmd in commands)
            {
                string usage = UsageArrayToHumanReadableUsage(cmd.Usage);
                list.Add(new KeyValuePair<string, CommandDictionaryEntry>(
                    cmd.Command,
                    new CommandDictionaryEntry(usage, cmd.Description)));
            }
            return list;
        }

        private static string UsageArrayToHumanReadableUsage(IEnumerable<string> usage)
        {
            if (usage == null)
                return string.Empty;

            var sb = StringBuilderPool.Shared.Rent();
            foreach (var u in usage)
            {
                if (sb.Length > 0)
                    sb.Append(' ');

                switch (u)
                {
                    case "[PlayerID or Name]":
                        sb.Append("<color=orange>[PlayerID/Name]</color>");
                        break;
                    case "[Role Name]":
                        sb.Append("<color=green>[Role]</color>");
                        break;
                    case "[Door Name]":
                        sb.Append("<color=blue>[Door]</color>");
                        break;
                    default:
                        sb.AppendFormat("[{0}]", u);
                        break;
                }
            }

            string result = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
            return result;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_ui.root_tbra.activeSelf)
                return;

            int linkIndex = TMP_TextUtilities.FindIntersectingLink(consoleWindow, Input.mousePosition, null);
            if (linkIndex != -1)
            {
                var linkInfo = consoleWindow.textInfo.linkInfo[linkIndex];
                string linkId = linkInfo.GetLinkID();

                if (!string.IsNullOrEmpty(linkId))
                {
                    GUIUtility.systemCopyBuffer = linkId;
                    UIController.Singleton?.SetToolTip("Copied!", 1f, false);
                }
            }
        }

        private static void FuzzySearchCommand(string partialCommand)
        {
            _distances.Clear();
            _addedDistances.Clear();

            if (string.IsNullOrEmpty(partialCommand))
                return;

            string lowerPartial = partialCommand.ToLowerInvariant();
            int partialLen = partialCommand.Length;

            foreach (var cmd in Commands)
            {
                string cmdName = cmd.Command;
                if (string.IsNullOrEmpty(cmdName))
                    continue;

                bool startsWith = cmdName.StartsWith(partialCommand, StringComparison.OrdinalIgnoreCase);
                int distance;

                if (!startsWith)
                {
                    string lcs = LongestCommonSubstring(lowerPartial, cmdName.ToLowerInvariant());
                    int lev = LevenshteinDistance(lowerPartial, cmdName.ToLowerInvariant());
                    distance = lev - lcs.Length + 2;
                }
                else
                {
                    distance = int.MaxValue; // ������ ���������� ����������� ��������
                }

                if (_addedDistances.TryGetValue(cmdName, out var existing))
                {
                    if (distance < existing._distance)
                    {
                        _distances.Remove(existing);
                        var better = new CommandDistance(cmd, distance);
                        _distances.Add(better);
                        _addedDistances[cmdName] = better;
                    }
                }
                else
                {
                    var cd = new CommandDistance(cmd, distance);
                    _distances.Add(cd);
                    _addedDistances[cmdName] = cd;
                }
            }

            _distances.Sort(_cdComparer);
        }

        private static string LongestCommonSubstring(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                return string.Empty;

            int[,] matrix = new int[a.Length, b.Length];
            int maxLen = 0;
            int endIndex = 0;

            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    if (a[i] == b[j])
                    {
                        matrix[i, j] = (i == 0 || j == 0) ? 1 : matrix[i - 1, j - 1] + 1;
                        if (matrix[i, j] > maxLen)
                        {
                            maxLen = matrix[i, j];
                            endIndex = i;
                        }
                    }
                }
            }

            return maxLen > 0 ? a.Substring(endIndex - maxLen + 1, maxLen) : string.Empty;
        }

        private static int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return b?.Length ?? 0;
            if (string.IsNullOrEmpty(b)) return a.Length;

            int[,] d = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) d[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[a.Length, b.Length];
        }

        private bool ShouldProcessAllPlayers(string args)
        {
            foreach (var alias in AllAliases)
            {
                if (StringUtils.Contains(args, alias, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static bool ShouldProcessSelf(string args)
        {
            foreach (var alias in SelfAliases)
            {
                if (StringUtils.Contains(args, alias, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private void LookupPlayer(string arg)
        {
            var matches = ReferenceHub.AllHubs.Where(h =>
                h.nicknameSync.MyNick.Contains(arg, StringComparison.OrdinalIgnoreCase) ||
                h.PlayerId.ToString() == arg).ToList();

            if (matches.Count == 0)
                return;

            if (matches.Count == 1)
            {
                var hub = matches[0];
                string nick = hub.nicknameSync.MyNick;
                string formatted;

                if (nick.Contains('"') || nick.Contains('.') || nick.Contains(' '))
                    formatted = $"@\"{nick}\".";
                else
                    formatted = $"@{nick}.";

                int lastSpace = commandField.text.LastIndexOf(' ');
                string prefix = lastSpace >= 0 ? commandField.text.Substring(0, lastSpace + 1) : string.Empty;

                commandField.text = prefix + formatted;
                commandField.MoveToEndOfLine(false, false);
            }
            else
            {
                AddLog("Multiple matches found!");
                AddLog("-----------------------------------------");
                foreach (var hub in matches)
                    AddLog($"[{hub.PlayerId}] {hub.nicknameSync.MyNick}");
                AddLog("-----------------------------------------");
            }
        }
    }
}