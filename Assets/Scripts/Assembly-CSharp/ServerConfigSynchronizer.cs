using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using GameCore;
using InventorySystem.Configs;
using Mirror;
using UnityEngine;

public class ServerConfigSynchronizer : NetworkBehaviour
{
    public enum MainBoolsSettings : byte
    {
        FriendlyFire = 1
    }

    [Serializable]
    public struct AmmoLimit
    {
        public ItemType AmmoType;

        public ushort Limit;
    }

    public struct PredefinedBanTemplate
    {
        public int Duration;

        public string FormattedDuration;

        public string Reason;
    }

    public static ServerConfigSynchronizer Singleton;

    public static Action OnRefreshed;

    [SyncVar]
    public byte MainBoolsSync;

    public readonly SyncList<sbyte> CategoryLimits = new();

    public readonly SyncList<AmmoLimit> AmmoLimitsSync = new();

    [SyncVar]
    public string ServerName;

    [SyncVar]
    public bool EnableRemoteAdminPredefinedBanTemplates = true;

    [SyncVar]
    public string RemoteAdminExternalPlayerLookupMode = "disabled";

    [SyncVar]
    public string RemoteAdminExternalPlayerLookupURL = "";

    [NonSerialized]
    public string RemoteAdminExternalPlayerLookupToken = string.Empty;

    public readonly SyncList<PredefinedBanTemplate> RemoteAdminPredefinedBanTemplates = new SyncList<PredefinedBanTemplate>();

    private bool _ready;

    private readonly Regex _regex = new Regex("[^\\w\\d]+", RegexOptions.Compiled);

    private void Awake()
    {
        Singleton = this;
    }

    public static void RefreshAllConfigs()
    {
        if (!(Singleton == null))
        {
            Singleton.RefreshMainBools();
            Singleton.RefreshCategoryLimits();
            Singleton.RefreshAmmoLimits();
            Singleton.RefreshRAConfigs();
            OnRefreshed?.Invoke();
        }
    }

    [Server]
    private void RefreshRAConfigs()
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'System.Void ServerConfigSynchronizer::RefreshRAConfigs()' called when server was not active");
            return;
        }
        if (ServerStatic.RolesConfig == null)
        {
            CustomNetworkManager.LoadServerPermissions();
        }
        EnableRemoteAdminPredefinedBanTemplates = ServerStatic.RolesConfig.GetBool("enable_predefined_ban_templates", def: true);
        RemoteAdminExternalPlayerLookupMode = ServerStatic.RolesConfig.GetString("external_player_lookup_mode", "disabled").Trim().ToLower();
        RemoteAdminExternalPlayerLookupURL = ServerStatic.RolesConfig.GetString("external_player_lookup_url");
        RemoteAdminExternalPlayerLookupToken = ServerStatic.RolesConfig.GetString("external_player_lookup_token");
        RemoteAdminPredefinedBanTemplates.Clear();
        if (!EnableRemoteAdminPredefinedBanTemplates)
        {
            return;
        }
        List<string> stringList = ServerStatic.RolesConfig.GetStringList("PredefinedBanTemplates");
        if (stringList != null)
        {
            PredefinedBanTemplate item = default(PredefinedBanTemplate);
            foreach (string item2 in stringList)
            {
                string[] array = YamlConfig.ParseCommaSeparatedString(item2);
                if (array.Length != 2)
                {
                    ServerConsole.AddLog("Invalid ban template in RA Config file! Template: " + item2);
                    continue;
                }
                if (!int.TryParse(array[0], out var result) || result < 0)
                {
                    ServerConsole.AddLog("Invalid ban template in RA Config file - duration must be a non-negative integer. Ban template name: " + item2);
                    continue;
                }
                item.Reason = array[1];
                TimeSpan timeSpan = TimeSpan.FromSeconds(result);
                item.Duration = (int)timeSpan.TotalMinutes;
                int num = timeSpan.Days / 365;
                if (num > 0)
                {
                    item.FormattedDuration = $"{num}y";
                }
                else if (timeSpan.Days > 0)
                {
                    item.FormattedDuration = $"{timeSpan.Days}d";
                }
                else if (timeSpan.Hours > 0)
                {
                    item.FormattedDuration = $"{timeSpan.Hours}h";
                }
                else if (timeSpan.Minutes > 0)
                {
                    item.FormattedDuration = $"{timeSpan.Minutes}m";
                }
                else
                {
                    item.FormattedDuration = $"{timeSpan.Seconds}s";
                }
                RemoteAdminPredefinedBanTemplates.Add(item);
            }
            if (RemoteAdminPredefinedBanTemplates.Count == 0)
            {
                EnableRemoteAdminPredefinedBanTemplates = false;
            }
        }
        else
        {
            EnableRemoteAdminPredefinedBanTemplates = false;
        }
    }


    [Server]
    public void RefreshMainBools()
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'System.Void ServerConfigSynchronizer::RefreshMainBools()' called when server was not active");
        }
        else
        {
            MainBoolsSync = Misc.BoolsToByte(ServerConsole.FriendlyFire);
        }
    }


    [Server]
    private void RefreshCategoryLimits()
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'System.Void ServerConfigSynchronizer::RefreshCategoryLimits()' called when server was not active");
            return;
        }
        CategoryLimits.Clear();
        for (int i = 0; Enum.IsDefined(typeof(ItemCategory), (ItemCategory)i); i++)
        {
            ItemCategory key = (ItemCategory)i;
            if (InventoryLimits.StandardCategoryLimits.TryGetValue(key, out var value) && value >= 0)
            {
                CategoryLimits.Add(ConfigFile.ServerConfig.GetSByte("limit_category_" + key.ToString().ToLowerInvariant(), value));
            }
        }
    }

    [Server]
    private void RefreshAmmoLimits()
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'System.Void ServerConfigSynchronizer::RefreshAmmoLimits()' called when server was not active");
            return;
        }
        if (AmmoLimitsSync.Count > 0)
        {
            AmmoLimitsSync.Clear();
        }
        foreach (KeyValuePair<ItemType, ushort> standardAmmoLimit in InventoryLimits.StandardAmmoLimits)
        {
            ushort uShort = ConfigFile.ServerConfig.GetUShort("limit_" + standardAmmoLimit.Key.ToString().ToLowerInvariant(), standardAmmoLimit.Value);
            AmmoLimitsSync.Add(new AmmoLimit
            {
                AmmoType = standardAmmoLimit.Key,
                Limit = uShort
            });
        }
    }

    private void Update()
    {
        if (!_ready)
        {
            if (!NetworkServer.active)
            {
                _ready = true;
            }
            else if (ReferenceHub.TryGetHostHub(out ReferenceHub hub))
            {
                _ready = true;
                RefreshAllConfigs();
            }
        }
    }
}
