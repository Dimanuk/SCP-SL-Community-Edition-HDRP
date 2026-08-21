public class YamlConfig : global::Utils.ConfigHandler.ConfigRegister
{
    private static readonly string[] _rolevars = new string[6] { "color", "badge", "cover", "hidden", "kick_power", "required_kick_power" };

    private static readonly string[] _deprecatedconfigs = new string[1] { "administrator_password" };

    private bool _afteradding;

    private bool _virtual;

    private string[] _rawDataUnfiltered;

    private string[] _rawData;

    private static readonly global::System.Collections.Generic.List<string> DataBuffer = new global::System.Collections.Generic.List<string>();

    public string Path;

    public string[] RawData
    {
        get
        {
            if (!_virtual)
            {
                return _rawData;
            }
            return _rawDataUnfiltered;
        }
        set
        {
            if (_virtual)
            {
                _rawDataUnfiltered = value;
            }
            else
            {
                _rawData = value;
            }
        }
    }

    public bool IsVirtual
    {
        get
        {
            return _virtual;
        }
        set
        {
            if (value && !_virtual)
            {
                _virtual = true;
                _rawDataUnfiltered = RawData;
            }
        }
    }

    public YamlConfig()
    {
        RawData = new string[0];
    }

    public YamlConfig(string path)
    {
        Path = path;
        LoadConfigFile(path);
    }

    public override void UpdateConfigValue(global::Utils.ConfigHandler.ConfigEntry configEntry)
    {
        if (configEntry == null)
        {
            throw new global::System.NullReferenceException("Config type unsupported (Config: Null).");
        }
        if (configEntry != null)
        {
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<bool> configEntry2)
            {
                global::Utils.ConfigHandler.ConfigEntry<bool> configEntry3 = configEntry2;
                configEntry3.Value = GetBool(configEntry3.Key, configEntry3.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<byte> configEntry4)
            {
                global::Utils.ConfigHandler.ConfigEntry<byte> configEntry5 = configEntry4;
                configEntry5.Value = GetByte(configEntry5.Key, configEntry5.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<char> configEntry6)
            {
                global::Utils.ConfigHandler.ConfigEntry<char> configEntry7 = configEntry6;
                configEntry7.Value = GetChar(configEntry7.Key, configEntry7.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<decimal> configEntry8)
            {
                global::Utils.ConfigHandler.ConfigEntry<decimal> configEntry9 = configEntry8;
                configEntry9.Value = GetDecimal(configEntry9.Key, configEntry9.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<double> configEntry10)
            {
                global::Utils.ConfigHandler.ConfigEntry<double> configEntry11 = configEntry10;
                configEntry11.Value = GetDouble(configEntry11.Key, configEntry11.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<float> configEntry12)
            {
                global::Utils.ConfigHandler.ConfigEntry<float> configEntry13 = configEntry12;
                configEntry13.Value = GetFloat(configEntry13.Key, configEntry13.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<int> configEntry14)
            {
                global::Utils.ConfigHandler.ConfigEntry<int> configEntry15 = configEntry14;
                configEntry15.Value = GetInt(configEntry15.Key, configEntry15.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<long> configEntry16)
            {
                global::Utils.ConfigHandler.ConfigEntry<long> configEntry17 = configEntry16;
                configEntry17.Value = GetLong(configEntry17.Key, configEntry17.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<sbyte> configEntry18)
            {
                global::Utils.ConfigHandler.ConfigEntry<sbyte> configEntry19 = configEntry18;
                configEntry19.Value = GetSByte(configEntry19.Key, configEntry19.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<short> configEntry20)
            {
                global::Utils.ConfigHandler.ConfigEntry<short> configEntry21 = configEntry20;
                configEntry21.Value = GetShort(configEntry21.Key, configEntry21.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<string> configEntry22)
            {
                global::Utils.ConfigHandler.ConfigEntry<string> configEntry23 = configEntry22;
                configEntry23.Value = GetString(configEntry23.Key, configEntry23.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<uint> configEntry24)
            {
                global::Utils.ConfigHandler.ConfigEntry<uint> configEntry25 = configEntry24;
                configEntry25.Value = GetUInt(configEntry25.Key, configEntry25.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<ulong> configEntry26)
            {
                global::Utils.ConfigHandler.ConfigEntry<ulong> configEntry27 = configEntry26;
                configEntry27.Value = GetULong(configEntry27.Key, configEntry27.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<ushort> configEntry28)
            {
                global::Utils.ConfigHandler.ConfigEntry<ushort> configEntry29 = configEntry28;
                configEntry29.Value = GetUShort(configEntry29.Key, configEntry29.Default);
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<bool>> configEntry30)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<bool>> configEntry31 = configEntry30;
                configEntry31.Value = GetBoolList(configEntry31.Key);
                if (configEntry31.Value.Count <= 0 && string.Equals(GetRawString(configEntry31.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry31.Value = configEntry31.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<byte>> configEntry32)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<byte>> configEntry33 = configEntry32;
                configEntry33.Value = GetByteList(configEntry33.Key);
                if (configEntry33.Value.Count <= 0 && string.Equals(GetRawString(configEntry33.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry33.Value = configEntry33.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<char>> configEntry34)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<char>> configEntry35 = configEntry34;
                configEntry35.Value = GetCharList(configEntry35.Key);
                if (configEntry35.Value.Count <= 0 && string.Equals(GetRawString(configEntry35.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry35.Value = configEntry35.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<decimal>> configEntry36)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<decimal>> configEntry37 = configEntry36;
                configEntry37.Value = GetDecimalList(configEntry37.Key);
                if (configEntry37.Value.Count <= 0 && string.Equals(GetRawString(configEntry37.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry37.Value = configEntry37.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<double>> configEntry38)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<double>> configEntry39 = configEntry38;
                configEntry39.Value = GetDoubleList(configEntry39.Key);
                if (configEntry39.Value.Count <= 0 && string.Equals(GetRawString(configEntry39.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry39.Value = configEntry39.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<float>> configEntry40)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<float>> configEntry41 = configEntry40;
                configEntry41.Value = GetFloatList(configEntry41.Key);
                if (configEntry41.Value.Count <= 0 && string.Equals(GetRawString(configEntry41.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry41.Value = configEntry41.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<int>> configEntry42)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<int>> configEntry43 = configEntry42;
                configEntry43.Value = GetIntList(configEntry43.Key);
                if (configEntry43.Value.Count <= 0 && string.Equals(GetRawString(configEntry43.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry43.Value = configEntry43.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<long>> configEntry44)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<long>> configEntry45 = configEntry44;
                configEntry45.Value = GetLongList(configEntry45.Key);
                if (configEntry45.Value.Count <= 0 && string.Equals(GetRawString(configEntry45.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry45.Value = configEntry45.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<sbyte>> configEntry46)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<sbyte>> configEntry47 = configEntry46;
                configEntry47.Value = GetSByteList(configEntry47.Key);
                if (configEntry47.Value.Count <= 0 && string.Equals(GetRawString(configEntry47.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry47.Value = configEntry47.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<short>> configEntry48)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<short>> configEntry49 = configEntry48;
                configEntry49.Value = GetShortList(configEntry49.Key);
                if (configEntry49.Value.Count <= 0 && string.Equals(GetRawString(configEntry49.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry49.Value = configEntry49.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<string>> configEntry50)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<string>> configEntry51 = configEntry50;
                configEntry51.Value = GetStringList(configEntry51.Key);
                if (configEntry51.Value.Count <= 0 && string.Equals(GetRawString(configEntry51.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry51.Value = configEntry51.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<uint>> configEntry52)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<uint>> configEntry53 = configEntry52;
                configEntry53.Value = GetUIntList(configEntry53.Key);
                if (configEntry53.Value.Count <= 0 && string.Equals(GetRawString(configEntry53.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry53.Value = configEntry53.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<ulong>> configEntry54)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<ulong>> configEntry55 = configEntry54;
                configEntry55.Value = GetULongList(configEntry55.Key);
                if (configEntry55.Value.Count <= 0 && string.Equals(GetRawString(configEntry55.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry55.Value = configEntry55.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<ushort>> configEntry56)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.List<ushort>> configEntry57 = configEntry56;
                configEntry57.Value = GetUShortList(configEntry57.Key);
                if (configEntry57.Value.Count <= 0 && string.Equals(GetRawString(configEntry57.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry57.Value = configEntry57.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.Dictionary<string, string>> configEntry58)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::System.Collections.Generic.Dictionary<string, string>> configEntry59 = configEntry58;
                configEntry59.Value = GetStringDictionary(configEntry59.Key);
                if (configEntry59.Value.Count <= 0 && string.Equals(GetRawString(configEntry59.Key), "default", global::System.StringComparison.OrdinalIgnoreCase))
                {
                    configEntry59.Value = configEntry59.Default;
                }
                return;
            }
            if (configEntry is global::Utils.ConfigHandler.ConfigEntry<global::Scp914.Scp914Mode> configEntry60)
            {
                global::Utils.ConfigHandler.ConfigEntry<global::Scp914.Scp914Mode> configEntry61 = configEntry60;
                string text = GetString(configEntry61.Key);
                if (text == "default" || !global::System.Enum.TryParse<global::Scp914.Scp914Mode>(text, out var result))
                {
                    configEntry61.Value = configEntry61.Default;
                }
                else
                {
                    configEntry61.Value = result;
                }
                return;
            }
        }
        throw new global::System.Exception("Config type unsupported (Config: Key = \"" + (configEntry.Key ?? "Null") + "\" Type = \"" + (configEntry.ValueType.FullName ?? "Null") + "\" Name = \"" + (configEntry.Name ?? "Null") + "\" Description = \"" + (configEntry.Description ?? "Null") + "\").");
    }

    private static string[] Filter(global::System.Collections.Generic.IEnumerable<string> lines)
    {
        return global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Where(lines, (string line) => !string.IsNullOrEmpty(line) && !line.StartsWith("#") && (line.StartsWith(" - ") || global::System.Linq.Enumerable.Contains(line, ':'))));
    }

    public void LoadConfigFile(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            Path = path;
            if (!ServerStatic.DisableConfigValidation)
            {
                RemoveInvalid(path);
            }
            if (!ServerStatic.DisableConfigValidation && Path.EndsWith("config_gameplay.txt") && !_afteradding && FileManager.FileExists("ConfigTemplates/config_gameplay.template.txt"))
            {
                AddMissingTemplateKeys("ConfigTemplates/config_gameplay.template.txt", path, ref _afteradding);
            }
            else if (!ServerStatic.DisableConfigValidation && Path.EndsWith("config_remoteadmin.txt") && !_afteradding)
            {
                AddMissingRoleVars(path);
                AddMissingPerms(path, ref _afteradding);
            }
            _rawDataUnfiltered = FileManager.ReadAllLines(path);
            RawData = Filter(_rawDataUnfiltered);
            UpdateRegisteredConfigValues();
        }
    }

    private static void RemoveDeprecated(string path)
    {
        global::System.Collections.Generic.List<string> list = FileManager.ReadAllLinesList(path);
        for (int num = list.Count - 1; num >= 0; num--)
        {
            for (int i = 0; i < _deprecatedconfigs.Length; i++)
            {
                if (list[num].StartsWith(_deprecatedconfigs[i] + ":") && (num == 0 || list[num - 1] != "#REMOVED FROM GAME - REDUNDANT"))
                {
                    list.Insert(num, "#REMOVED FROM GAME - REDUNDANT");
                }
            }
        }
        FileManager.WriteToFile(list, path);
    }

    private static void AddMissingPerms(string path, ref bool _afteradding)
    {
        string[] perms = GetStringList("Permissions", path).ToArray();
        string[] names = global::System.Enum.GetNames(typeof(PlayerPermissions));
        if (perms.Length == names.Length)
        {
            return;
        }
        global::System.Collections.Generic.List<string> collection = global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Where(global::System.Linq.Enumerable.Select(names, (string permtype) => new
        {
            permtype = permtype,
            inconfig = global::System.Linq.Enumerable.Any(perms, (string perm) => perm.StartsWith(permtype))
        }), t => !t.inconfig), t => " - " + t.permtype + ": []"));
        global::System.Collections.Generic.List<string> list = FileManager.ReadAllLinesList(path);
        for (int num = 0; num < list.Count; num++)
        {
            if (list[num] == "Permissions:")
            {
                list.InsertRange(num + 1, collection);
            }
        }
        FileManager.WriteToFile(list, path);
        _afteradding = true;
    }

    private static void AddMissingRoleVars(string path)
    {
        string time = TimeBehaviour.FormatTime("yyyy/MM/dd HH:mm:ss");
        global::System.Collections.Generic.List<string> stringList = GetStringList("Roles", path);
        global::System.Collections.Generic.List<string> list = global::NorthwoodLib.Pools.ListPool<string>.Shared.Rent();
        string config = FileManager.ReadAllText(path);
        foreach (string role in stringList)
        {
            list.AddRange(global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Where(_rolevars, (string rolevar) => !config.Contains(role + "_" + rolevar + ":")), (string rolevar) => role + "_" + rolevar + ": default"));
        }
        if (list.Count > 0)
        {
            Write(list, path, ref time);
        }
        global::NorthwoodLib.Pools.ListPool<string>.Shared.Return(list);
    }

    private static void AddMissingTemplateKeys(string templatepath, string path, ref bool _afteradding)
    {
        string time = TimeBehaviour.FormatTime("yyyy/MM/dd HH:mm:ss");
        string text = FileManager.ReadAllText(path);
        string[] array = FileManager.ReadAllLines(templatepath);
        global::System.Collections.Generic.List<string> list = global::NorthwoodLib.Pools.ListPool<string>.Shared.Rent();
        global::System.Collections.Generic.List<string> list2 = global::NorthwoodLib.Pools.ListPool<string>.Shared.Rent();
        global::System.Collections.Generic.List<string> list3 = global::NorthwoodLib.Pools.ListPool<string>.Shared.Rent();
        for (int i = 0; i < array.Length; i++)
        {
            if (!array[i].StartsWith("#") && !array[i].StartsWith(" -") && array[i].Contains(":") && ((i + 1 < array.Length && array[i + 1].StartsWith(" -")) || array[i].EndsWith("[]")))
            {
                list.Add(array[i]);
            }
            else if (!array[i].StartsWith("#") && array[i].Contains(":") && !array[i].StartsWith(" -"))
            {
                list2.Add(array[i].Substring(0, array[i].IndexOf(':') + 1));
            }
        }
        foreach (string item in list2)
        {
            if (!text.Contains(item))
            {
                list3.Add(item + " default");
            }
        }
        global::NorthwoodLib.Pools.ListPool<string>.Shared.Return(list2);
        Write(list3, path, ref time);
        global::NorthwoodLib.Pools.ListPool<string>.Shared.Return(list3);
        foreach (string item2 in list)
        {
            if (text.Contains(item2))
            {
                continue;
            }
            bool flag = false;
            global::System.Collections.Generic.List<string> list4 = new global::System.Collections.Generic.List<string> { "#LIST", item2 };
            string[] array2 = array;
            foreach (string text2 in array2)
            {
                if (text2.StartsWith(item2) && text2.EndsWith("[]"))
                {
                    list4.Clear();
                    list4.AddRange(new string[2] { "#LIST - [] equals to empty", text2 });
                    break;
                }
                if (text2.StartsWith(item2))
                {
                    flag = true;
                }
                else if (flag)
                {
                    if (text2.StartsWith(" - "))
                    {
                        list4.Add(text2);
                    }
                    else if (!text2.StartsWith("#"))
                    {
                        break;
                    }
                }
            }
            Write(list4, path, ref time);
        }
        global::NorthwoodLib.Pools.ListPool<string>.Shared.Return(list);
        _afteradding = true;
    }

    private static void Write(global::System.Collections.Generic.IEnumerable<string> text, string path, ref string time)
    {
        string[] array = global::System.Linq.Enumerable.ToArray(text);
        if (array.Length != 0)
        {
            Write(string.Join("\r\n", array), path, ref time);
        }
    }

    private static void Write(string text, string path, ref string time)
    {
        using (global::System.IO.StreamWriter streamWriter = global::System.IO.File.AppendText(path))
        {
            streamWriter.Write("\r\n\r\n#ADDED BY CONFIG VALIDATOR - " + time + " Game version: " + global::GameCore.Version.VersionString + "\r\n" + text);
        }
    }

    private static void RemoveInvalid(string path)
    {
        string[] array = FileManager.ReadAllLines(path);
        bool flag = false;
        for (int i = 0; i < array.Length; i++)
        {
            if (!array[i].StartsWith("#") && !array[i].StartsWith(" -") && !array[i].Contains(":") && !string.IsNullOrEmpty(array[i].Replace(" ", "")))
            {
                flag = true;
                array[i] = "#INVALID - " + array[i];
            }
        }
        if (flag)
        {
            FileManager.WriteToFile(array, path);
        }
    }

    private void CommentInvalid(string key, string type)
    {
        if (IsVirtual)
        {
            return;
        }
        for (int i = 0; i < _rawDataUnfiltered.Length; i++)
        {
            if (_rawDataUnfiltered[i].StartsWith(key + ": ", global::System.StringComparison.Ordinal))
            {
                _rawDataUnfiltered[i] = "#INVALID " + type + " - " + _rawDataUnfiltered[i];
            }
        }
        if (!ServerStatic.DisableConfigValidation)
        {
            FileManager.WriteToFile(_rawDataUnfiltered, Path);
        }
    }

    public bool Reload()
    {
        if (IsVirtual)
        {
            return false;
        }
        if (string.IsNullOrEmpty(Path))
        {
            return false;
        }
        LoadConfigFile(Path);
        return true;
    }

    public bool TryGetString(string key, out string value)
    {
        string[] rawData = RawData;
        foreach (string text in rawData)
        {
            if (text.StartsWith(key + ": "))
            {
                value = text.Substring(key.Length + 2);
                return true;
            }
        }
        value = "default";
        return false;
    }

    private string GetRawString(string key)
    {
        if (!TryGetString(key, out var value))
        {
            return "default";
        }
        return value;
    }

    public void SetString(string key, string value = null)
    {
        Reload();
        int num = 0;
        global::System.Collections.Generic.List<string> list = null;
        for (int i = 0; i < _rawDataUnfiltered.Length; i++)
        {
            if (_rawDataUnfiltered[i].StartsWith(key + ": "))
            {
                if (value == null)
                {
                    list = global::System.Linq.Enumerable.ToList(_rawDataUnfiltered);
                    list.RemoveAt(i);
                    num = 2;
                }
                else
                {
                    _rawDataUnfiltered[i] = key + ": " + value;
                    num = 1;
                }
                break;
            }
        }
        if (IsVirtual)
        {
            return;
        }
        switch (num)
        {
            case 0:
                list = global::System.Linq.Enumerable.ToList(_rawDataUnfiltered);
                list.Insert(list.Count, key + ": " + value);
                FileManager.WriteToFile(list, Path);
                break;
            case 1:
                FileManager.WriteToFile(_rawDataUnfiltered, Path);
                break;
            case 2:
                if (list != null)
                {
                    FileManager.WriteToFile(list, Path);
                }
                break;
        }
        Reload();
    }

    private static global::System.Collections.Generic.List<string> GetStringList(string key, string path)
    {
        bool flag = false;
        global::System.Collections.Generic.List<string> list = new global::System.Collections.Generic.List<string>();
        string[] array = FileManager.ReadAllLines(path);
        foreach (string text in array)
        {
            if (text.StartsWith(key) && text.EndsWith("[]"))
            {
                break;
            }
            if (text.StartsWith(key + ":"))
            {
                string text2 = text.Substring(key.Length + 1);
                if (text2.Contains("[") && text2.Contains("]"))
                {
                    return global::System.Linq.Enumerable.ToList(ParseCommaSeparatedString(text2));
                }
                flag = true;
            }
            else if (flag)
            {
                if (text.StartsWith(" - "))
                {
                    list.Add(text.Substring(3));
                }
                else if (!text.StartsWith("#"))
                {
                    break;
                }
            }
        }
        return list;
    }

    public void SetStringListItem(string key, string value, string newValue)
    {
        Reload();
        bool flag = false;
        int num = 0;
        global::System.Collections.Generic.List<string> list = null;
        for (int i = 0; i < _rawDataUnfiltered.Length; i++)
        {
            string text = _rawDataUnfiltered[i];
            if (text.StartsWith(key + ":"))
            {
                flag = true;
            }
            else
            {
                if (!flag)
                {
                    continue;
                }
                if (value != null && text == " - " + value)
                {
                    if (newValue == null)
                    {
                        list = global::System.Linq.Enumerable.ToList(_rawDataUnfiltered);
                        list.RemoveAt(i);
                        num = 2;
                    }
                    else
                    {
                        _rawDataUnfiltered[i] = " - " + newValue;
                        num = 1;
                    }
                    break;
                }
                if (!text.StartsWith(" - ") && !text.StartsWith("#"))
                {
                    if (value != null)
                    {
                        list = global::System.Linq.Enumerable.ToList(_rawDataUnfiltered);
                        list.Insert(i, " - " + newValue);
                        num = 2;
                    }
                    break;
                }
            }
        }
        if (IsVirtual)
        {
            return;
        }
        switch (num)
        {
            case 1:
                FileManager.WriteToFile(_rawDataUnfiltered, Path);
                break;
            case 2:
                if (list != null)
                {
                    FileManager.WriteToFile(list, Path);
                }
                break;
        }
        Reload();
    }

    public global::System.Collections.Generic.IEnumerable<string> StringListToText(string key, global::System.Collections.Generic.List<string> list)
    {
        yield return key + ":";
        foreach (string item in list)
        {
            yield return " - " + item;
        }
    }

    public global::System.Collections.Generic.Dictionary<string, string> GetStringDictionary(string key)
    {
        GetStringCollection(key, DataBuffer);
        global::System.Collections.Generic.Dictionary<string, string> dictionary = new global::System.Collections.Generic.Dictionary<string, string>();
        foreach (string item in DataBuffer)
        {
            if (!item.Contains(": "))
            {
                ServerConsole.AddLog("Invalid entry \"" + item + "\" in dictionary " + key + " in the config file - missing \": \".", global::System.ConsoleColor.Red);
                continue;
            }
            int num = item.IndexOf(": ", global::System.StringComparison.Ordinal);
            string text = item.Substring(0, num);
            if (!dictionary.ContainsKey(text))
            {
                dictionary.Add(text, item.Substring(num + 2));
                continue;
            }
            ServerConsole.AddLog("Ignoring duplicated subkey " + text + " in dictionary " + key + " in the config file.", global::System.ConsoleColor.Yellow);
        }
        return dictionary;
    }

    public void SetStringDictionaryItem(string key, string subkey, string value)
    {
        Reload();
        bool flag = false;
        int num = 0;
        global::System.Collections.Generic.List<string> list = null;
        for (int i = 0; i < _rawDataUnfiltered.Length; i++)
        {
            string text = _rawDataUnfiltered[i];
            if (text.StartsWith(key + ":"))
            {
                flag = true;
            }
            else
            {
                if (!flag)
                {
                    continue;
                }
                if (text.StartsWith(" - " + subkey + ": "))
                {
                    if (value == null)
                    {
                        list = global::System.Linq.Enumerable.ToList(_rawDataUnfiltered);
                        list.RemoveAt(i);
                        num = 2;
                    }
                    else
                    {
                        _rawDataUnfiltered[i] = " - " + subkey + ": " + value;
                        num = 1;
                    }
                    break;
                }
                if (!text.StartsWith(" - ") && !text.StartsWith("#"))
                {
                    if (value != null)
                    {
                        list = global::System.Linq.Enumerable.ToList(_rawDataUnfiltered);
                        list.Insert(i, " - " + subkey + ": " + value);
                        num = 2;
                    }
                    break;
                }
            }
        }
        if (IsVirtual)
        {
            return;
        }
        switch (num)
        {
            case 0:
                list = global::System.Linq.Enumerable.ToList(_rawDataUnfiltered);
                list.Insert(list.Count, " - " + subkey + ": " + value);
                FileManager.WriteToFile(list, Path);
                break;
            case 1:
                FileManager.WriteToFile(_rawDataUnfiltered, Path);
                break;
            case 2:
                if (list != null)
                {
                    FileManager.WriteToFile(list, Path);
                }
                break;
        }
        Reload();
    }

    public static string[] ParseCommaSeparatedString(string data)
    {
        data = data.Trim();
        if (!data.StartsWith("[", global::System.StringComparison.Ordinal) || !data.EndsWith("]", global::System.StringComparison.Ordinal))
        {
            return null;
        }
        data = data.Substring(1, data.Length - 2).Replace(", ", ",");
        return data.Split(',');
    }

    public global::System.Collections.Generic.IEnumerable<string> GetKeys()
    {
        return global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Where(RawData, (string line) => line.Contains(":")), (string line) => line.Split(':')[0]);
    }

    public bool IsList(string key)
    {
        bool flag = false;
        string[] rawData = RawData;
        foreach (string text in rawData)
        {
            if (text.StartsWith(key + ":"))
            {
                flag = true;
            }
            else if (flag)
            {
                if (text.StartsWith(" - "))
                {
                    return true;
                }
                if (!text.StartsWith("#"))
                {
                    break;
                }
            }
        }
        return false;
    }

    public void Merge(ref YamlConfig toMerge)
    {
        string[] array = global::System.Linq.Enumerable.ToArray(GetKeys());
        IsVirtual = true;
        foreach (string key in toMerge.GetKeys())
        {
            if (array.Contains<string>(key))
            {
                continue;
            }
            if (toMerge.IsList(key))
            {
                foreach (string item in toMerge.StringListToText(key, toMerge.GetStringList(key)))
                {
                    global::System.Linq.Enumerable.Append(RawData, item);
                }
            }
            else
            {
                SetString(key, toMerge.GetRawString(key));
            }
        }
    }

    public bool GetBool(string key, bool def = false)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (bool.TryParse(text, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid bool!");
        CommentInvalid(key, "BOOL");
        return def;
    }

    public byte GetByte(string key, byte def = 0)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (byte.TryParse(text, global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid byte!");
        CommentInvalid(key, "BYTE");
        return def;
    }

    public sbyte GetSByte(string key, sbyte def = 0)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (sbyte.TryParse(text, global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid signed byte!");
        CommentInvalid(key, "SBYTE");
        return def;
    }

    public char GetChar(string key, char def = ' ')
    {
        string rawString = GetRawString(key);
        if (rawString == "default")
        {
            return def;
        }
        if (char.TryParse(rawString, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has an invalid value, " + rawString + " is not a valid char!");
        CommentInvalid(key, "CHAR");
        return def;
    }

    public decimal GetDecimal(string key, decimal def = 0m)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (decimal.TryParse(text.Replace(',', '.'), global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid decimal!");
        CommentInvalid(key, "DECIMAL");
        return def;
    }

    public double GetDouble(string key, double def = 0.0)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (double.TryParse(text.Replace(',', '.'), global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid double!");
        CommentInvalid(key, "DOUBLE");
        return def;
    }

    public float GetFloat(string key, float def = 0f)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (float.TryParse(text.Replace(',', '.'), global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid float!");
        CommentInvalid(key, "FLOAT");
        return def;
    }

    public int GetInt(string key, int def = 0)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (int.TryParse(text, global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid integer!");
        CommentInvalid(key, "INT");
        return def;
    }

    public uint GetUInt(string key, uint def = 0u)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (uint.TryParse(text, global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid unsigned integer!");
        CommentInvalid(key, "UINT");
        return def;
    }

    public long GetLong(string key, long def = 0L)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (long.TryParse(text, global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid long!");
        CommentInvalid(key, "LONG");
        return def;
    }

    public ulong GetULong(string key, ulong def = 0uL)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (ulong.TryParse(text, global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid unsigned long!");
        CommentInvalid(key, "ULONG");
        return def;
    }

    public short GetShort(string key, short def = 0)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (short.TryParse(text, global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid short!");
        CommentInvalid(key, "SHORT");
        return def;
    }

    public ushort GetUShort(string key, ushort def = 0)
    {
        string text = GetRawString(key).ToLower();
        if (text == "default")
        {
            return def;
        }
        if (ushort.TryParse(text, global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid unsigned short!");
        CommentInvalid(key, "USHORT");
        return def;
    }

    public string GetString(string key, string def = "")
    {
        string rawString = GetRawString(key);
        if (!(rawString == "default"))
        {
            return rawString;
        }
        return def;
    }

    public global::System.Collections.Generic.List<bool> GetBoolList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, bool.Parse));
    }

    public global::System.Collections.Generic.List<byte> GetByteList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, byte.Parse));
    }

    public global::System.Collections.Generic.List<sbyte> GetSByteList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, sbyte.Parse));
    }

    public global::System.Collections.Generic.List<char> GetCharList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, char.Parse));
    }

    public global::System.Collections.Generic.List<decimal> GetDecimalList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, decimal.Parse));
    }

    public global::System.Collections.Generic.List<double> GetDoubleList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, double.Parse));
    }

    public global::System.Collections.Generic.List<float> GetFloatList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, float.Parse));
    }

    public global::System.Collections.Generic.List<int> GetIntList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, int.Parse));
    }

    public global::System.Collections.Generic.List<uint> GetUIntList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, uint.Parse));
    }

    public global::System.Collections.Generic.List<long> GetLongList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, long.Parse));
    }

    public global::System.Collections.Generic.List<ulong> GetULongList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, ulong.Parse));
    }

    public global::System.Collections.Generic.List<short> GetShortList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, short.Parse));
    }

    public global::System.Collections.Generic.List<ushort> GetUShortList(string key)
    {
        GetStringCollection(key, DataBuffer);
        return global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(DataBuffer, ushort.Parse));
    }

    public global::System.Collections.Generic.List<string> GetStringList(string key)
    {
        global::System.Collections.Generic.List<string> list = new global::System.Collections.Generic.List<string>();
        GetStringCollection(key, list);
        return list;
    }

    public void GetStringCollection(string key, global::System.Collections.Generic.ICollection<string> collection)
    {
        bool flag = false;
        collection.Clear();
        string[] rawData = RawData;
        foreach (string text in rawData)
        {
            if (text.StartsWith(key) && text.TrimEnd().EndsWith("[]", global::System.StringComparison.Ordinal))
            {
                break;
            }
            if (text.StartsWith(key + ":", global::System.StringComparison.Ordinal))
            {
                if (text.StartsWith(key + ": ", global::System.StringComparison.Ordinal))
                {
                    string text2 = text.Substring(key.Length + 2);
                    if (text2.Contains("[") && text2.Contains("]"))
                    {
                        if (collection is global::System.Collections.Generic.List<string> list)
                        {
                            list.AddRange(ParseCommaSeparatedString(text2));
                            break;
                        }
                        string[] array = ParseCommaSeparatedString(text2);
                        foreach (string item in array)
                        {
                            collection.Add(item);
                        }
                        break;
                    }
                }
                flag = true;
            }
            else if (flag)
            {
                if (text.StartsWith(" - ", global::System.StringComparison.Ordinal))
                {
                    collection.Add(text.Substring(3).TrimEnd());
                }
                else if (!text.StartsWith("#", global::System.StringComparison.Ordinal))
                {
                    break;
                }
            }
        }
    }
}
