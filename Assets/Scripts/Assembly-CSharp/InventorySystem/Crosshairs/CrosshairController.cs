using System;
using InventorySystem.Items;
using PlayerRoles;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Crosshairs
{
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _defaultCrosshair;
        [SerializeField] private MonoBehaviour[] _customCrosshairs;
        [SerializeField] private GameObject _rootObject;

        private static CrosshairController _singleton;

        private void Start()
        {
            _singleton = this;
            Inventory.OnCurrentItemChanged += OnItemChanged;
        }

        private void OnDestroy()
        {
            Inventory.OnCurrentItemChanged -= OnItemChanged;
        }

        private void Update()
        {
            bool showCrosshair = false;

            if (ReferenceHub.TryGetLocalHub(out ReferenceHub localHub))
            {
                PlayerRoleBase currentRole = localHub.roleManager?.CurrentRole;
                if (currentRole != null && PlayerRolesUtils.IsAlive(currentRole.RoleTypeId))
                    showCrosshair = !Cursor.visible;
            }

            if (_rootObject != null)
                _rootObject.SetActive(showCrosshair);
        }

        private static void OnItemChanged(ReferenceHub ply, ItemIdentifier prevItem, ItemIdentifier newItem)
        {
            if (!ply.isLocalPlayer || _singleton == null) return;

            Inventory inventory = ply.inventory;
            if (inventory == null || inventory.UserInventory == null) return;

            inventory.UserInventory.Items.TryGetValue(newItem.SerialNumber, out ItemBase currentItem);

            Type wantedType = (currentItem as ICustomCrosshairItem)?.CrosshairType;

            bool found = false;
            MonoBehaviour[] customCrosshairs = _singleton._customCrosshairs;
            for (int i = 0; i < customCrosshairs.Length; i++)
            {
                MonoBehaviour custom = customCrosshairs[i];
                if (custom == null) continue;

                bool match = !found && wantedType != null && custom.GetType().IsAssignableFrom(wantedType);
                custom.gameObject.SetActive(match);
                if (match) found = true;
            }

            if (_singleton._defaultCrosshair != null)
                _singleton._defaultCrosshair.gameObject.SetActive(!found);
        }
    }
}