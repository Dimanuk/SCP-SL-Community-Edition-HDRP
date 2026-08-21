using InventorySystem.Items;
using PlayerRoles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace InventorySystem.Drawers
{
    public class InventoryDrawersController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _alertText;

        [SerializeField]
        private Slider _progressbarSlider;

        [SerializeField]
        private RectTransform _progressbarSliderRect;

        [SerializeField]
        private Graphic[] _detailsToRecolor;

        [SerializeField]
        private CanvasGroup _thisGroup;

        [SerializeField]
        private CanvasGroup _inventoryGroup;

        private IItemAlertDrawer _alertToTrack;

        private IItemProgressbarDrawer _progressbarToTrack;

        private bool _active;

        private void Start()
        {
            InventorySystem.Inventory.OnCurrentItemChanged += ItemChanged;
            PlayerRoleManager.OnRoleChanged += RecolorDetails;
        }

        private void OnDestroy()
        {
            InventorySystem.Inventory.OnCurrentItemChanged -= ItemChanged;
            PlayerRoleManager.OnRoleChanged -= RecolorDetails;
        }

        private void RecolorDetails(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (hub == null || !hub.isLocalPlayer)
                return;

            Color roleColor = newRole.RoleColor;

            foreach (Graphic graphic in _detailsToRecolor)
            {
                if (graphic != null)
                {
                    // Apply the role RGB but preserve each graphic's own alpha, so the dim
                    // progress-bar track (a≈0.06) and translucent fill (a≈0.12) stay translucent.
                    // Overwriting alpha with the role colour's 1.0 made both opaque and identical,
                    // which is why the use progress bar looked permanently full.
                    Color existing = graphic.color;
                    graphic.color = new Color(roleColor.r, roleColor.g, roleColor.b, existing.a);
                }
            }
        }

        private void Update()
        {
            if (!_active)
                return;

            // Drawer HUD (item alert/hint + progress bar) is shown while the inventory
            // is CLOSED and fades out as it opens: alpha = 1 - inventoryGroup.alpha.
            if (_inventoryGroup != null && _thisGroup != null)
            {
                _thisGroup.alpha = 1f - _inventoryGroup.alpha;
            }

            if (_alertText != null)
            {
                _alertText.text = (_alertToTrack != null) ? _alertToTrack.AlertText : string.Empty;
            }

            if (_progressbarSlider != null)
            {
                bool shouldShowBar = _progressbarToTrack != null && _progressbarToTrack.ProgressbarEnabled;

                if (shouldShowBar)
                {
                    if (!_progressbarSlider.gameObject.activeSelf)
                        _progressbarSlider.gameObject.SetActive(true);

                    _progressbarSlider.minValue = _progressbarToTrack.ProgressbarMin;
                    _progressbarSlider.maxValue = _progressbarToTrack.ProgressbarMax;
                    _progressbarSlider.value = _progressbarToTrack.ProgressbarValue;

                    if (_progressbarSliderRect != null)
                    {
                        Vector2 size = _progressbarSliderRect.sizeDelta;
                        size.x = _progressbarToTrack.ProgressbarWidth;
                        _progressbarSliderRect.sizeDelta = size;
                    }
                }
                else
                {
                    if (_progressbarSlider.gameObject.activeSelf)
                        _progressbarSlider.gameObject.SetActive(false);
                }
            }
        }

        private void ItemChanged(ReferenceHub hub, ItemIdentifier prevItem, ItemIdentifier newItem)
        {
            if (hub == null || !hub.isLocalPlayer)
                return;

            _active = false;
            _alertToTrack = null;
            _progressbarToTrack = null;

            if (_alertText != null)
                _alertText.text = string.Empty;

            if (newItem.SerialNumber != 0 && hub.inventory.UserInventory.Items.TryGetValue(newItem.SerialNumber, out ItemBase itemBase))
            {
                _alertToTrack = itemBase as IItemAlertDrawer;
                _progressbarToTrack = itemBase as IItemProgressbarDrawer;

                _active = (_alertToTrack != null || _progressbarToTrack != null);
            }

            if (_progressbarToTrack == null && _progressbarSlider != null)
            {
                _progressbarSlider.gameObject.SetActive(false);
            }
        }
    }
}