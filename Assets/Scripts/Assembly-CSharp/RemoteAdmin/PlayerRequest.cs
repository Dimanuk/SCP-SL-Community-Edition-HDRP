using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RemoteAdmin
{
    public class PlayerRequest : MonoBehaviour
    {
        public Transform parent;
        public GameObject template;
        public static PlayerRequest Singleton;
        public TMP_InputField playerSearch;
        private readonly List<GameObject> _spawnedObjects = new List<GameObject>();

        private void Awake()
        {
            Singleton = this;
        }

        private void Update()
        {
            if (ServerStatic.IsDedicated)
                return;

            var ui = UIController.Singleton;
            if (ui == null)
                return;

            if (!ui.IsEnabled || !ui.CanToggle)
                return;

            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (ctrl && Input.GetKeyDown(KeyCode.A))
            {
                foreach (var record in PlayerRecord.Instances)
                {
                    if (record != null)
                        record.IsSelected = true;
                }
            }
        }

        public void ResponsePlayerList(string data, bool isSuccess, bool showClasses)
        {
            if (!isSuccess)
                return;

            var selectedIds = NorthwoodLib.Pools.ListPool<string>.Shared.Rent();
            foreach (var record in PlayerRecord.Instances)
            {
                if (record != null && record.IsSelected)
                    selectedIds.Add(record.PlayerId);
            }

            PlayerRecord.Instances.Clear();
            foreach (var go in _spawnedObjects)
            {
                if (go != null)
                    Destroy(go);
            }
            _spawnedObjects.Clear();

            if (!string.IsNullOrEmpty(data))
            {
                var lines = data.Split('\n');
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    string search = playerSearch?.text;
                    if (!string.IsNullOrEmpty(search) &&
                        !NorthwoodLib.StringUtils.Contains(line, search, StringComparison.OrdinalIgnoreCase))
                        continue;

                    int open = line.IndexOf('(');
                    int close = line.IndexOf(')');
                    if (open < 0 || close <= open)
                        continue;

                    GameObject go = Instantiate(template, parent);
                    go.SetActive(true);
                    PlayerRecord record = go.GetComponentInChildren<PlayerRecord>();
                    go.transform.localScale = Vector3.one;
                    _spawnedObjects.Add(go);

                    if (record == null)
                        continue;

                    string playerId = line.Substring(open + 1, close - open - 1);

                    record.PlayerId = playerId;

                    if (selectedIds.Contains(playerId))
                        record.SetState(true, true, false);

                    string colorHex = Misc.ToHex(Color.white);
                    if (showClasses &&
                        int.TryParse(playerId, out int hubId) &&
                        ReferenceHub.TryGetHub(hubId, out var hub))
                    {
                        var roleId = PlayerRoles.PlayerRolesUtils.GetRoleId(hub);

                        if ((int)roleId == 15)
                        {
                            colorHex = "#737373";
                        }
                        else
                        {
                            var role = hub.roleManager.CurrentRole;
                            if (role != null)
                                colorHex = Misc.ToHex(role.RoleColor);
                        }
                    }

                    record.SetText(line.Replace("{RA_ClassColor}", colorHex));
                }
            }

            if (selectedIds.Count == 0)
                PlayerRecord.LastSelectedId = null;

            NorthwoodLib.Pools.ListPool<string>.Shared.Return(selectedIds);
        }
    }
}
