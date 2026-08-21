public class PermissionsHandler
{
    private readonly string _overridePassword;

    private readonly string _overrideRole;

    private readonly global::System.Collections.Generic.Dictionary<string, UserGroup> _groups;

    private readonly global::System.Collections.Generic.Dictionary<string, string> _members;

    private readonly global::System.Collections.Generic.Dictionary<string, ulong> _permissions;

    private readonly global::System.Collections.Generic.HashSet<ulong> _raPermissions;

    private readonly YamlConfig _config;

    private readonly YamlConfig _sharedGroups;

    private readonly YamlConfig _sharedGroupsMembers;

    private ulong _lastPerm;

    private readonly bool _managerAccess;

    private readonly bool _banTeamAccess;

    private readonly bool _banTeamSlots;

    private readonly bool _banTeamGeoBypass;

    public static readonly global::System.Collections.Generic.Dictionary<PlayerPermissions, string> PermissionCodes = new global::System.Collections.Generic.Dictionary<PlayerPermissions, string>
    {
        {
            PlayerPermissions.KickingAndShortTermBanning,
            "BN1"
        },
        {
            PlayerPermissions.BanningUpToDay,
            "BN2"
        },
        {
            PlayerPermissions.LongTermBanning,
            "BN3"
        },
        {
            PlayerPermissions.ForceclassSelf,
            "FSE"
        },
        {
            PlayerPermissions.ForceclassToSpectator,
            "FSP"
        },
        {
            PlayerPermissions.ForceclassWithoutRestrictions,
            "FWR"
        },
        {
            PlayerPermissions.GivingItems,
            "GIV"
        },
        {
            PlayerPermissions.WarheadEvents,
            "EWA"
        },
        {
            PlayerPermissions.RespawnEvents,
            "ERE"
        },
        {
            PlayerPermissions.RoundEvents,
            "ERO"
        },
        {
            PlayerPermissions.SetGroup,
            "SGR"
        },
        {
            PlayerPermissions.GameplayData,
            "GMD"
        },
        {
            PlayerPermissions.Overwatch,
            "OVR"
        },
        {
            PlayerPermissions.FacilityManagement,
            "FCM"
        },
        {
            PlayerPermissions.PlayersManagement,
            "PLM"
        },
        {
            PlayerPermissions.PermissionsManagement,
            "PRM"
        },
        {
            PlayerPermissions.ServerConsoleCommands,
            "SCC"
        },
        {
            PlayerPermissions.ViewHiddenBadges,
            "VHB"
        },
        {
            PlayerPermissions.ServerConfigs,
            "CFG"
        },
        {
            PlayerPermissions.Broadcasting,
            "BRC"
        },
        {
            PlayerPermissions.PlayerSensitiveDataAccess,
            "CDA"
        },
        {
            PlayerPermissions.Noclip,
            "NCP"
        },
        {
            PlayerPermissions.AFKImmunity,
            "AFK"
        },
        {
            PlayerPermissions.AdminChat,
            "ATC"
        },
        {
            PlayerPermissions.ViewHiddenGlobalBadges,
            "GHB"
        },
        {
            PlayerPermissions.Announcer,
            "ANN"
        },
        {
            PlayerPermissions.Effects,
            "EFF"
        },
        {
            PlayerPermissions.FriendlyFireDetectorImmunity,
            "FFI"
        },
        {
            PlayerPermissions.FriendlyFireDetectorTempDisable,
            "FFT"
        }
    };

    public bool BanTeamSlots
    {
        get
        {
            if (!_banTeamSlots)
            {
                return IsVerified;
            }
            return true;
        }
    }

    public bool BanTeamBypassGeo
    {
        get
        {
            if (!_banTeamGeoBypass)
            {
                return IsVerified;
            }
            return true;
        }
    }

    public UserGroup OverrideGroup
    {
        get
        {
            if (!OverrideEnabled)
            {
                return null;
            }
            if (_groups.ContainsKey(_overrideRole))
            {
                return _groups[_overrideRole];
            }
            return null;
        }
    }

    public bool OverrideEnabled
    {
        get
        {
            if (string.IsNullOrEmpty(_overridePassword) || _overridePassword == "none")
            {
                return false;
            }
            if (!IsVerified)
            {
                return true;
            }
            if (_overridePassword.Length < 8)
            {
                ServerConsole.AddLog("Override password refused, because it's too short (requirement for verified servers only).");
                return false;
            }
            if (_overridePassword.ToLower() == _overridePassword || _overridePassword.ToUpper() == _overridePassword)
            {
                ServerConsole.AddLog("Override password refused, because it must contain mixed case chars (requirement for verified servers only).");
                return false;
            }
            if (global::System.Linq.Enumerable.Any(_overridePassword, (char c) => !char.IsLetter(c)))
            {
                return true;
            }
            ServerConsole.AddLog("Override password refused, because it must contain digit or special symbol (requirement for verified servers only).");
            return false;
        }
    }

    public bool IsVerified { get; private set; }

    public ulong FullPerm { get; private set; }

    public bool StaffAccess { get; }

    public bool ManagersAccess
    {
        get
        {
            if (!_managerAccess && !StaffAccess)
            {
                return IsVerified;
            }
            return true;
        }
    }

    public bool BanningTeamAccess
    {
        get
        {
            if (!_banTeamAccess && !StaffAccess)
            {
                return IsVerified;
            }
            return true;
        }
    }

    public bool NorthwoodAccess { get; }

    public PermissionsHandler(ref YamlConfig configuration, ref YamlConfig sharedGroups, ref YamlConfig sharedGroupsMembers)
    {
        _config = new YamlConfig(configuration.Path);
        _sharedGroups = sharedGroups;
        _sharedGroupsMembers = sharedGroupsMembers;
        _overridePassword = configuration.GetString("override_password", "none");
        _overrideRole = configuration.GetString("override_password_role", "owner");
        StaffAccess = configuration.GetBool("enable_staff_access");
        _managerAccess = configuration.GetBool("enable_manager_access", def: true);
        _banTeamAccess = configuration.GetBool("enable_banteam_access", def: true);
        _banTeamSlots = configuration.GetBool("enable_banteam_reserved_slots", def: true);
        _banTeamGeoBypass = configuration.GetBool("enable_banteam_bypass_geoblocking", def: true);
        NorthwoodAccess = configuration.GetBool("enable_northwood_access");
        if (NorthwoodAccess)
        {
            ServerConsole.AddLog("WARNING - Northwood staff access is enabled! All NW Studios staff members will have FULL Remote Admin access (this should only be used on testing servers!) you can disable this by setting 'enable_northwood_access' to false in your remote admin config file.", global::System.ConsoleColor.Yellow);
        }
        _groups = new global::System.Collections.Generic.Dictionary<string, UserGroup>();
        _raPermissions = new global::System.Collections.Generic.HashSet<ulong>();
        global::System.Collections.Generic.List<string> stringList = configuration.GetStringList("Roles");
        global::System.Collections.Generic.List<string> stringList2 = configuration.GetStringList("Roles");
        if (sharedGroups != null)
        {
            global::System.Collections.Generic.List<string> stringList3 = sharedGroups.GetStringList("SharedRoles");
            string text = global::GameCore.ConfigFile.SharingConfig.GetString("groups_sharing_mode");
            switch (text)
            {
                case "all":
                    stringList.AddRange(stringList3);
                    break;
                case "opt-in":
                    {
                        global::System.Collections.Generic.List<string> optIn = global::GameCore.ConfigFile.SharingConfig.GetStringList("groups_opt_in_list");
                        stringList.AddRange(global::System.Linq.Enumerable.Where(stringList3, (string group) => optIn.Contains(group)));
                        break;
                    }
                case "opt-out":
                    {
                        global::System.Collections.Generic.List<string> optOut = global::GameCore.ConfigFile.SharingConfig.GetStringList("groups_opt_out_list");
                        stringList.AddRange(global::System.Linq.Enumerable.Where(stringList3, (string group) => !optOut.Contains(group)));
                        break;
                    }
                default:
                    ServerConsole.AddLog("Invalid group sharing mode set!");
                    break;
            }
        }
        string[] array = global::System.Linq.Enumerable.ToArray(configuration.GetKeys());
        foreach (string item in stringList)
        {
            string text2 = ((array.Contains<string>(item + "_badge") || sharedGroups == null) ? configuration.GetString(item + "_badge") : sharedGroups.GetString(item + "_badge"));
            string text3 = ((array.Contains<string>(item + "_color") || sharedGroups == null) ? configuration.GetString(item + "_color") : sharedGroups.GetString(item + "_color"));
            bool cover = ((array.Contains<string>(item + "_cover") || sharedGroups == null) ? configuration.GetBool(item + "_cover", def: true) : sharedGroups.GetBool(item + "_cover", def: true));
            bool hiddenByDefault = ((array.Contains<string>(item + "_hidden") || sharedGroups == null) ? configuration.GetBool(item + "_hidden") : sharedGroups.GetBool(item + "_hidden"));
            byte kickPower = ((array.Contains<string>(item + "_kick_power") || sharedGroups == null) ? configuration.GetByte(item + "_kick_power", 0) : sharedGroups.GetByte(item + "_kick_power", 0));
            byte requiredKickPower = ((array.Contains<string>(item + "_required_kick_power") || sharedGroups == null) ? configuration.GetByte(item + "_required_kick_power", 0) : sharedGroups.GetByte(item + "_required_kick_power", 0));
            if (!(text2 == "") && !(text3 == ""))
            {
                if (_groups.ContainsKey(item))
                {
                    ServerConsole.AddLog("Duplicated group definition: " + item + ".");
                    continue;
                }
                _groups.Add(item, new UserGroup
                {
                    BadgeColor = text3,
                    BadgeText = text2,
                    Permissions = 0uL,
                    Cover = cover,
                    HiddenByDefault = hiddenByDefault,
                    Shared = !stringList2.Contains(item),
                    KickPower = kickPower,
                    RequiredKickPower = requiredKickPower
                });
            }
        }
        _members = configuration.GetStringDictionary("Members");
        global::System.Collections.Generic.Dictionary<string, string> dictionary = _sharedGroupsMembers?.GetStringDictionary("SharedMembers");
        if (dictionary != null)
        {
            foreach (global::System.Collections.Generic.KeyValuePair<string, string> item2 in dictionary)
            {
                if (_members.ContainsKey(item2.Key))
                {
                    ServerConsole.AddLog("Duplicated group member: " + item2.Key + ". Is member of " + _members[item2.Key] + " and " + item2.Value + ".");
                }
                else
                {
                    _members.Add(item2.Key, item2.Value);
                }
            }
        }
        _lastPerm = 1uL;
        global::System.Collections.Generic.HashSet<string> hashSet = new global::System.Collections.Generic.HashSet<string>();
        if (_members != null)
        {
            foreach (global::System.Collections.Generic.KeyValuePair<string, string> member in _members)
            {
                if (!_groups.ContainsKey(member.Value))
                {
                    hashSet.Add(member.Key);
                }
            }
        }
        if (hashSet.Count > 0 && _members != null)
        {
            foreach (string item3 in hashSet)
            {
                _members.Remove(item3);
            }
        }
        hashSet.Clear();
        hashSet = null;
        _permissions = new global::System.Collections.Generic.Dictionary<string, ulong>();
        string[] names = global::System.Enum.GetNames(typeof(PlayerPermissions));
        foreach (string text4 in names)
        {
            ulong num2 = (ulong)global::System.Enum.Parse(typeof(PlayerPermissions), text4);
            FullPerm |= num2;
            _permissions.Add(text4, num2);
            if (num2 != 4096 && num2 != 131072 && num2 != 2097152 && num2 != 4194304 && num2 != 16777216 && num2 != 134217728)
            {
                _raPermissions.Add(num2);
            }
            if (num2 > _lastPerm)
            {
                _lastPerm = num2;
            }
        }
        RefreshPermissions();
    }

    public ulong RegisterPermission(string name, bool remoteAdmin, bool refresh = true)
    {
        _lastPerm = (ulong)global::System.Math.Pow(2.0, global::System.Math.Log(_lastPerm, 2.0) + 1.0);
        FullPerm |= _lastPerm;
        _permissions.Add(name, _lastPerm);
        if (remoteAdmin)
        {
            _raPermissions.Add(_lastPerm);
        }
        if (refresh)
        {
            RefreshPermissions();
        }
        return _lastPerm;
    }

    public void RefreshPermissions()
    {
        foreach (global::System.Collections.Generic.KeyValuePair<string, UserGroup> group in _groups)
        {
            group.Value.Permissions = 0uL;
        }
        global::System.Collections.Generic.Dictionary<string, string> stringDictionary = _config.GetStringDictionary("Permissions");
        global::System.Collections.Generic.Dictionary<string, string> dictionary = _sharedGroups?.GetStringDictionary("SharedPermissions");
        foreach (string key3 in _permissions.Keys)
        {
            ulong num = _permissions[key3];
            if (stringDictionary.ContainsKey(key3))
            {
                string[] array = YamlConfig.ParseCommaSeparatedString(stringDictionary[key3]);
                if (array == null)
                {
                    ServerConsole.AddLog("Failed to process group permissions in remote admin config! Make sure there is no typo.");
                }
                else
                {
                    string[] array2 = array;
                    foreach (string key in array2)
                    {
                        if (_groups.ContainsKey(key))
                        {
                            _groups[key].Permissions |= num;
                        }
                    }
                }
            }
            else
            {
                ServerConsole.AddLog("RemoteAdmin config is missing permission definition: " + key3);
            }
            if (dictionary == null)
            {
                continue;
            }
            if (dictionary.ContainsKey(key3))
            {
                string[] array3 = YamlConfig.ParseCommaSeparatedString(dictionary[key3]);
                if (array3 == null)
                {
                    continue;
                }
                string[] array2 = array3;
                foreach (string key2 in array2)
                {
                    if (_groups.ContainsKey(key2))
                    {
                        _groups[key2].Permissions |= num;
                    }
                }
            }
            else
            {
                ServerConsole.AddLog("Shared groups config is missing permission definition: " + key3);
            }
        }
    }

    public bool IsRaPermitted(ulong permissions)
    {
        return global::System.Linq.Enumerable.Any(_raPermissions, (ulong perm) => IsPermitted(permissions, perm));
    }

    public UserGroup GetGroup(string name)
    {
        if (_groups.ContainsKey(name))
        {
            return _groups[name].Clone();
        }
        return null;
    }

    public global::System.Collections.Generic.List<string> GetAllGroupsNames()
    {
        return global::System.Linq.Enumerable.ToList(_groups.Keys);
    }

    public global::System.Collections.Generic.Dictionary<string, UserGroup> GetAllGroups()
    {
        return global::System.Linq.Enumerable.ToDictionary(_groups.Keys, (string gr) => gr, (string gr) => _groups[gr]);
    }

    public string GetPermissionName(ulong value)
    {
        return global::System.Linq.Enumerable.FirstOrDefault(_permissions, (global::System.Collections.Generic.KeyValuePair<string, ulong> x) => x.Value == value).Key;
    }

    public ulong GetPermissionValue(string name)
    {
        return global::System.Linq.Enumerable.FirstOrDefault(_permissions, (global::System.Collections.Generic.KeyValuePair<string, ulong> x) => x.Key == name).Value;
    }

    public global::System.Collections.Generic.List<string> GetAllPermissions()
    {
        return global::System.Linq.Enumerable.ToList(_permissions.Keys);
    }

    public void SetServerAsVerified()
    {
        IsVerified = true;
    }

    public static bool IsPermitted(ulong permissions, PlayerPermissions check)
    {
        return IsPermitted(permissions, (ulong)check);
    }

    public static bool IsPermitted(ulong permissions, PlayerPermissions[] check)
    {
        if (check.Length == 0)
        {
            return true;
        }
        ulong num = 0uL;
        num = global::System.Linq.Enumerable.Aggregate(check, 0uL, (ulong current, PlayerPermissions c) => current | (ulong)c);
        return IsPermitted(permissions, num);
    }

    public bool IsPermitted(ulong permissions, string check)
    {
        if (_permissions.ContainsKey(check))
        {
            return IsPermitted(permissions, _permissions[check]);
        }
        return false;
    }

    public bool IsPermitted(ulong permissions, string[] check)
    {
        if (check.Length == 0)
        {
            return true;
        }
        ulong check2 = global::System.Linq.Enumerable.Aggregate(global::System.Linq.Enumerable.Where(check, (string c) => _permissions.ContainsKey(c)), 0uL, (ulong current, string c) => current | _permissions[c]);
        return IsPermitted(permissions, check2);
    }

    public static bool IsPermitted(ulong permissions, ulong check)
    {
        return (permissions & check) != 0;
    }

    public byte[] DerivePassword(byte[] serverSalt, byte[] clientSalt)
    {
        return global::RemoteAdmin.QueryProcessor.DerivePassword(_overridePassword, serverSalt, clientSalt);
    }

    public UserGroup GetUserGroup(string userId)
    {
        return _groups[_members[userId]];
    }
}
