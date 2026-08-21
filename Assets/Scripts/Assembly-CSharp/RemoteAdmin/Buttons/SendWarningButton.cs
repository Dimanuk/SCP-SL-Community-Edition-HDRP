using System;
using System.Linq;
using RemoteAdmin.Elements;
using UnityEngine;

namespace RemoteAdmin.Buttons
{
    public class SendWarningButton : SendButton
    {
        [SerializeField]
        private GameObject _warning;

        public override void Select()
        {
            SubmenuSelector.Singleton?.SelectedMenu?.SetResponse(Color.white, string.Empty);

            int selectedCount = PlayerRecord.Instances.Count(r => r.IsSelected);
            int threshold = Mathf.Max(1, PlayerRecord.Instances.Count / 2);

            // Warn (and abort) when trying to hit half the server or more at once.
            if (selectedCount >= threshold)
            {
                if (_warning != null)
                    _warning.SetActive(true);

                return;
            }

            if (string.IsNullOrEmpty(CustomCommand))
            {
                Debug.LogError($"[SendButton] {gameObject.name} has a null/empty custom command.", gameObject);
                return;
            }

            base.Select();
        }
    }
}