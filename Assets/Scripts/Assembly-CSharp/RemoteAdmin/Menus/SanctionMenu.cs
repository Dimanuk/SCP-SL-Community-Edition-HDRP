using RemoteAdmin.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin.Menus
{
    public class SanctionMenu : RaCommandMenu
    {
        [SerializeField]
        private Transform _rootParent;

        [SerializeField]
        private GameObject _template;

        [SerializeField]
        private Button _predefinedReasonsButton;

        protected override void OnStart()
        {
            if (Options != null)
            {
                foreach (var btn in Options.ToArray())
                {
                    if (btn != null)
                    {
                        Options.Remove(btn);
                        Destroy(btn.gameObject);
                    }
                }
            }

            if (_predefinedReasonsButton != null)
            {
                bool enabled = ServerConfigSynchronizer.Singleton?.EnableRemoteAdminPredefinedBanTemplates ?? false;
                _predefinedReasonsButton.interactable = enabled;
            }

            if (ServerConfigSynchronizer.Singleton?.EnableRemoteAdminPredefinedBanTemplates != true)
                return;

            var templates = ServerConfigSynchronizer.Singleton.RemoteAdminPredefinedBanTemplates;
            foreach (var template in templates)
            {
                if (_template == null || _rootParent == null)
                    continue;

                GameObject go = Instantiate(_template, _rootParent);

                if (!go.TryGetComponent<CustomSanctionButton>(out var sanctionBtn))
                {
                    Debug.LogError("Predefined Sanction button's template doesn't have the CustomSanctionButton component, correct this through Unity.");
                    continue;
                }

                sanctionBtn.CommandMenu = this;
                sanctionBtn.Value = string.Format("{0} {1}", template.Duration, template.Reason);

                if (sanctionBtn.Text != null)
                    sanctionBtn.Text.text = template.Reason;

                if (sanctionBtn.DurationLabel != null)
                {
                    if (template.Duration != 0)
                    {
                        sanctionBtn.DurationLabel.text = " BAN [ " + template.FormattedDuration + "]";
                        sanctionBtn.DurationLabel.color = Color.red;
                    }
                    else
                    {
                        sanctionBtn.DurationLabel.text = " KICK";
                        sanctionBtn.DurationLabel.color = Color.yellow;
                    }
                }

                if (Options != null && !Options.Contains(sanctionBtn))
                    Options.Add(sanctionBtn);
            }
        }
    }
}