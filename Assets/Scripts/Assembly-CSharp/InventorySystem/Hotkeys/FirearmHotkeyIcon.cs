using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using PlayerRoles;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Hotkeys
{
    public class FirearmHotkeyIcon : HotkeyIconBase
    {
        [SerializeField]
        private RawImage _rootIcon;

        [SerializeField]
        private RawImage[] _attachmentsPool;

        [SerializeField]
        private RectTransform _transformToFit;

        [SerializeField]
        private float _horizontalPadding;

        [SerializeField]
        private Vector2 _maxSize;

        private uint _prevAttachments;
        private RoleTypeId _prevRole;

        private Color _cachedTeamColor;

        protected override bool ForceRefresh(ushort serial)
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub hub))
                return false;

            if (hub == null)
                return false;

            var inventory = hub.inventory;
            if (inventory == null)
                return false;

            var userInventory = inventory.UserInventory;
            if (userInventory == null)
                return false;

            var items = userInventory.Items;
            if (items == null)
                return false;

            if (!items.TryGetValue(serial, out ItemBase item))
                return false;

            if (item == null)
                return false;

            if (!(item is Firearm firearm))
                return false;

            uint currentAttachments = AttachmentsUtils.GetCurrentAttachmentsCode(firearm);
            RoleTypeId currentRoleId = PlayerRolesUtils.GetRoleId(hub);

            bool changed = _prevRole != currentRoleId || _prevAttachments != currentAttachments;

            if (changed)
            {
                _prevRole = PlayerRolesUtils.GetRoleId(hub);
                _prevAttachments = currentAttachments;
            }

            return changed;
        }

        protected override void OnRefreshed(ushort serial)
        {
            _prevAttachments = 0;
            _prevRole = unchecked((RoleTypeId)255);
        }

        protected override void UpdateIcons(ushort serial, ItemBase ib)
        {
            if (ib == null)
                return;

            if (!(ib is Firearm firearm))
                return;

            ReferenceHub owner = firearm.Owner;
            if (owner == null)
                return;

            var roleManager = owner.roleManager;
            if (roleManager == null)
                return;

            var currentRole = roleManager.CurrentRole;
            if (currentRole == null)
                return;

            _cachedTeamColor = currentRole.RoleColor;

            Vector2 generatedSize = FirearmIconGenerator.GenerateIcon(
                firearm,
                _rootIcon,
                _attachmentsPool,
                _maxSize,
                GetAttachmentColor
            );

            if (_rootIcon != null)
                _rootIcon.color = _cachedTeamColor;

            if (_transformToFit != null)
            {
                Vector2 sizeDelta = _transformToFit.sizeDelta;
                sizeDelta.x = generatedSize.x + _horizontalPadding;
                _transformToFit.sizeDelta = sizeDelta;
            }
        }

        private Color GetAttachmentColor(int attachmentIndex)
        {
            return _cachedTeamColor;
        }
    }
}
