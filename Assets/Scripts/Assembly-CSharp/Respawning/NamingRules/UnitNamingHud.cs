using System.Text;
using PlayerRoles;
using TMPro;
using UnityEngine;

namespace Respawning.NamingRules
{
    public class UnitNamingHud : MonoBehaviour
    {
        public TextMeshProUGUI UnitsList;

        private SpawnableTeamType _localTeam;
        private int _localIndex;
        private static readonly StringBuilder StrBuilder = new StringBuilder();

        private void AppendUnit(int index, string unitName)
        {
            if (_localIndex != index)
            {
                StrBuilder.Append(unitName);
                StrBuilder.Append("\n");
            }
            else
            {
                StrBuilder.Append("<u>");
                StrBuilder.Append(unitName);
                StrBuilder.Append("</u>\n");
            }
        }

        private void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (!hub.isLocalPlayer)
                return;

            if (newRole is HumanRole humanRole && humanRole.UsesUnitNames)
            {
                _localTeam = humanRole.UnitNameTeam;
                _localIndex = humanRole.UnitNameIndex;

                if (UnitsList != null)
                    UnitsList.enabled = true;

                StrBuilder.Clear();
                if (UnitNameMessageHandler.ReceivedNames.TryGetValue(_localTeam, out var names))
                {
                    for (int i = 0; i < names.Count; i++)
                    {
                        AppendUnit(i, names[i]);
                    }
                }

                if (UnitsList != null)
                    UnitsList.text = StrBuilder.ToString();
            }
            else
            {
                _localTeam = SpawnableTeamType.None;
                if (UnitsList != null)
                    UnitsList.enabled = false;
            }
        }

        private void OnMsgReceived(SpawnableTeamType team, string unit, int index)
        {
            if (team != _localTeam)
                return;

            AppendUnit(index, unit);

            if (UnitsList != null)
                UnitsList.text = StrBuilder.ToString();
        }

        private void OnDestroy()
        {
            PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
            UnitNameMessageHandler.OnNameAdded -= OnMsgReceived;
        }

        private void Start()
        {
            PlayerRoleManager.OnRoleChanged += OnRoleChanged;
            UnitNameMessageHandler.OnNameAdded += OnMsgReceived;
        }
    }
}